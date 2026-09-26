using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using PodRat.Models;

namespace PodRat.ViewModels;

public partial class FeedViewerViewModel : ViewModelBase
{
    // Keep track of instances, so a call can be made on exit
    public static List<FeedViewerViewModel> Instances { get; } = new List<FeedViewerViewModel>(); 

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

    public event EventHandler<string, DownloadManager.DownloadStatus>? UpdateDownloadStatus;
    public event EventHandler<string, float>? ProgressChanged;
    public event EventHandler? SelectedFeedModified;

    RSSFeedReader? feedReader;
    Dictionary<string, CancellationTokenSource> currentDownloadCancellationTokens = new Dictionary<string, CancellationTokenSource>(); 

    public FeedViewerViewModel()
    {
        UserData = UserDataInstancer.GetUserData();
        UserDataInstancer.UserDataChanged += OnUserDataChanged;
        UserDataInstancer.LoadUserData();

        Instances.Add(this);
    }

    ~FeedViewerViewModel()
    {
        CancelAllDownloads();
    }

    public void OnExit()
    {
        CancelAllDownloads();
        // Must clean up downloads on the main thread, since task clean up is 
        // not guaranteed to run when exiting
        DownloadManager.CleanUpCurrentDownloads();
    }

    void CancelAllDownloads()
    {
        foreach (string cancellationToken in currentDownloadCancellationTokens.Keys)
        {
            currentDownloadCancellationTokens[cancellationToken].Cancel();
            currentDownloadCancellationTokens[cancellationToken].Dispose();
        }
    }

    void OnUserDataChanged(object? sender, UserDataChangedEventArgs e)
    {
        UserData = UserDataInstancer.GetUserData();
        if (e.changeType == UserDataChangedEventArgs.ChangeType.RenameFeed)
        {
            if (e.affectedFeed == SelectedFeed)
                SelectedFeedModified?.Invoke(this, EventArgs.Empty);
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
        if (SelectedFeed == null || feedReader == null)
            return;
        if (feedReader.GetItem(id, out SyndicationItem? item) && item != null)
        {
            currentDownloadCancellationTokens.Add(id, new CancellationTokenSource());
            try 
            {
                Progress<float> progress = new Progress<float>();
                progress.ProgressChanged += (sender, progress) => ProgressChanged?.Invoke(id, progress);
                await UserDataInstancer.DownloadManagerInstance.DownloadItem(SelectedFeed, 
                    item, progress, currentDownloadCancellationTokens[id].Token);
                // Update with current status, since the item either finished 
                // downloading or the download was canceled 
                UpdateDownloadStatus?.Invoke(id, 
                    DownloadManager.GetDownloadStatus(SelectedFeed, item));
            }
            finally
            {
                currentDownloadCancellationTokens[id].Dispose();
                currentDownloadCancellationTokens.Remove(id);
            }
        }
    }

    public async Task CancelDownloadItem(string id)
    {
        if (SelectedFeed == null)
            return;
        if (currentDownloadCancellationTokens.ContainsKey(id))
        {
            currentDownloadCancellationTokens[id].Cancel();
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
        CurrentFeed = null;
    }

    public bool ItemDownloaded(string id)
    {
        if (feedReader == null)
            return false;
        if (!feedReader.GetItem(id, out SyndicationItem? item) || item == null 
            || SelectedFeed == null)
        {
            return false;
        }
        return DownloadManager.IsDownloaded(SelectedFeed, item);
    }

    public DownloadManager.DownloadStatus ItemDownloadStatus(string id)
    {
        if (feedReader == null)
            return DownloadManager.DownloadStatus.notDownloaded;
        if (!feedReader.GetItem(id, out SyndicationItem? item) || item == null 
            || SelectedFeed == null)
        {
            return DownloadManager.DownloadStatus.notDownloaded;
        }
        return DownloadManager.GetDownloadStatus(SelectedFeed, item);
    }

    public void DeleteItem(string id)
    {
        if (SelectedFeed == null || feedReader == null)
            return;
        if (feedReader.GetItem(id, out SyndicationItem? item) && item != null)
        {
            UserDataInstancer.DownloadManagerInstance.DeleteItem(SelectedFeed, item);
            UpdateDownloadStatus?.Invoke(id, DownloadManager.GetDownloadStatus(SelectedFeed, item));
        }
    }

    public PodcastFeedItem? GetItem(string id)
    {
        if (feedReader == null)
            return null;
        if (!feedReader.GetItem(id, out SyndicationItem? item) || item == null 
            || SelectedFeed == null)
        {
            return null;
        }
        return new PodcastFeedItem { Feed = SelectedFeed, Item = item };
    }
}