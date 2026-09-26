using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using PodRat.Models;
using PodRat.ViewModels;
using PodRat.Views;

namespace PodRat;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
            desktop.Exit += OnExit;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        // Call OnExit for all media player instances, so listen data is saved 
        // for any currently loaded episode 
        foreach (MediaPlayerViewModel mediaPlayer in MediaPlayerViewModel.Instances)
            mediaPlayer.OnExit();

        foreach (FeedViewerViewModel feedViewer in FeedViewerViewModel.Instances)
            feedViewer.OnExit();
        
        // Save listen data for all feeds to file on exit 
        foreach (PodcastFeed feed in UserDataInstancer.GetUserData().PodcastFeeds)
            ListenDataManager.SaveListenData(feed);

        UserDataInstancer.SaveUserPreferences();
    }
}