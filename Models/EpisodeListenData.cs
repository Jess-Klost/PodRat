namespace RSSPod.Models;

public class EpisodeListenData(string episodeId, float position)
{
    public string EpisodeId { get; set; } = episodeId;
    public float Position { get; set;} = position;
}