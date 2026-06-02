using System.IO;
using System.Text.Json;

public sealed class UserDataInstancer
{
    private static UserData? instance = null;
    const string UserDataFile = "userdata.json";

    public static UserData GetUserData()
    {
        if (instance == null) 
        {
            instance = new UserData();
        }

        return instance;
    }

    private UserDataInstancer() {}
    
    public static void LoadUserData(string jsonFile = UserDataFile)
    {
        try
        {
            using StreamReader fileReader = new (jsonFile);
            string jsonString = fileReader.ReadToEnd();
            instance = JsonSerializer.Deserialize<UserData>(jsonString);
        }
        catch (FileNotFoundException)
        {
            return;
        }
    }

    public static void SaveUserData(string jsonFile = UserDataFile)
    {
        if (instance == null)
            return;
        string jsonString = JsonSerializer.Serialize(instance);
        File.WriteAllText(jsonFile, jsonString);
    }

    public static void AddFeed(PodcastFeed feed)
    {
        instance?.PodcastFeeds.Add(feed);
    }
}