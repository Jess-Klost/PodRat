using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using RSSPod.ViewModels;

namespace RSSPod.Views;

public partial class FeedAdder : UserControl
{
    private FeedAdderViewModel? viewModel => DataContext as FeedAdderViewModel;

    public event EventHandler? AddFeedSubmitted;

    public FeedAdder()
    {
        InitializeComponent();
    }

    private void AddFeed_OnClick(object? sender, RoutedEventArgs e)
    {
        
        if (AddFeedNameBox.Text != null && AddFeedRSSBox.Text != null)
        {
            viewModel?.AddFeed(AddFeedNameBox.Text, AddFeedRSSBox.Text);
            AddFeedSubmitted?.Invoke(this, EventArgs.Empty);
        }
    }
    
    public void Reset()
    {
        AddFeedNameBox.Clear();
        AddFeedRSSBox.Clear();
    }
}