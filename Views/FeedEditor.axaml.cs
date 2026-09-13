using System;
using Avalonia;
using Avalonia.Controls;
using PodBat.Models;
using PodBat.ViewModels;

namespace PodBat.Views;

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