using RSSPod.Models;

namespace RSSPod.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
        UserDataInstancer.LoadUserData();
    }
}
