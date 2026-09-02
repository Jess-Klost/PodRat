using System;
using System.Collections.Generic;
using Avalonia.Controls;
using RSSPod.Models;
using RSSPod.ViewModels;

namespace RSSPod.Views;

public partial class FeedsOverview : UserControl
{
    private FeedsOverviewViewModel? viewModel => DataContext as FeedsOverviewViewModel;
    
    public event EventHandler<PodcastFeed> selectFeed;

    public FeedsOverview()
    {
        InitializeComponent();
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
}