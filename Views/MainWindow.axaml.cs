using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
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
        DataContextChanged += InitializeDatacontext;
        if (this.TryFindResource("play_regular", out object? playIcon) && playIcon != null)
        {
            IsDownloadedToIconConverter.playIcon = playIcon;
        }
        if (this.TryFindResource("arrow_download_regular", out object? downloadIcon) && downloadIcon != null)
        {
            IsDownloadedToIconConverter.downloadIcon = downloadIcon;
        }
    }

    private void InitializeDatacontext(object? sender, EventArgs e)
    {
        viewModel?.DownloadComplete += OnItemDownloaded;
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

    private void InteractItem_OnClick(object? sender, RoutedEventArgs e)
    {
        if (e.Source == null)
            return;
        Button? button = e.Source as Button;
        if (button == null || button.Name == null)
            return;
        if (viewModel != null && viewModel.ItemDownloaded(button.Name))
        {
            viewModel?.LoadItem(button.Name);
        }
        else
        {
            viewModel?.DownloadItem(button.Name);
        }
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

    void OnItemDownloaded(object? sender, string id)
    {
        SetItemButtonDownloadState(id, true);
    }

    void SetItemButtonDownloadState(string id, bool downloaded)
    {
        // Find button
        Button? interactButton = null;
        foreach(Avalonia.Visual child in FeedViewer.GetVisualDescendants())
        {
            if ((interactButton = child as Button) != null && interactButton.Name == id)
            {
                break;        
            }
        }
        if (interactButton == null)
            return;
        PathIcon? icon = null;
        foreach(Avalonia.Visual child in interactButton.GetVisualDescendants())
        {
            if ((icon = child as PathIcon) != null)
            {
                break;        
            }
        }
        if (icon == null)
            return;
        
        // Set to proper icon
        string iconString;
        if (downloaded)
            iconString = "play_regular";
        else
            iconString = "download_regular";
        icon.Bind(PathIcon.DataProperty, Resources.GetResourceObservable(iconString));
    }
}