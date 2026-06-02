using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Threading.Tasks;

public class DownloadManager
{
    public static string downloadDirectory { get; set; } = "AudioDownloads";

    public DownloadManager()
    {
        if (!Directory.Exists(downloadDirectory))
            Directory.CreateDirectory(downloadDirectory);
    }

    public DownloadManager(string downloadDirectory)
    {
        DownloadManager.downloadDirectory = downloadDirectory;
        if (!Directory.Exists(downloadDirectory))
            Directory.CreateDirectory(downloadDirectory);
    }

    public async Task DownloadItem(PodcastFeed feed, SyndicationItem item, IProgress<float>? progress = null, CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(Path.Combine(downloadDirectory, feed.Name)))
            Directory.CreateDirectory(Path.Combine(downloadDirectory, feed.Name));
        if (!FindAudioLink(item, out Uri? audioLink))
        {
            throw new InvalidOperationException("DownloadManager: No audio link found in item");
        }
        if (audioLink == null)
            throw new InvalidOperationException("DownloadManager: No audio link found in item");
        using (HttpClient client = new HttpClient())
        {
            try
            {
                using (var response = await client.GetAsync(audioLink, HttpCompletionOption.ResponseHeadersRead)) 
                {
                    var contentLength = response.Content.Headers.ContentLength;
                    using (var download = await response.Content.ReadAsStreamAsync())
                    using (FileStream fs = 
                        new FileStream(DownloadedItemPath(feed, item, audioLink), 
                        FileMode.CreateNew)) 
                    {
                        if (progress is null || !contentLength.HasValue) 
                        {
                            await download.CopyToAsync(fs);
                            return;
                        }
                        var progressWrapper = new Progress<long> (totalBytes =>
                             progress.Report(GetProgressPercentage (totalBytes, contentLength.Value)));
                        await CopyToAsync(download, fs, 81920, progressWrapper, cancellationToken);
                    }
                }

                float GetProgressPercentage (float totalBytes, float currentBytes) => (totalBytes / currentBytes) * 100f;
            } 
            catch (InvalidOperationException)
            {
                return;
            }
        }
    }

    public void DeleteItem(PodcastFeed feed, SyndicationItem item)
    {
        if (!IsDownloaded(feed, item))
            return;
        File.Delete(DownloadedItemPath(feed, item));
    }

    public void DeleteFeed(PodcastFeed feed)
    {
        if (Directory.Exists(Path.Combine(downloadDirectory, feed.Name)))
        {
            Directory.Delete(Path.Combine(downloadDirectory, feed.Name), true);        
        }
    }

    static async Task CopyToAsync(Stream source, Stream destination,
        int bufferSize = 81920, IProgress<long>? progress = null,
        CancellationToken cancellationToken = default)
    {
        if (bufferSize < 0)
            throw new ArgumentOutOfRangeException(nameof(bufferSize));
        if (source is null)
            throw new ArgumentNullException(nameof(source));
        if (!source.CanRead)
            throw new InvalidOperationException($"'{nameof (source)}' is not readable.");
        if (destination == null)
            throw new ArgumentNullException (nameof(destination));
        if (!destination.CanWrite)
            throw new InvalidOperationException($"'{nameof (destination)}' is not writable.");

        var buffer = new byte[bufferSize];
        long totalBytesRead = 0;
        int bytesRead;
        while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length,
            cancellationToken).ConfigureAwait (false)) != 0)
        {
            await destination.WriteAsync(buffer, 0, bytesRead, 
                cancellationToken).ConfigureAwait (false);
            totalBytesRead += bytesRead;
            progress?.Report(totalBytesRead);
        }
    }

    public static bool IsDownloaded(PodcastFeed feed, SyndicationItem item)
    {
        if (FindAudioLink(item, out Uri? audioLink) && audioLink != null)
        {
            return File.Exists(DownloadedItemPath(feed, item, audioLink));
        }
        return false;
    }

    static bool FindAudioLink(SyndicationItem item, out Uri? audioLink)
    {
        foreach(SyndicationLink link in item.Links)
        {
            if (link.MediaType == "audio/mpeg")
            {
                audioLink = link.Uri;
                return true;
            }
        }
        audioLink = null;
        return false;
    }

    public static string DownloadedItemPath(PodcastFeed feed, SyndicationItem item, Uri audioLink)
    {
        return Path.GetFullPath(Path.Combine(downloadDirectory, feed.Name, audioLink.Segments.Last()));
    }

    public static string DownloadedItemPath(PodcastFeed feed, SyndicationItem item)
    {
        FindAudioLink(item, out Uri? audioLink);
        if (audioLink == null)
            throw new InvalidOperationException("Download Manager: no AudioLink found on item, therefore download is impossible");
        return DownloadedItemPath(feed, item, audioLink);
    }
}