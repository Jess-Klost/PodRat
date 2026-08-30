using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace RSSPod.Models;

public class RSSFeedReader
{
    string uri;
    SyndicationFeed? feed;
    Dictionary<string, SyndicationItem>? itemsDict;

    public RSSFeedReader(string uri)
    {
        this.uri = uri;
    }

    public async Task<bool> ReadRSSFeed()
    {
        using (HttpClient client = new HttpClient())
        {
            Stream stream;
            try 
            {
                stream = await client.GetStreamAsync(uri);
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            using (XmlReader reader = XmlReader.Create(stream))
            {
                feed = SyndicationFeed.Load(reader);   
            }
        }
        itemsDict = new Dictionary<string, SyndicationItem>();
        foreach(SyndicationItem item in feed.Items)
        {
            itemsDict.Add(item.Id, item);
        }
        return true;
    }

    public ObservableCollection<PodcastFeedItem> GetFeedItems(PodcastFeed podcastFeed)
    {
        if (feed == null)
            return new ObservableCollection<PodcastFeedItem>();
        ObservableCollection<PodcastFeedItem> collection = new ObservableCollection<PodcastFeedItem>();
        foreach (SyndicationItem item in feed.Items)
        {
            collection.Add(new PodcastFeedItem { Feed = podcastFeed, Item = item });
        }
        return collection;
    }

    public bool GetItem(string id, out SyndicationItem? item)
    {
        if (itemsDict == null)
        {
            item = null;
            return false;
        }
        item = itemsDict[id];
        foreach (SyndicationElementExtension extension in item.ElementExtensions)
        {
            XElement element = extension.GetObject<XElement>();
        }
        return item != null;
    }

    public static bool GetImageFromItem(SyndicationItem item, out string imageLink)
    {
        foreach (SyndicationElementExtension extension in item.ElementExtensions)
        {
            XElement element = extension.GetObject<XElement>();
            if (element.FirstAttribute != null && element.FirstAttribute.Value != null)
            {
                if (element.Name.LocalName == "image")
                {
                    imageLink = element.FirstAttribute.Value;
                    return true;
                }
            }
            
        }
        imageLink = "";
        return false;
    }

    public static bool GetDurationFromItem(SyndicationItem item, out string durationString)
    {
        foreach (SyndicationElementExtension extension in item.ElementExtensions)
        {
            XElement element = extension.GetObject<XElement>();
            if (element != null && element.Value != null)
            {
                if (element.Name.LocalName == "duration")
                {
                    durationString = TimeSpan.FromSeconds(Convert.ToDouble(element.Value)).ToString(@"hh\:mm\:ss");
                    return true;
                }
            }
        }
        durationString = "";
        return false;
    }
}