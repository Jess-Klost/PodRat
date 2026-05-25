using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

public class PodcastFeedItem
{
    public required PodcastFeed Feed { get; set; }
    public required SyndicationItem Item { get; set; }
    public Task<bool> IsDownloaded => GetIsDownloaded();

    private async Task<bool> GetIsDownloaded()
    {
        return DownloadManager.IsDownloaded(Feed, Item);    
    }
}