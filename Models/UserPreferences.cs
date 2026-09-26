namespace PodRat.Models;

public class UserPreferences
{
    public bool AutoResume { get; set; } = true;
    public bool PlayOnSelect { get; set; } = true;
    public string Theme { get; set; } = "Default";
    public int Volume { get; set; } = 50;
}