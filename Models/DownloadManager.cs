using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
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

    public async Task DownloadItem(PodcastFeed feed, SyndicationItem item)
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
                HttpResponseMessage response = await client.GetAsync(audioLink);
                using (FileStream fs = 
                    new FileStream(DownloadedItemPath(feed, item, audioLink), 
                    FileMode.CreateNew))
                {
                    await response.Content.CopyToAsync(fs); 
                }  
            } 
            catch (InvalidOperationException)
            {
                return;
            }
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