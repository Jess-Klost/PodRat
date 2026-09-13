using System.Collections.ObjectModel;

namespace PodRat.Models;

public class UserData
{
    public ObservableCollection<PodcastFeed> PodcastFeeds { get; set; } = new ObservableCollection<PodcastFeed>();
}