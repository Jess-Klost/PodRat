using System;
using LibVLCSharp.Shared;

namespace PodRat.Models;

public class AudioPlayer
{
    private LibVLC _libVLC;
    public LibVLC LibVLC
    {
        get => _libVLC;
        private set => _libVLC = value;
    }
    public event EventHandler<float> audioPositionChanged;
    public event EventHandler<long> audioLengthChanged;
    public event EventHandler playingStart;

    private MediaPlayer _mediaPlayer;
    private MediaPlayer MediaPlayer
    {
        get => _mediaPlayer;
        set => _mediaPlayer = value;
    }
    private bool unplayed = false;

    public AudioPlayer()
    {
        #if OS_WINDOWS
        LibVLC = new LibVLC(enableDebugLogs: true,  "--audio-resampler=speex_resampler");
        #else
        LibVLC = new LibVLC(enableDebugLogs: true);
        #endif
        MediaPlayer = new MediaPlayer(LibVLC);
        MediaPlayer.PositionChanged += OnPositionChanged;
        MediaPlayer.Playing += PlayingStart;
    }

    public bool LoadAudio(string path)
    {
        Uri uri;
        try
        {
            uri = new Uri(path);
        }
        catch (UriFormatException)
        {
            return false;
        }
        Media media = new Media(LibVLC, uri);
        MediaPlayer.Media = media;
        media.Dispose();
        unplayed = true;
        if (MediaPlayer == null)
        {
            return false;            
        }
        return true;
    }

    public void Play()
    {
        // Auto-replay if ended
        if (MediaPlayer.Media?.State == VLCState.Ended)
        {
            float currentPosition = MediaPlayer.Position;
            MediaPlayer.Play(MediaPlayer.Media);
            MediaPlayer.Position = currentPosition;
        }
        MediaPlayer.Play();        
    }

    public void Pause()
    {
        MediaPlayer.Pause();
    }
    
    public bool IsPlaying()
    {
        return MediaPlayer.IsPlaying;
    }

    public void SetVolume(int volume)
    {
        MediaPlayer.Volume = volume;
    }

    public float GetPosition()
    {
        return MediaPlayer.Position;
    }

    public void SetPosition(float position)
    {
        MediaPlayer.Position = position;
    }

    void OnPositionChanged(object? sender, MediaPlayerPositionChangedEventArgs eventArgs)
    {
        audioPositionChanged?.Invoke(this, eventArgs.Position); 
    }

    void PlayingStart(object? sender, EventArgs e)
    {
        if (unplayed)
        {
            unplayed = false;
            playingStart.Invoke(this, EventArgs.Empty);
        }
    }

    public long GetLengthTime()
    {
        return MediaPlayer.Length;
    }
}