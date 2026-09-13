using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PodBat.Models;

public class FeedListenData
{
    [JsonInclude]
    Dictionary<string, EpisodeListenData> episodeListenData;

    public FeedListenData()
    {
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