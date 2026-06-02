namespace RSSPod.ViewModels;

public partial class FeedAdderViewModel : ViewModelBase
{
    public void AddFeed(string name, string uri)
    {
        PodcastFeed feed = new PodcastFeed(name, uri);
        UserDataInstancer.AddFeed(feed);
        UserDataInstancer.SaveUserData();
    }
}