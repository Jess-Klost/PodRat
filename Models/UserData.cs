using System.Collections.ObjectModel;

public class UserData
{
    public ObservableCollection<PodcastFeed> PodcastFeeds { get; set; } = new ObservableCollection<PodcastFeed>();
}