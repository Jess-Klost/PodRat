using System;
using CommunityToolkit.Mvvm.ComponentModel;
using RSSPod.Models;

namespace RSSPod.ViewModels;

public partial class MediaPlayerViewModel : ViewModelBase
{
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
    public partial string ThumbnailLink { get; set; }

    AudioPlayer audioPlayer;

    public MediaPlayerViewModel()
    {
        audioPlayer = new AudioPlayer();
        audioPlayer.audioPositionChanged += AudioPositionChanged;
        audioPlayer.playingStart += PlayingStart;
        audioPlayer.SetVolume((int)Volume);
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

    void AudioPositionChanged(object? sender, float position)
    {
        Position = position;
    }

    float TimeToPercent(float time)
    {
        return time / audioPlayer.GetLengthTime();
    }

    public void LoadItem(PodcastFeedItem item)
    {
        audioPlayer.LoadAudio(item.GetAudioPath());
        Title = item.Item.Title.Text;
        Author = item.Feed.Name;
        ThumbnailLink = item.GetThumbnailLink();
        Position = 0;
        LengthMS = 0;
    }
}