using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using RSSPod.ViewModels;

namespace RSSPod.Views;

public partial class MainWindow : Window
{
    private MainWindowViewModel? viewModel => DataContext as MainWindowViewModel;
    bool playing = false;
    //AudioPlayer audioPlayer;  

    public MainWindow()
    {
        InitializeComponent();
        viewModel?.OnVolumeChanged((int)VolumeSlider.Value);
    }

    private void Play_OnClick(object? sender, RoutedEventArgs e)
    {
        if (playing)
        {
            PlayButtonIcon.Bind(PathIcon.DataProperty, Resources.GetResourceObservable("play_regular"));
            playing = false;
            viewModel?.OnPauseButtonClicked();
        }
        else
        {
            PlayButtonIcon.Bind(PathIcon.DataProperty, Resources.GetResourceObservable("pause_regular"));
            playing = true;
            viewModel?.OnPlayButtonClicked();
        }
    }

    private void Volume_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        viewModel?.OnVolumeChanged((int)VolumeSlider.Value);
    }
}