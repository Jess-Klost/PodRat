using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;

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
        using HttpClient client = new HttpClient();
        Stream stream;
        try 
        {
            stream = await client.GetStreamAsync(uri);
        }
        catch (InvalidOperationException)
        {
            return false;
        }
        using XmlReader reader = XmlReader.Create(stream);
        feed = SyndicationFeed.Load(reader);
        itemsDict = new Dictionary<string, SyndicationItem>();
        foreach(SyndicationItem item in feed.Items)
        {
            itemsDict.Add(item.Id, item);
        }
        return true;
    }

    public ObservableCollection<SyndicationItem> GetFeedItems()
    {
        if (feed == null)
            return new ObservableCollection<SyndicationItem>();
        ObservableCollection<SyndicationItem> collection = [.. feed.Items];
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
        return item != null;
    }
}