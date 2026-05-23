using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using RSSPod.ViewModels;

namespace RSSPod.Views;

public partial class MainWindow : Window
{
    private MainWindowViewModel? viewModel => DataContext as MainWindowViewModel;
    bool playing = false;
    bool editingPosition = false;

    public MainWindow()
    {
        InitializeComponent();
        viewModel?.ChangeVolume((int)VolumeSlider.Value);
        PositionSlider.AddHandler(PointerPressedEvent, Position_Pressed, 
            routes: RoutingStrategies.Direct 
                    | RoutingStrategies.Tunnel 
                    | RoutingStrategies.Bubble, handledEventsToo: false);
        PositionSlider.AddHandler(PointerReleasedEvent, Position_Released, 
            routes: RoutingStrategies.Direct 
                    | RoutingStrategies.Tunnel 
                    | RoutingStrategies.Bubble, handledEventsToo: false);
    }

    private void Play_OnClick(object? sender, RoutedEventArgs e)
    {
        if (playing)
        {
            PlayButtonIcon.Bind(PathIcon.DataProperty, Resources.GetResourceObservable("play_regular"));
            playing = false;
            viewModel?.PauseAudio();
        }
        else
        {
            PlayButtonIcon.Bind(PathIcon.DataProperty, Resources.GetResourceObservable("pause_regular"));
            playing = true;
            viewModel?.PlayAudio();
        }
    }

    private void Volume_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        viewModel?.ChangeVolume((int)VolumeSlider.Value);
    }

    private void Position_Pressed(object? sender, PointerPressedEventArgs e)
    {
        editingPosition = true;
        if (playing)
            viewModel?.PauseAudio();
    }

    private void Position_Released(object? sender, PointerReleasedEventArgs e)
    {
        editingPosition = false;
        if (playing)
            viewModel?.PlayAudio();
    }

    private void Position_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (editingPosition)
            viewModel?.EditPosition((float)PositionSlider.Value);
    }
}