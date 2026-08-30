using System;
using System.Collections.ObjectModel;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using RSSPod.Models;

namespace RSSPod.ViewModels;

public partial class FeedViewerViewModel : ViewModelBase
{
    const string DownloadDirectory = "AudioDownloads";
    
    [ObservableProperty]
    public partial ObservableCollection<PodcastFeedItem>? CurrentFeed { get; set; }
    [ObservableProperty]
    public partial bool CurrentFeedLoading { get; set; } = false;
    [ObservableProperty]
    public partial UserData UserData { get; set; }

    public event EventHandler<string, bool> UpdateDownloadStatus;
    public event EventHandler<string, float> ProgressChanged;

    RSSFeedReader feedReader;
    DownloadManager downloadManager = new DownloadManager(DownloadDirectory);
    PodcastFeed? selectedFeed;

    public FeedViewerViewModel()
    {
        UserData = UserDataInstancer.GetUserData();
        UserDataInstancer.UserDataChanged += OnUserDataChanged;
        UserDataInstancer.LoadUserData();
    }

    void SaveUserData()
    {
        UserDataInstancer.SaveUserData();
    }

    void OnUserDataChanged(object? sender, EventArgs e)
    {
        UserData = UserDataInstancer.GetUserData();
    }

    async void PopulateFeed()
    {
        if (selectedFeed == null)
            return;
        CurrentFeedLoading = true;
        feedReader = new RSSFeedReader(selectedFeed.Uri);
        await feedReader.ReadRSSFeed();
        CurrentFeed = feedReader.GetFeedItems(selectedFeed);
        CurrentFeedLoading = false;
    }

    public async Task DownloadItem(string id)
    {
        if (selectedFeed == null)
            return;
        if (feedReader.GetItem(id, out SyndicationItem? item) && item != null)
        {
            Progress<float> progress = new Progress<float>();
            progress.ProgressChanged += (sender, progress) => ProgressChanged?.Invoke(id, progress);
            await downloadManager.DownloadItem(selectedFeed, item, progress);
            UpdateDownloadStatus?.Invoke(id, true);
        }
    }

    public void OnFeedChanged(PodcastFeed newFeed)
    {
        selectedFeed = newFeed;
        Task.Run(() => PopulateFeed());
    }

    public void RemoveFeed()
    {
        if (selectedFeed == null)
            return;
        UserDataInstancer.RemoveFeed(selectedFeed);
        downloadManager.DeleteFeed(selectedFeed);
        CurrentFeed = null;
        SaveUserData();
    }

    public bool ItemDownloaded(string id)
    {
        if (!feedReader.GetItem(id, out SyndicationItem? item) || item == null 
            || selectedFeed == null)
        {
            return false;
        }
        return DownloadManager.IsDownloaded(selectedFeed, item);
    }

    public void DeleteItem(string id)
    {
        if (selectedFeed == null)
            return;
        if (feedReader.GetItem(id, out SyndicationItem? item) && item != null)
        {
            downloadManager.DeleteItem(selectedFeed, item);
            UpdateDownloadStatus?.Invoke(id, false);
        }
    }

    public PodcastFeedItem? GetItem(string id)
    {
        if (!feedReader.GetItem(id, out SyndicationItem? item) || item == null 
            || selectedFeed == null)
        {
            return null;
        }
        return new PodcastFeedItem { Feed = selectedFeed, Item = item };
    }
}