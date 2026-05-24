using System.Collections.ObjectModel;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RSSPod.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    const string DownloadDirectory = "AudioDownloads";

    [ObservableProperty]
    public partial double Position { get; set; } = 0;
    [ObservableProperty]
    public partial double Volume { get; set; } = 50;
    [ObservableProperty]
    public partial string Title { get; set; } = "Audio Title";
    [ObservableProperty]
    public partial ObservableCollection<SyndicationItem>? CurrentFeed { get; set; }
    [ObservableProperty]
    public partial ObservableCollection<PodcastFeed>? PodcastFeeds { get; set; } = new ObservableCollection<PodcastFeed> { new PodcastFeed { Name = "RTVS", Uri = "https://feed.podbean.com/wayneradiotv/feed.xml"}};

    AudioPlayer audioPlayer;
    RSSFeedReader feedReader;
    DownloadManager downloadManager = new DownloadManager(DownloadDirectory);
    PodcastFeed? selectedFeed;

    public MainWindowViewModel()
    {
        audioPlayer = new AudioPlayer();
        audioPlayer.audioPositionChanged += AudioPositionChanged;
        audioPlayer.SetVolume((int)Volume);
    }

    async void PopulateFeed()
    {
        if (selectedFeed == null)
            return;
        feedReader = new RSSFeedReader(selectedFeed.Uri);
        await feedReader.ReadRSSFeed();
        CurrentFeed = feedReader.GetFeedItems();
    }

    void AudioPositionChanged(object? sender, float position)
    {
        Position = position;
    }

    public void PlayAudio()
    {
        audioPlayer.Play();
    }

    public void PauseAudio()
    {
        audioPlayer.Pause();
    }

    public void ChangeVolume(int volume)
    {
        audioPlayer.SetVolume(volume);
    }

    public void EditPosition(float position)
    {
        audioPlayer.SetPosition(position);
    }

    public void FastForward(float time)
    {
        float percentChange = TimeToPercent(time);
        audioPlayer.SetPosition(audioPlayer.GetPosition() + percentChange);
    }

    public bool IsPlaying()
    {
        return audioPlayer.IsPlaying();
    }

    float TimeToPercent(float time)
    {
        return time / audioPlayer.GetLengthTime();
    }

    public async Task DownloadItem(string id)
    {
        if (selectedFeed == null)
            return;
        if (feedReader.GetItem(id, out SyndicationItem? item) && item != null)
        {
            await downloadManager.DownloadItem(selectedFeed, item);
        }
    }

    public void OnFeedChanged(PodcastFeed newFeed)
    {
        selectedFeed = newFeed;
        PopulateFeed();
    }
}
