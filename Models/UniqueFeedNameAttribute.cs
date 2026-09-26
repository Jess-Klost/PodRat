using System.ComponentModel.DataAnnotations;

namespace PodRat.Models;

public sealed class UniqueFeedNameAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        string? valueString = value as string; 
        UserData userData = UserDataInstancer.GetUserData();
        foreach (PodcastFeed feed in userData.PodcastFeeds) 
        {
            if (feed.Name == valueString)
            {
                return new(GetErrorMessage());
            }
        }
        return ValidationResult.Success;
    }

    string GetErrorMessage()
    {
        if (ErrorMessage == null)
            return "Feed name must be unique";
        else
            return ErrorMessage;
    }
}