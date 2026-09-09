using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RSSPod.Models;

public class PodcastFeed
{
    public string Name { get; set; }
    public string Uri { get; set; }

    [JsonIgnore]
    public Task<Uri?> Thumbnail {
        get
        {
            return GetFeedThumbnailUri();
        } 
    }

    public PodcastFeed(string name, string uri)
    {
        if (!ValidName(name))
            throw new ArgumentException("Name is invalid");
        Name = name;
        Uri = uri;
    }

    public static bool ValidName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return false;
        }
        return true;
    }

    async Task<Uri?> GetFeedThumbnailUri()
    {
        // TODO: optimize this, so that we are not reading the same feed 
        // multiple times for no reason  
        RSSFeedReader reader = new RSSFeedReader(Uri);
        if (!await reader.ReadRSSFeed())
            return null;
        return reader.GetImageUri();
    }
}