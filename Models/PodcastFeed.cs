using System;

namespace RSSPod.Models;

public class PodcastFeed
{
    public string Name { get; set; }
    public string Uri { get; set; }

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
}