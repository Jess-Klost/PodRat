using System;
using System.Collections.ObjectModel;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using RSSPod.Models;
using RSSPod.Views;

namespace RSSPod.ViewModels;

public partial class FeedViewerViewModel : ViewModelBase
{
    const string DownloadDirectory = "AudioDownloads";
    
    [ObservableProperty]
    public partial ObservableCollection<PodcastFeedItem>? CurrentFeed { get; set; }
    [ObservableProperty]
    public partial PodcastFeed? SelectedFeed { get; set; }
    [ObservableProperty]
    public partial bool CurrentFeedLoading { get; set; } = false;
    [ObservableProperty]
    // Start valid, so no feed selected is properly displayed
    public partial bool CurrentFeedValid { get; set; } = true;
    [ObservableProperty]
    public partial UserData UserData { get; set; }
    [ObservableProperty]
    public partial bool EditFeedPopupVisible { get; set; } = false;

    public event EventHandler<string, bool> UpdateDownloadStatus;
    public event EventHandler<string, float> ProgressChanged;
    public event EventHandler SelectedFeedModified;

    RSSFeedReader feedReader;
    DownloadManager downloadManager = new DownloadManager(DownloadDirectory);

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

    void OnUserDataChanged(object? sender, UserDataChangedEventArgs e)
    {
        UserData = UserDataInstancer.GetUserData();
        if (e.changeType == UserDataChangedEventArgs.ChangeType.RenameFeed)
        {
            if (e.affectedFeed == SelectedFeed)
                SelectedFeedModified.Invoke(this, EventArgs.Empty);
        }
    }

    async void PopulateFeed()
    {
        if (SelectedFeed == null)
            return;
        CurrentFeedLoading = true;
        feedReader = new RSSFeedReader(SelectedFeed.Uri);
        CurrentFeedValid = await feedReader.ReadRSSFeed();
        if (CurrentFeedValid)
            CurrentFeed = feedReader.GetFeedItems(SelectedFeed);
        else
            CurrentFeed = null;
        CurrentFeedLoading = false;
    }

    public async Task DownloadItem(string id)
    {
        if (SelectedFeed == null)
            return;
        if (feedReader.GetItem(id, out SyndicationItem? item) && item != null)
        {
            Progress<float> progress = new Progress<float>();
            progress.ProgressChanged += (sender, progress) => ProgressChanged?.Invoke(id, progress);
            await downloadManager.DownloadItem(SelectedFeed, item, progress);
            UpdateDownloadStatus?.Invoke(id, true);
        }
    }

    public void OnFeedChanged(PodcastFeed newFeed)
    {
        SelectedFeed = newFeed;
        Task.Run(() => PopulateFeed());
    }

    public void RemoveFeed()
    {
        if (SelectedFeed == null)
            return;
        UserDataInstancer.RemoveFeed(SelectedFeed);
        downloadManager.DeleteFeed(SelectedFeed);
        CurrentFeed = null;
        SaveUserData();
    }

    public bool ItemDownloaded(string id)
    {
        if (!feedReader.GetItem(id, out SyndicationItem? item) || item == null 
            || SelectedFeed == null)
        {
            return false;
        }
        return DownloadManager.IsDownloaded(SelectedFeed, item);
    }

    public void DeleteItem(string id)
    {
        if (SelectedFeed == null)
            return;
        if (feedReader.GetItem(id, out SyndicationItem? item) && item != null)
        {
            downloadManager.DeleteItem(SelectedFeed, item);
            UpdateDownloadStatus?.Invoke(id, false);
        }
    }

    public PodcastFeedItem? GetItem(string id)
    {
        if (!feedReader.GetItem(id, out SyndicationItem? item) || item == null 
            || SelectedFeed == null)
        {
            return null;
        }
        return new PodcastFeedItem { Feed = SelectedFeed, Item = item };
    }
}