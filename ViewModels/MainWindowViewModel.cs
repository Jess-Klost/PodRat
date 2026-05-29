using System;
using System.Collections.ObjectModel;
using System.IO;
using System.ServiceModel.Syndication;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RSSPod.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    const string DownloadDirectory = "AudioDownloads";
    const string UserDataFile = "userdata.json";

    [ObservableProperty]
    public partial double Position { get; set; } = 0;
    [ObservableProperty]
    public partial long LengthMS { get; set; }
    [ObservableProperty]
    public partial double Volume { get; set; } = 50;
    [ObservableProperty]
    public partial string Title { get; set; }
    [ObservableProperty]
    public partial string Author { get; set; }
    [ObservableProperty]
    public partial ObservableCollection<PodcastFeedItem>? CurrentFeed { get; set; }
    [ObservableProperty]
    public partial UserData UserData { get; set; } = new UserData();

    public event EventHandler<string, bool> UpdateDownloadStatus;
    public event EventHandler<string, float> ProgressChanged;

    AudioPlayer audioPlayer;
    RSSFeedReader feedReader;
    DownloadManager downloadManager = new DownloadManager(DownloadDirectory);
    PodcastFeed? selectedFeed;

    public MainWindowViewModel()
    {
        audioPlayer = new AudioPlayer();
        audioPlayer.audioPositionChanged += AudioPositionChanged;
        audioPlayer.playingStart += PlayingStart;
        audioPlayer.SetVolume((int)Volume);

        LoadUserData();
    }

    void LoadUserData()
    {
        try
        {
            using StreamReader fileReader = new (UserDataFile);
            string jsonString = fileReader.ReadToEnd();
            UserData? data = JsonSerializer.Deserialize<UserData>(jsonString);
            if (data != null)
                UserData = data;
        }
        catch (FileNotFoundException)
        {
            return;
        }
    }

    void SaveUserData()
    {
        string jsonString = JsonSerializer.Serialize(UserData);
        File.WriteAllText(UserDataFile, jsonString);
    }

    async void PopulateFeed()
    {
        if (selectedFeed == null)
            return;
        feedReader = new RSSFeedReader(selectedFeed.Uri);
        await feedReader.ReadRSSFeed();
        CurrentFeed = feedReader.GetFeedItems(selectedFeed);
    }

    void AudioPositionChanged(object? sender, float position)
    {
        Position = position;
    }

    /// <summary>
    /// Called when audioPlayer is first starts playing the newly loaded audio
    /// </summary>
    void PlayingStart(object? sender, EventArgs e)
    {
        LengthMS = audioPlayer.GetLengthTime();
        // Position has been set before first play, 
        // must set it after audio is played for the first time
        if (Position != 0) 
        {
            audioPlayer.SetPosition((float)Position);
        }
    }

    public void PlayAudio()
    {
        audioPlayer.Play();
    }

    public void PauseAudio()
    {
        audioPlayer.Pause();
    }

    public void ChangeVolume(int volume)
    {
        audioPlayer.SetVolume(volume);
    }

    public void EditPosition(float position)
    {
        audioPlayer.SetPosition(position);
    }

    public void FastForward(float time)
    {
        if (LengthMS == 0)
            return;
        float percentChange = TimeToPercent(time);
        audioPlayer.SetPosition(audioPlayer.GetPosition() + percentChange);
        // Change position property for immediate UI feedback
        Position += percentChange;
        Position = Math.Clamp(Position, 0, 1.0);
    }

    public bool IsPlaying()
    {
        return audioPlayer.IsPlaying();
    }

    float TimeToPercent(float time)
    {
        return time / audioPlayer.GetLengthTime();
    }

    public async Task DownloadItem(string id)
    {
        if (selectedFeed == null)
            return;
        if (feedReader.GetItem(id, out SyndicationItem? item) && item != null)
        {
            Progress<float> progress = new Progress<float>();
            progress.ProgressChanged += (sender, progress) => ProgressChanged.Invoke(id, progress);
            await downloadManager.DownloadItem(selectedFeed, item, progress);
            UpdateDownloadStatus?.Invoke(id, true);
        }
    }

    public void OnFeedChanged(PodcastFeed newFeed)
    {
        selectedFeed = newFeed;
        PopulateFeed();
    }

    public void AddFeed(string name, string uri)
    {
        PodcastFeed feed = new PodcastFeed(name, uri);
        UserData.PodcastFeeds.Add(feed);
        SaveUserData();
    }

    public void RemoveFeed()
    {
        if (selectedFeed == null)
            return;
        UserData.PodcastFeeds.Remove(selectedFeed);
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

    public void LoadItem(string id)
    {
        if (!feedReader.GetItem(id, out SyndicationItem? item) || item == null 
            || selectedFeed == null)
        {
            return;
        }
        audioPlayer.LoadAudio(DownloadManager.DownloadedItemPath(selectedFeed, item));
        Title = item.Title.Text;
        Author = selectedFeed.Name;
        Position = 0;
        LengthMS = 0;
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
}
