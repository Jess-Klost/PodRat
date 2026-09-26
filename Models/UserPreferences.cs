namespace PodRat.Models;

public class UserPreferences
{
    public bool AutoResume { get; set; } = true;
    public bool PlayOnSelect { get; set; } = true;
    public string Theme { get; set; } = "Default";
}