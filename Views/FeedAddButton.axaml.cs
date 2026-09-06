using System;
using Avalonia.Controls;
using RSSPod.ViewModels;

namespace RSSPod.Views;

public partial class FeedAddButton : UserControl
{
    private FeedAddButtonViewModel? viewModel => DataContext as FeedAddButtonViewModel;

    public FeedAddButton()
    {
        InitializeComponent();
        FeedAdderFlyout.AddFeedSubmitted += OnFeedAdderSubmit;
    }

    private void OnFeedAdderSubmit(object? sender, EventArgs e)
    {
        AddFeedPopupButton?.Flyout?.Hide();
    }

    private void AddFeed_Closed(object? sender, EventArgs e)
    {
        FeedAdderFlyout.Reset();
    }
}