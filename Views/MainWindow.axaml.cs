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
    }
}