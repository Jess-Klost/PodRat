using System.Collections.ObjectModel;

namespace RSSPod.Models;

public class UserData
{
    public ObservableCollection<PodcastFeed> PodcastFeeds { get; set; } = new ObservableCollection<PodcastFeed>();
}