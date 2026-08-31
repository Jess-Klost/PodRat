using System.ServiceModel.Syndication;
using System.Threading.Tasks;

namespace RSSPod.Models;

public class PodcastFeedItem
{
    public required PodcastFeed Feed { get; set; }
    public required SyndicationItem Item { get; set; }
    public Task<bool> IsDownloaded => GetIsDownloaded();
    public string Duration { get => GetDuration(); }
    public string ThumbnailLink { get => GetThumbnailLink(); }
    public long FileSize { get => GetFileSize(); }
    public float ListenDataPosition { get => ListenDataManager.GetListenDataPosition(Feed, Item.Id); }

    private async Task<bool> GetIsDownloaded()
    {
        return DownloadManager.IsDownloaded(Feed, Item);    
    }

    public string GetAudioPath()
    {
        return DownloadManager.DownloadedItemPath(Feed, Item);
    }

    public string GetThumbnailLink()
    {
        RSSFeedReader.GetImageFromItem(Item, out string imageResult);
        return imageResult;
    }

    public string GetDuration()
    {
        RSSFeedReader.GetDurationFromItem(Item, out string duration);
        return duration;
    }

    public long GetFileSize()
    {
        foreach(SyndicationLink link in Item.Links)
        {
            if (link.MediaType == "audio/mpeg")
            {
                return link.Length;
            }
        }
        return 0;
    }
}