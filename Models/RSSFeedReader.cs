using System.IO;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Xml;

public class RSSFeedReader
{
    string uri;
    SyndicationFeed? feed;

    public RSSFeedReader(string uri)
    {
        this.uri = uri;
    }

    public async void ReadRSSFeed()
    {
        using HttpClient client = new HttpClient();

        Stream stream = await client.GetStreamAsync(uri);

        using XmlReader reader = XmlReader.Create(stream);
        feed = SyndicationFeed.Load(reader);
    }
}