using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using PodRat.Models;

namespace PodRat.ViewModels;

public partial class MediaPlayerViewModel : ViewModelBase
{
    // Keep track of instances, so a call can be made on exit
    public static List<MediaPlayerViewModel> Instances { get; } = new List<MediaPlayerViewModel>(); 

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

    PodcastFeedItem? currentLoadedItem = null;

    public MediaPlayerViewModel()
    {
        audioPlayer = new AudioPlayer();
        audioPlayer.audioPositionChanged += AudioPositionChanged;
        audioPlayer.playingStart += PlayingStart;
        audioPlayer.SetVolume((int)Volume);

        Instances.Add(this);
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

    public void EditPosition(float position, bool shouldBePlaying = false)
    {
        if (shouldBePlaying && audioPlayer.GetPosition() == 1f)
        {
            audioPlayer.Play();
        }
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
        if (currentLoadedItem != null) 
        {
            ListenDataManager.UpdateListenData(currentLoadedItem.Feed, currentLoadedItem.Item.Id, audioPlayer.GetPosition());
        }
        bool hasAudio = item.GetAudioPath(out string path);
        if (!hasAudio) // Audio does not exist for this item, cannot play 
            return;
        audioPlayer.LoadAudio(path);
        Title = item.Item.Title.Text;
        Author = item.Feed.Name;
        ThumbnailLink = item.GetThumbnailLink();
        Position = UserDataInstancer.UserPreferencesInstance.AutoResume ? 
            ListenDataManager.GetListenDataPosition(item.Feed, item.Item.Id) : 0;
        if (Position == 1) Position = 0; // if episode is played after completed, go back to start 
        LengthMS = 0;
        currentLoadedItem = item;
    }

    public void OnExit()
    {
        // Track listen data for currently loaded item
        if (currentLoadedItem != null) 
        {
            ListenDataManager.UpdateListenData(currentLoadedItem.Feed, currentLoadedItem.Item.Id, audioPlayer.GetPosition());
        }
    }
}