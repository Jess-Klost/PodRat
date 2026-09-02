using CommunityToolkit.Mvvm.ComponentModel;
using RSSPod.Models;

namespace RSSPod.ViewModels;

public partial class FeedsOverviewViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial UserData UserData { get; set; }

    public FeedsOverviewViewModel()
    {
        UserData = UserDataInstancer.GetUserData();
        UserDataInstancer.UserDataChanged += OnUserDataChanged;
    }

    void OnUserDataChanged(object? sender, UserDataChangedEventArgs e)
    {
        UserData = UserDataInstancer.GetUserData();
    }
}