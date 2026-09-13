using System.Collections.ObjectModel;

namespace PodBat.Models;

public class UserData
{
    public ObservableCollection<PodcastFeed> PodcastFeeds { get; set; } = new ObservableCollection<PodcastFeed>();
}