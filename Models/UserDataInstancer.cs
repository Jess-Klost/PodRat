using System;
using System.IO;
using System.Text.Json;

namespace PodRat.Models;

public class UserDataChangedEventArgs
{
    public static readonly UserDataChangedEventArgs Empty = new UserDataChangedEventArgs();

    public enum ChangeType
    {
        RenameFeed
    }

    public ChangeType? changeType = null;
    public PodcastFeed? affectedFeed = null;
}

public static class UserDataInstancer
{
    private static UserData? instance = null;
    static readonly string UserDataFile =
    #if OS_WINDOWS
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PodRat/userdata.json");
    #elif OS_LINUX
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".podrat/userdata.json");
    #else
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PodRat/userdata.json");
    #endif
    static readonly string UserPreferencesFile = 
    #if OS_WINDOWS
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PodRat/userpreferences.json");
    #elif OS_LINUX
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".podrat/userpreferences.json");
    #else
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PodRat/userdata.json");
    #endif

    public static event EventHandler<object?, UserDataChangedEventArgs>? UserDataChanged;

    public static DownloadManager DownloadManagerInstance { get; private set; } = new DownloadManager();
    public static UserPreferences UserPreferencesInstance { get; private set; } = new UserPreferences();

    public static UserData GetUserData()
    {
        if (instance == null) 
        {
            instance = new UserData();
        }

        return instance;
    }
    
    public static void LoadUserData(string jsonFile = "")
    {
        if (jsonFile == "")
            jsonFile = UserDataFile;
        try
        {
            using StreamReader fileReader = new (jsonFile);
            string jsonString = fileReader.ReadToEnd();
            instance = JsonSerializer.Deserialize<UserData>(jsonString);
            UserDataChanged?.Invoke(null, UserDataChangedEventArgs.Empty);
        }
        catch (FileNotFoundException)
        {
            return;
        }
    }

    public static void LoadUserPreferences(string jsonFile = "")
    {
        if (jsonFile == "")
            jsonFile = UserPreferencesFile;
        try
        {
            using StreamReader fileReader = new (jsonFile);
            string jsonString = fileReader.ReadToEnd();
            UserPreferencesInstance = JsonSerializer.Deserialize<UserPreferences>(jsonString);
        }
        catch (FileNotFoundException)
        {
            return;
        }
    }

    public static void SaveUserData(string jsonFile = "")
    {
        if (instance == null)
            return;
        if (jsonFile == "")
            jsonFile = UserDataFile;
        string jsonString = JsonSerializer.Serialize(instance);
        File.WriteAllText(jsonFile, jsonString);
    }

    public static void SaveUserPreferences(string jsonFile = "")
    {
        if (instance == null)
            return;
        if (jsonFile == "")
            jsonFile = UserPreferencesFile;
        string jsonString = JsonSerializer.Serialize(UserPreferencesInstance);
        File.WriteAllText(jsonFile, jsonString);
    }

    public static void AddFeed(PodcastFeed feed)
    {
        instance?.PodcastFeeds.Add(feed);
        UserDataChanged?.Invoke(null, UserDataChangedEventArgs.Empty);
    }

    public static void RemoveFeed(PodcastFeed feed, bool saveAfterOperation = true)
    {
        instance?.PodcastFeeds.Remove(feed);
        DownloadManagerInstance.DeleteFeed(feed);
        UserDataChanged?.Invoke(null, UserDataChangedEventArgs.Empty);
        if (saveAfterOperation)
            SaveUserData();
    }

    public static void RenameFeed(PodcastFeed feed, string newName)
    {
        if (instance == null)
            return;

        int feedIndex = instance.PodcastFeeds.IndexOf(feed);
        if (feedIndex < 0)
            return;

        feed.Name = newName;
        instance?.PodcastFeeds[feedIndex] = feed;
        UserDataChanged?.Invoke(null, new UserDataChangedEventArgs { 
            changeType = UserDataChangedEventArgs.ChangeType.RenameFeed,
            affectedFeed = feed });
    }
}