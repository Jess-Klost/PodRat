using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using RSSPod.ViewModels;

namespace RSSPod.Views;

public partial class MediaPlayer : UserControl
{
    private MediaPlayerViewModel? viewModel => DataContext as MediaPlayerViewModel;
    bool playing = false;
    bool editingPosition = false;

    public MediaPlayer()
    {
        InitializeComponent();
        PositionSlider.AddHandler(PointerPressedEvent, Position_Pressed,
            routes: RoutingStrategies.Direct
                    | RoutingStrategies.Tunnel
                    | RoutingStrategies.Bubble, handledEventsToo: false);
        PositionSlider.AddHandler(PointerReleasedEvent, Position_Released,
            routes: RoutingStrategies.Direct
                    | RoutingStrategies.Tunnel
                    | RoutingStrategies.Bubble, handledEventsToo: false);
    }

    public void LoadItem(PodcastFeedItem item)
    {
        viewModel?.LoadItem(item);
        playing = false;
        PlayButton.IsChecked = false;
    }

    private void Play_OnClick(object? sender, RoutedEventArgs e)
    {
        if (PlayButton.IsChecked == null)
        {
            return;
        }
        if (!(bool)PlayButton.IsChecked)
        {
            viewModel?.PauseAudio();
        }
        else
        {
            viewModel?.PlayAudio();
        }

        playing = (bool)PlayButton.IsChecked;
    }

    private void Volume_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        viewModel?.ChangeVolume((int)VolumeSlider.Value);
    }

    private void Position_Pressed(object? sender, PointerPressedEventArgs e)
    {
        editingPosition = true;
        if (playing)
            viewModel?.PauseAudio();
    }

    private void Position_Released(object? sender, PointerReleasedEventArgs e)
    {
        editingPosition = false;
        if (playing)
            viewModel?.PlayAudio();
    }

    private void Position_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (editingPosition)
            viewModel?.EditPosition((float)PositionSlider.Value);
    }

    private void Forward_OnClick(object? sender, RoutedEventArgs e)
    {
        viewModel?.FastForward(10000);
    }

    private void Backward_OnClick(object? sender, RoutedEventArgs e)
    {
        viewModel?.FastForward(-10000);
    }
}