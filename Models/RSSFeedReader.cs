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

    public RSSFeedReader(string uri)
    {
        this.uri = uri;
    }

    public async Task ReadRSSFeed()
    {
        using HttpClient client = new HttpClient();

        Stream stream = await client.GetStreamAsync(uri);

        using XmlReader reader = XmlReader.Create(stream);
        feed = SyndicationFeed.Load(reader);
    }

    public ObservableCollection<SyndicationItem> GetFeedItems()
    {
        if (feed == null)
            return new ObservableCollection<SyndicationItem>();
        ObservableCollection<SyndicationItem> collection = [.. feed.Items];
        return collection;
    }
}