using System;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PodBat.Models;

namespace PodBat.ViewModels;

public partial class FeedAdderViewModel : ObservableValidator
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddFeedCommand))]
    [Required(ErrorMessage="{0} is required.")]
    [DirectoryName]
    private string _name = "";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddFeedCommand))]
    [Display(Name="RSS Link")]
    [Required(ErrorMessage="{0} is required.")]
    [Url(ErrorMessage="{0} must be a valid URL.")]
    private string _uri = "";

    public event EventHandler? SuccessfulSubmit;

    [RelayCommand()]
    private void AddFeed()
    {
        ValidateAllProperties();

        if (HasErrors)
            return;

        PodcastFeed feed = new PodcastFeed(Name, Uri);
        UserDataInstancer.AddFeed(feed);
        UserDataInstancer.SaveUserData();
        SuccessfulSubmit?.Invoke(this, EventArgs.Empty);
    }
}