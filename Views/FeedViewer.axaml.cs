using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using RSSPod.ViewModels;

namespace RSSPod.Views;

public partial class FeedViewer : UserControl
{
    private FeedViewerViewModel? viewModel => DataContext as FeedViewerViewModel;

    public event EventHandler<PodcastFeedItem> loadItem;

    public FeedViewer()
    {
        InitializeComponent();
        DataContextChanged += (sender, e) => InitializeDatacontext();
        FeedAdderFlyout.AddFeedSubmitted += OnFeedAdderSubmit;
        InitializeDatacontext();
    }

    private void InitializeDatacontext()
    {
        viewModel?.UpdateDownloadStatus += OnItemDownloadedChanged;
        viewModel?.ProgressChanged += UpdateProgress;
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
            PodcastFeedItem? item = viewModel?.GetItem(button.Name);
            if (item != null)
            {
                loadItem?.Invoke(this, item);
            }
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

    private void FeedList_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox listBox) 
        {
            foreach(object item in e.AddedItems)
            {
                if (item is PodcastFeedItem podcastFeedItem)
                {
                    if (FindNamedChild(podcastFeedItem.Item.Id + "ListItem", listBox) is Control listItem)
                    {
                        FlyoutBase.ShowAttachedFlyout(listItem);
                    } 
                }
            }
            listBox.UnselectAll();
        }
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
        Button? interactButton = FindNamedChild(id, FeedList) as Button;
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
        Button? interactButton = FindNamedChild(itemId, FeedList) as Button;
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