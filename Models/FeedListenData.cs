using System.Collections.Generic;

namespace RSSPod.Models;

public class FeedListenData
{
    PodcastFeed feed;
    Dictionary<string, EpisodeListenData> episodeListenData;

    public FeedListenData(PodcastFeed feed)
    {
        this.feed = feed;
        episodeListenData = new Dictionary<string, EpisodeListenData>();
    }

    public void UpdateListenData(string episodeId, float position)
    {
        if (episodeListenData.ContainsKey(episodeId))
            episodeListenData[episodeId].Position = position;
        else
            episodeListenData.Add(episodeId, new EpisodeListenData(episodeId, position));
    }

    public float GetEpisodeListenData(string episodeId)
    {
        if (episodeListenData.ContainsKey(episodeId))
            return episodeListenData[episodeId].Position;
        return 0;
    }
}