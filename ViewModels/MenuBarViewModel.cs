using Avalonia;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using PodRat.Models;

namespace PodRat.ViewModels;

public partial class MenuBarViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool _playOnSelect;
    [ObservableProperty]
    private bool _autoResume;
    [ObservableProperty]
    private string _theme;

    public MenuBarViewModel()
    {
        UserDataInstancer.LoadUserPreferences();
        SetTheme(UserDataInstancer.UserPreferencesInstance.Theme);
        PlayOnSelect = UserDataInstancer.UserPreferencesInstance.PlayOnSelect;
        AutoResume = UserDataInstancer.UserPreferencesInstance.AutoResume;
        Theme = UserDataInstancer.UserPreferencesInstance.Theme;
    }

    void SetTheme(string theme)
    {
        if (Application.Current is { } app)
        {
            app.RequestedThemeVariant = theme switch
            {
                "Light" => ThemeVariant.Light,
                "Dark" => ThemeVariant.Dark,
                _ => ThemeVariant.Default
            };
        }
    }

    partial void OnPlayOnSelectChanged(bool value)
    {
        UserDataInstancer.UserPreferencesInstance.PlayOnSelect = value;
    }

    partial void OnAutoResumeChanged(bool value)
    {
        UserDataInstancer.UserPreferencesInstance.AutoResume = value;
    }

    partial void OnThemeChanged(string value)
    {
        SetTheme(value);
        UserDataInstancer.UserPreferencesInstance.Theme = value;
    }
}