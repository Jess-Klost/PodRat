using System;
using Avalonia.Controls;
using PodRat.Models;
using PodRat.ViewModels;

namespace PodRat.Views;

public partial class MainWindow : Window
{
    private MainWindowViewModel? viewModel => DataContext as MainWindowViewModel;

    public MainWindow()
    {
        InitializeComponent();
        if (this.TryFindResource("play_regular", out object? playIcon) && playIcon != null)
        {
            IsDownloadedToIconConverter.playIcon = playIcon;
        }
        if (this.TryFindResource("arrow_download_regular", out object? downloadIcon) && downloadIcon != null)
        {
            IsDownloadedToIconConverter.downloadIcon = downloadIcon;
        }
        FeedViewerControl.loadItem += (sender, item) => MediaPlayerControl.LoadItem(item);
        FeedViewerControl.backButtonPressed += FeedViewerControl_OnBackButtonPressed;
        FeedsOverviewControl.selectFeed += FeedOverviewControl_OnFeedSelected;
    }

    private void FeedOverviewControl_OnFeedSelected(object? sender, PodcastFeed feed)
    {
        FeedsOverviewControl.IsVisible = false;
        FeedViewerControl.IsVisible = true;
        FeedViewerControl.SetSelectedFeed(feed);
    }

    private void FeedViewerControl_OnBackButtonPressed(object? sender, EventArgs e)
    {
        FeedsOverviewControl.IsVisible = true;
        FeedViewerControl.IsVisible = false;
    }
}