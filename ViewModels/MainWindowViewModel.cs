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


    public MainWindowViewModel()
    {
        audioPlayer = new AudioPlayer();
        audioPlayer.audioPositionChanged += AudioPositionChanged;
        audioPlayer.SetVolume((int)Volume);
        audioPlayer.LoadAudio("/home/aklost/github/RSSPod/Where_Do_We_Begin_Episode_57_Brant_Summer.mp3");
        PopulateFeed();
    }

    async void PopulateFeed()
    {
        RSSFeedReader feedReader = new RSSFeedReader("https://feed.podbean.com/wayneradiotv/feed.xml");
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
}
