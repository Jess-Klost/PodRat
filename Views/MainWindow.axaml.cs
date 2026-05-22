using Avalonia.Controls;
using Avalonia.Interactivity;

namespace RSSPod.Views;

public partial class MainWindow : Window
{
    bool playing = false;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Play_OnClick(object? sender, RoutedEventArgs e)
    {
        if (playing)
        {
            PlayButtonIcon.Bind(PathIcon.DataProperty, Resources.GetResourceObservable("play_regular"));
            playing = false;
        }
        else
        {
            PlayButtonIcon.Bind(PathIcon.DataProperty, Resources.GetResourceObservable("pause_regular"));
            playing = true;
        }
    }
}