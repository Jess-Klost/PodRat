using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using RSSPod.ViewModels;

namespace RSSPod.Views;

public partial class MainWindow : Window
{
    private MainWindowViewModel? viewModel => DataContext as MainWindowViewModel;
    bool playing = false;
    bool editingPosition = false;

    public MainWindow()
    {
        InitializeComponent();
        viewModel?.ChangeVolume((int)VolumeSlider.Value);
        PositionSlider.AddHandler(PointerPressedEvent, Position_Pressed,
            routes: RoutingStrategies.Direct
                    | RoutingStrategies.Tunnel
                    | RoutingStrategies.Bubble, handledEventsToo: false);
        PositionSlider.AddHandler(PointerReleasedEvent, Position_Released,
            routes: RoutingStrategies.Direct
                    | RoutingStrategies.Tunnel
                    | RoutingStrategies.Bubble, handledEventsToo: false);
    }

    private void Play_OnClick(object? sender, RoutedEventArgs e)
    {
        if (playing)
        {
            PlayButtonIcon.Bind(PathIcon.DataProperty, Resources.GetResourceObservable("play_regular"));
            playing = false;
            viewModel?.PauseAudio();
        }
        else
        {
            PlayButtonIcon.Bind(PathIcon.DataProperty, Resources.GetResourceObservable("pause_regular"));
            playing = true;
            viewModel?.PlayAudio();
        }
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

    private void Download_OnClick(object? sender, RoutedEventArgs e)
    {
        if (e.Source == null)
            return;
        Button? button = e.Source as Button;
        if (button == null || button.Name == null)
            return;
        viewModel?.DownloadItem(button.Name);
    }

    private void FeedSelector_OnChange(object? sender, SelectionChangedEventArgs e)
    {
        if (e.Source == null)
            return;
        ComboBox? selector = e.Source as ComboBox;
        if (selector == null || selector.SelectedItem == null)
            return;
        PodcastFeed? feed = selector.SelectedItem as PodcastFeed;
        if (feed == null)
            return;
        viewModel?.OnFeedChanged(feed);
    }

    private void AddFeed_OnClick(object? sender, RoutedEventArgs e)
    {
        if (AddFeedNameBox.Text != null && AddFeedRSSBox.Text != null)
        {
            viewModel?.AddFeed(AddFeedNameBox.Text, AddFeedRSSBox.Text);
            AddFeedPopupButton?.Flyout?.Hide();
        }
    }

    private void AddFeed_Closed(object? sender, System.EventArgs e)
    {
        // Clear text inputs when flyout is closed
        AddFeedNameBox.Clear();
        AddFeedRSSBox.Clear();
    }
}