using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace RSSPod.Models;

public sealed class ListenDataManager
{
    const string ListenDataFile = "listendata.json";

    private static ListenDataManager? instance = new ListenDataManager();

    Dictionary<PodcastFeed, FeedListenData> feedListenData = new Dictionary<PodcastFeed, FeedListenData>();

    public static void LoadListenData(PodcastFeed feed)
    {
        if (instance == null)
            return;
        try
        {
            using StreamReader fileReader = new (GetFeedListenDataPath(feed));
            string jsonString = fileReader.ReadToEnd();
            FeedListenData? listenData = JsonSerializer.Deserialize<FeedListenData>(jsonString);
            if (listenData != null)
                instance.feedListenData.Add(feed, listenData);
        }
        catch (FileNotFoundException)
        {
            return;
        }
    }

    public static void SaveListenData(PodcastFeed feed)
    {
        if (instance == null)
            return;
        string jsonString = JsonSerializer.Serialize(instance.feedListenData[feed]);
        File.WriteAllText(GetFeedListenDataPath(feed), jsonString);
    }

    public static FeedListenData? GetListenData(PodcastFeed feed)
    {
        if (instance == null)
            return null;
        if (instance.feedListenData.ContainsKey(feed))
            return instance.feedListenData[feed];
        return null;
    }

    public static float GetListenDataPosition(PodcastFeed feed, string episodeId)
    {
        if (instance == null)
            return 0;
        if (instance.feedListenData.ContainsKey(feed))
            return instance.feedListenData[feed].GetEpisodeListenData(episodeId);
        return 0;
    }

    public static void UpdateListenData(PodcastFeed feed, FeedListenData newListenData)
    {
        if (instance == null)
            return;
        if (instance.feedListenData.ContainsKey(feed))
            instance.feedListenData[feed] = newListenData;
        else    
            instance.feedListenData.Add(feed, newListenData);
    }

    public static void UpdateListenData(PodcastFeed feed, string episodeId, float position)
    {
        if (instance == null)
            return;
        if (instance.feedListenData.ContainsKey(feed))
            instance.feedListenData[feed].UpdateListenData(episodeId, position);
        else
        {
            FeedListenData feedListenData = new FeedListenData(feed);
            feedListenData.UpdateListenData(episodeId, position);
            instance.feedListenData.Add(feed, feedListenData);
        }
    }

    static string GetFeedListenDataPath(PodcastFeed feed)
    {
        return Path.GetFullPath(Path.Combine(DownloadManager.FeedDownloadPath(feed), ListenDataFile));
    }
}