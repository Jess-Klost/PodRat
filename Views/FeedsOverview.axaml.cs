using System;
using Avalonia.Interactivity;
using Avalonia.Controls;
using RSSPod.Models;
using RSSPod.ViewModels;
using Avalonia.Input;

namespace RSSPod.Views;

public partial class FeedsOverview : UserControl
{
    private FeedsOverviewViewModel? viewModel => DataContext as FeedsOverviewViewModel;

    public event EventHandler<PodcastFeed> selectFeed;

    public FeedsOverview()
    {
        InitializeComponent();
        FeedsGrid.AddHandler(PointerPressedEvent, FeedsGrid_PointerPressed,
            RoutingStrategies.Tunnel);
    }

    private void FeedsGrid_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // Ignore right clicks, so that opening the context menu does not select
        // the feed.
        if (e.Properties.IsRightButtonPressed)
            e.Handled = true;
    }

    private void FeedsGrid_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count == 1 && e.AddedItems[0] is PodcastFeed selectedFeed)
        {
            selectFeed.Invoke(this, selectedFeed);
        }

        if (sender is ListBox listBox)
            listBox.UnselectAll();
    }

    private void DeleteFeed_Click(object? sender, RoutedEventArgs e)
    {
        if (e.Source is Control control)
        {
            if (control.Tag is PodcastFeed feed)
            {
                // TODO: Set up delete feed
            }
        }
    }

    private void EditFeed_Click(object? sender, RoutedEventArgs e)
    {
        if (e.Source is Control control)
        {
            if (control.Tag is PodcastFeed feed)
            {
                // TODO: Set up edit feed
            }
        }
    }
}