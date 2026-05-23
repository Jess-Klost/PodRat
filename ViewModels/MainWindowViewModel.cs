using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RSSPod.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string Greeting { get; } = "Welcome to Avalonia!";
    [ObservableProperty]
    public partial double Position { get; set; } = 0;
    public double Volume { get; set; } = 50; 

    AudioPlayer audioPlayer;  


    public MainWindowViewModel()
    {
        audioPlayer = new AudioPlayer();
        audioPlayer.audioPositionChanged += AudioPositionChanged;
        audioPlayer.SetVolume((int)Volume);
        audioPlayer.LoadAudio("/home/aklost/Documents/sound-effects/Sonniss.com-GDC2024-GameAudioBundle2of9/InMotionAudio - Submerge/WATRFlow_WaterFlow21_InMotionAudio_Submerge.wav");
    }

    void AudioPositionChanged(object? sender, float position)
    {
        Position = position;
    }

    public void OnPlayButtonClicked()
    {
        audioPlayer.Play();
    }

    public void OnPauseButtonClicked()
    {
        audioPlayer.Pause();
    }

    public void OnVolumeChanged(int volume)
    {
        audioPlayer.SetVolume(volume);
    }
}
