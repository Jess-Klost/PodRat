using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using RSSPod.Models;

namespace RSSPod.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
        UserDataInstancer.LoadUserData();
        // Attempt to get listen data for all feeds
        foreach (PodcastFeed feed in UserDataInstancer.GetUserData().PodcastFeeds)
            ListenDataManager.LoadListenData(feed);
    }
}
