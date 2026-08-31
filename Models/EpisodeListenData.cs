using System.Text.Json.Serialization;

namespace RSSPod.Models;

public class EpisodeListenData(string episodeId, float position)
{
    [JsonInclude]
    public string EpisodeId { get; set; } = episodeId;
    [JsonInclude]
    public float Position { get; set;} = position;
}