using System;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RSSPod.Models;

namespace RSSPod.ViewModels;

public partial class FeedAdderViewModel : ObservableValidator
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddFeedCommand))]
    [Required]
    [DirectoryName]
    private string _name = "";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddFeedCommand))]
    [Required]
    [Url]
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