using System;
using Avalonia.Remote.Protocol.Viewport;
using LibVLCSharp.Shared;

public class AudioPlayer
{
    private LibVLC _libVLC;
    public LibVLC LibVLC
    {
        get => _libVLC;
        private set => _libVLC = value;
    }
    public EventHandler<float> audioPositionChanged;
    public EventHandler<long> audioLengthChanged;
    public EventHandler playingStart;

    private MediaPlayer _mediaPlayer;
    private MediaPlayer MediaPlayer
    {
        get => _mediaPlayer;
        set => _mediaPlayer = value;
    }

    public AudioPlayer()
    {
        LibVLC = new LibVLC(enableDebugLogs: true);
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
        if (MediaPlayer == null)
        {
            return false;            
        }
        return true;
    }

    public void Play()
    {
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
        playingStart.Invoke(this, EventArgs.Empty);   
    }

    public long GetLengthTime()
    {
        return MediaPlayer.Length;
    }
}