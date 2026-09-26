using PodRat.ViewModels;
using Avalonia.Controls;
using PodRat.Models;

namespace PodRat.Views;

public partial class MenuBar : UserControl
{
    private MenuBarViewModel? viewModel => DataContext as MenuBarViewModel;

    public MenuBar()
    {
        InitializeComponent();
        DataContextChanged += (sender, e) => InitializeDatacontext();
        InitializeDatacontext();
    }

    private void InitializeDatacontext()
    {
        if (viewModel != null)
        {
            AutoResumeButton.IsChecked = viewModel.AutoResume;
            PlayOnSelectButton.IsChecked = viewModel.PlayOnSelect;
            ThemeLightButton.IsChecked = false;
            ThemeDarkButton.IsChecked = false;
            ThemeDefaultButton.IsChecked = false;
            switch (viewModel.Theme)
            {
                case "Light":
                    ThemeLightButton.IsChecked = true;
                    break;
                case "Dark":
                    ThemeDarkButton.IsChecked = true;
                    break;
                case "Default":
                    ThemeDefaultButton.IsChecked = true;
                    break;
            }
        }
    }

    private void AutoResumeButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        viewModel?.AutoResume = AutoResumeButton.IsChecked;
    }

    private void PlayOnSelectButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        viewModel?.PlayOnSelect = PlayOnSelectButton.IsChecked;
    }

    private void ThemeDarkButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        viewModel?.Theme = "Dark";
    }

    private void ThemeLightButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        viewModel?.Theme = "Light";
    }

    private void ThemeDefaultButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        viewModel?.Theme = "Default";
    }
}