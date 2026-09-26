using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using PodRat.Models;
using PodRat.ViewModels;

namespace PodRat.Views;

public partial class FeedViewer : UserControl
{
    private FeedViewerViewModel? viewModel => DataContext as FeedViewerViewModel;

    public event EventHandler<PodcastFeedItem>? loadItem;
    public event EventHandler? backButtonPressed;


    public FeedViewer()
    {
        InitializeComponent();
        DataContextChanged += (sender, e) => InitializeDatacontext();
        FeedAdderFlyout.AddFeedSubmitted += OnFeedAdderSubmit;
        FeedEditorControl.EditFeedSubmitted += OnFeedEditorSubmit;
        PopupBackground.PointerPressed += OnPopupBackgroundPointerPressed;
        InitializeDatacontext();
    }

    private void InitializeDatacontext()
    {
        viewModel?.UpdateDownloadStatus += OnItemDownloadedChanged;
        viewModel?.ProgressChanged += UpdateProgress;
        viewModel?.SelectedFeedModified += SelectedFeedModified;
    }

    private void InteractItem_OnClick(object? sender, RoutedEventArgs e)
    {
        if (viewModel == null || e.Source == null)
            return;
        Button? button = e.Source as Button;
        if (button == null || button.Name == null)
            return;
        
        DownloadManager.DownloadStatus status = viewModel.ItemDownloadStatus(button.Name);
        if (status == DownloadManager.DownloadStatus.downloaded)
        {
            PodcastFeedItem? item = viewModel?.GetItem(button.Name);
            if (item != null)
            {
                loadItem?.Invoke(this, item);
            }
        }
        else if (status == DownloadManager.DownloadStatus.notDownloaded)
        {
            viewModel?.DownloadItem(button.Name);
        }
        else if (status == DownloadManager.DownloadStatus.currentlyDownloading)
        {
            viewModel?.CancelDownloadItem(button.Name);
        }
    }

    private void InteractItem_PointerEntered(object? sender, PointerEventArgs e)
    {
        if (viewModel == null || e.Source == null)
            return;
        Button? button = e.Source as Button;
        if (button == null || button.Name == null)
            return;

        PathIcon? icon = FindChildOfType<PathIcon>(button) as PathIcon;
        if (icon == null)
            return;
        
        DownloadManager.DownloadStatus status = viewModel.ItemDownloadStatus(button.Name);
        if (status == DownloadManager.DownloadStatus.currentlyDownloading)
        {
            icon.Bind(PathIcon.DataProperty, Resources.GetResourceObservable("dismiss_circle_regular"));
        }
    }

    private void InteractItem_PointerExited(object? sender, PointerEventArgs e)
    {
        if (viewModel == null || e.Source == null)
            return;
        Button? button = e.Source as Button;
        if (button == null || button.Name == null)
            return;

        PathIcon? icon = FindChildOfType<PathIcon>(button) as PathIcon;
        if (icon == null)
            return;
        
        DownloadManager.DownloadStatus status = viewModel.ItemDownloadStatus(button.Name);
        if (status == DownloadManager.DownloadStatus.currentlyDownloading)
        {
            icon.Bind(PathIcon.DataProperty, Resources.GetResourceObservable("arrow_download_regular"));
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

        // Switch progress bar back to download bar
        // Find button
        Button? interactButton = FindNamedChild(id, FeedList) as Button;
        if (interactButton == null)
            return;
        
        // Hide listen data progress bar
        ProgressBar? listenDataProgressBar = FindChildWithTag("ListenDataProgressBar", interactButton) as ProgressBar;
        if (listenDataProgressBar != null)
            listenDataProgressBar.IsVisible = false;
    }

    private void RemoveFeed_OnClick(object? sender, RoutedEventArgs e)
    {
        viewModel?.RemoveFeed();
    }

    private void EditFeed_OnClick(object? sender, RoutedEventArgs e)
    {
        if (viewModel?.SelectedFeed == null)
            return;
        FeedEditorControl.SetFeedToEdit(viewModel?.SelectedFeed);
        viewModel?.EditFeedPopupVisible = true;
    }

    private void OnFeedEditorSubmit(object? sender, EventArgs e)
    {
        viewModel?.EditFeedPopupVisible = false;
    }

    private void OnPopupBackgroundPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // Light dismiss does not work with current Avalonia version, so instead
        // dismiss when user presses grayed out background
        viewModel?.EditFeedPopupVisible = false;
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

    private void SelectedFeedModified(object? sender, EventArgs e)
    {
        FeedSelector.SelectedItem = viewModel?.SelectedFeed;
    }

    public void SetSelectedFeed(PodcastFeed feed)
    {
        FeedSelector.SelectedItem = feed;
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

    private void AddFeed_Closed(object? sender, EventArgs e)
    {
        FeedAdderFlyout.Reset();
    }

    private void BackButton_OnClick(object? sender, RoutedEventArgs e)
    {
        backButtonPressed?.Invoke(this, EventArgs.Empty);
    }

    void OnItemDownloadedChanged(string id, DownloadManager.DownloadStatus downloadStatus)
    {
        SetItemButtonDownloadState(id, downloadStatus);
    }

    void SetItemButtonDownloadState(string id, DownloadManager.DownloadStatus downloadStatus)
    {
        // Find button
        Button? interactButton = FindNamedChild(id, FeedList) as Button;
        if (interactButton == null)
            return;
        PathIcon? icon = FindChildOfType<PathIcon>(interactButton) as PathIcon;
        if (icon == null)
            return;
        
        // Set progress bar visibility
        ProgressBar? progressBar = FindChildWithTag("DownloadProgressBar", interactButton) as ProgressBar;
        if (progressBar != null)
        {
            progressBar.Value = 0;
            progressBar.IsVisible = downloadStatus == DownloadManager.DownloadStatus.notDownloaded;
        }

        // Show listen data progress bar
        ProgressBar? listenDataProgressBar = FindChildWithTag("ListenDataProgressBar", interactButton) as ProgressBar;
        if (listenDataProgressBar != null)
            listenDataProgressBar.IsVisible = downloadStatus == DownloadManager.DownloadStatus.downloaded;
        
        // Set to proper icon
        string iconString;
        if (downloadStatus == DownloadManager.DownloadStatus.downloaded)
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

    Avalonia.Visual? FindChildWithTag(string tag, Control parent)
    {
        Control? control;
        string? tagString;
        foreach(Avalonia.Visual child in parent.GetVisualDescendants())
        {
            control = child as Control;
            if (control != null)
            {
                tagString = control.Tag as string;
                if (tagString != null && tagString == tag)
                    return child;
            }        
        }
        return null;
    }
}