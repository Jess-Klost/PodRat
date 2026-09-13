using System;
using Avalonia.Controls;
using PodRat.ViewModels;

namespace PodRat.Views;

public partial class FeedAdder : UserControl
{
    private FeedAdderViewModel? viewModel => DataContext as FeedAdderViewModel;

    public event EventHandler? AddFeedSubmitted;

    public FeedAdder()
    {
        InitializeComponent();
        viewModel?.SuccessfulSubmit += (sender, e) => AddFeedSubmitted?.Invoke(sender, e);
    }

    public void Reset()
    {
        AddFeedNameBox.Clear();
        AddFeedRSSBox.Clear();
    }
}