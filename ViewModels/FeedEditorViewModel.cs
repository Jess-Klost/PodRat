using System;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PodRat.Models;

namespace PodRat.ViewModels;

public partial class FeedEditorViewModel : ObservableValidator
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EditFeedCommand))]
    [Required(ErrorMessage="{0} is required.")]
    [DirectoryName]
    [UniqueFeedName(ErrorMessage="A feed with this name already exists.")]
    private string _name = "";
    
    [ObservableProperty]
    public partial PodcastFeed? FeedToEdit { get; set; }

    public event EventHandler? SuccessfulSubmit;

    [RelayCommand()]
    private void EditFeed()
    {
        if (FeedToEdit == null)
            return;
        
        ValidateAllProperties();

        if (HasErrors)
            return;

        DownloadManager.RenameFeed(FeedToEdit, Name);
        UserDataInstancer.RenameFeed(FeedToEdit, Name);
        UserDataInstancer.SaveUserData();
        SuccessfulSubmit?.Invoke(this, EventArgs.Empty);

        // Reset properties
        FeedToEdit = null;
        Name = "";
    }
}