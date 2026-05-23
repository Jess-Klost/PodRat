using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel.Syndication;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RSSPod.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial double Position { get; set; } = 0;
    [ObservableProperty]
    public partial double Volume { get; set; } = 50;
    [ObservableProperty]
    public partial string Title { get; set; } = "Audio Title";
    [ObservableProperty]
    public partial ObservableCollection<SyndicationItem> Feed { get; set; }

    AudioPlayer audioPlayer;
    RSSFeedReader feedReader = new RSSFeedReader("");

    public MainWindowViewModel()
    {
        audioPlayer = new AudioPlayer();
        audioPlayer.audioPositionChanged += AudioPositionChanged;
        audioPlayer.SetVolume((int)Volume);
        audioPlayer.LoadAudio("a");
        PopulateFeed();
    }

    async void PopulateFeed()
    {
        await feedReader.ReadRSSFeed();
        Feed = feedReader.GetFeedItems();
    }

    void AudioPositionChanged(object? sender, float position)
    {
        Position = position;
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
        float percentChange = TimeToPercent(time);
        audioPlayer.SetPosition(audioPlayer.GetPosition() + percentChange);
    }

    public bool IsPlaying()
    {
        return audioPlayer.IsPlaying();
    }

    float TimeToPercent(float time)
    {
        return time / audioPlayer.GetLengthTime();
    }

    public void DownloadItem(string id)
    {
        feedReader.GetItem(id, out SyndicationItem? item);
    }
}
