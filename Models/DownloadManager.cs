using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;

public class DownloadManager
{
    string downloadDirectory = "AudioDownloads";

    public DownloadManager()
    {
        if (!Directory.Exists(downloadDirectory))
            Directory.CreateDirectory(downloadDirectory);
    }

    public DownloadManager(string downloadDirectory)
    {
        this.downloadDirectory = downloadDirectory;
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
                    new FileStream(Path.Combine(downloadDirectory, feed.Name, audioLink.Segments.Last()), 
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

    bool FindAudioLink(SyndicationItem item, out Uri? audioLink)
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
}