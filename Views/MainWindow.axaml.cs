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
        FeedAdderFlyout.AddFeedSubmitted += OnFeedAdderSubmit;
    }

    private void InitializeDatacontext(object? sender, EventArgs e)
    {
        viewModel?.UpdateDownloadStatus += OnItemDownloadedChanged;
        viewModel?.ProgressChanged += UpdateProgress;
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
            PlayButton.IsChecked = false;
            playing = false;
        }
        else
        {
            viewModel?.DownloadItem(button.Name);
        }
    }
    
    private void DeleteItem_OnClick(object? sender, RoutedEventArgs e)
    {
        if (e.Source == null)
            return;
        Button? button = e.Source as Button;
        string? id;
        if (button == null || button.Tag == null || (id = button.Tag as string) == null)
            return;
        viewModel?.DeleteItem(id);
    }

    private void RemoveFeed_OnClick(object? sender, RoutedEventArgs e)
    {
        viewModel?.RemoveFeed();
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

    private void OnFeedAdderSubmit(object? sender, EventArgs e)
    {
        AddFeedPopupButton?.Flyout?.Hide();
    }

    private void AddFeed_Closed(object? sender, System.EventArgs e)
    {
        FeedAdderFlyout.Reset();
    }

    void OnItemDownloadedChanged(string id, bool downloaded)
    {
        SetItemButtonDownloadState(id, downloaded);
    }

    void SetItemButtonDownloadState(string id, bool downloaded)
    {
        // Find button
        Button? interactButton = FindNamedChild(id, FeedViewer) as Button;
        if (interactButton == null)
            return;
        PathIcon? icon = FindChildOfType<PathIcon>(interactButton) as PathIcon;
        if (icon == null)
            return;
        
        // Set progress bar visibility
        ProgressBar? progressBar = FindChildOfType<ProgressBar>(interactButton) as ProgressBar;
        if (progressBar != null)
        {
            progressBar.Value = 0;
            progressBar.IsVisible = !downloaded;
        }

        
        // Set to proper icon
        string iconString;
        if (downloaded)
        {
            iconString = "play_regular";
        }
        else
            iconString = "arrow_download_regular";
        icon.Bind(PathIcon.DataProperty, Resources.GetResourceObservable(iconString));
    }

    void UpdateProgress(string itemId, float progress)
    {
        Button? interactButton = FindNamedChild(itemId, FeedViewer) as Button;
        if (interactButton == null)
            return;
        ProgressBar? progressBar = FindChildOfType<ProgressBar>(interactButton) as ProgressBar;
        if (progressBar == null)
            return;
        progressBar.Value = progress;
    }

    Avalonia.Visual? FindNamedChild(string name, Control parent)
    {
        foreach(Avalonia.Visual child in parent.GetVisualDescendants())
        {
            if (child.Name == name)
            {
                return child;
            }        
        }
        return null;
    }

    Avalonia.Visual? FindChildOfType<T>(Control parent)
    {
        foreach(Avalonia.Visual child in parent.GetVisualDescendants())
        {
            if (child is T)
            {
                return child;
            }        
        }
        return null;
    }
}