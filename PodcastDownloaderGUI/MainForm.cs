using Core;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PodcastDownloaderGUI
{
    public partial class MainForm : Form
    {
        private const string Caption = @"Podcast Downloader";
        private MusicPlayer _musicPlayer;
        private State _currentState;

        public MainForm()
        {
            InitializeComponent();

            _musicPlayer = new MusicPlayer(PodcastDownloaderConfiguration.Instance.MusicPlayer.Path);
            _currentState = new StoppedState(this);
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            progressBar.Style = ProgressBarStyle.Marquee;
            object[] podcasts = await GetLastPodcasts(10);
            progressBar.Style = ProgressBarStyle.Blocks;

            podcastsList.DataSource = podcasts;
            podcastsList.DisplayMember = "Title";
            podcastsList.SelectedIndex = 0;
        }

        private static async Task<PodcastItem[]> GetLastPodcasts(int numberOfItems)
        {
            //private const string Url = "https://www.rtl.fr/podcast/les-grosses-tetes.xml";
            //private const string Url = "https://rss.art19.com/the-curiosity-podcast";
            //private const string Url = "http://feeds.djpod.com/dj-hs";

            string xmlList;
            using (var dl = new Downloader())
            {
                xmlList = await dl.DownloadStringAsync("https://www.rtl.fr/podcast/les-grosses-tetes.xml");
            }

            var items = new PodcastXmlParser()
                .GetPodcasts(xmlList)
                .OrderByDescending(i => i.PublishedDate)
                .Take(numberOfItems)
                .ToArray();

            return items;
        }

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            _currentState.Change();
        }

        private void StartMusicPlayer(PodcastItem podcastItem)
        {
            if (!_musicPlayer.IsAvailable) return;

            _musicPlayer.Play(podcastItem.Path);
        }

        private void PodcastsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            downloadButton.Enabled = podcastsList.SelectedItem != null;
        }

        abstract class State
        {
            private readonly MainForm _form;

            protected State(MainForm form)
            {
                _form = form;
            }

            protected MainForm Form => _form;

            public abstract void Change();
        }

        class DownloadingState : State
        {
            private readonly CancellationTokenSource cts;

            public DownloadingState(MainForm form, CancellationTokenSource cts)
                : base(form)
            {
                this.cts = cts;

                Form.downloadButton.Text = "Cancel";
                Form.podcastsList.Enabled = false;
            }

            public override void Change()
            {
                cts.Cancel();

                Form._currentState = new StoppedState(Form);
            }
        }

        class StoppedState : State
        {
            public StoppedState(MainForm form)
                : base(form)
            {
                Form.downloadButton.Text = "Download";
                Form.podcastsList.Enabled = true;
            }

            public override void Change()
            {
                var selectedPodcast = Form.podcastsList.SelectedItem as PodcastItem;
                if (selectedPodcast == null) return;

                selectedPodcast.Path = Path.Combine(@"C:\Temp\", selectedPodcast.Filename);

                var cts = new CancellationTokenSource();
                IProgress<int> progress = new Progress<int>(progressPercentage => Form.progressBar.Value = progressPercentage);
                using (var downloader = new Downloader())
                {
                    Task.Run(() => downloader.DownloadPodcast(selectedPodcast, cts.Token, progress))
                        .ContinueWith(task => Form._currentState = new StoppedState(Form), CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.FromCurrentSynchronizationContext());
                }

                Form._currentState = new DownloadingState(Form, cts);
            }
        }
    }
}
