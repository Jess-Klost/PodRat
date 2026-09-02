using System;
using Avalonia;
using Avalonia.Controls;
using RSSPod.Models;
using RSSPod.ViewModels;

namespace RSSPod.Views;

public partial class FeedEditor : UserControl
{
    private FeedEditorViewModel? viewModel => DataContext as FeedEditorViewModel;
    
    public event EventHandler EditFeedSubmitted;

    public FeedEditor()
    {
        InitializeComponent();
        viewModel?.SuccessfulSubmit += (sender, e) => EditFeedSubmitted?.Invoke(sender, e);
    }

    public void SetFeedToEdit(PodcastFeed? feed)
    {
        viewModel?.FeedToEdit = feed;
    }
}