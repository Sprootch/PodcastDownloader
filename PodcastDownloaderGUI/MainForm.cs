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
        private CancellationTokenSource _cts;
        private MusicPlayer _musicPlayer;

        public MainForm()
        {
            InitializeComponent();

            _musicPlayer = new MusicPlayer(PodcastDownloaderConfiguration.Instance.MusicPlayer.Path);
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

        private async void DownloadButton_Click(object sender, EventArgs e)
        {
            if (_cts != null)
            {
                _cts.Cancel();
                return;
            }

            var selectedPodcast = podcastsList.SelectedItem as PodcastItem;
            if (selectedPodcast == null) return;

            await DownloadPodcast(selectedPodcast);
            if (selectedPodcast.IsDownloaded)
            {
                StartMusicPlayer(selectedPodcast);
            }
        }

        private async Task<PodcastItem> DownloadPodcast(PodcastItem podcast)
        {
            downloadButton.Text = @"Cancel";
            podcastsList.Enabled = false;
            _cts = new CancellationTokenSource();

            try
            {
                podcast.Path = Path.Combine(@"C:\Temp\", podcast.Filename);

                using (var downloader = new Downloader())
                {
                    var progress = new Progress<int>(progressPercentage => progressBar.Value = progressPercentage);
                    await downloader.DownloadPodcastAsync(podcast, _cts.Token, progress);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            finally
            {
                downloadButton.Text = @"Download";
                podcastsList.Enabled = true;

                _cts.Dispose();
                _cts = null;
            }

            return podcast;
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
            protected readonly MainForm _form;

            protected State(MainForm form)
            {
                _form = form;
            }

            public abstract void Change();
        }

        class DownloadingState : State
        {
            public DownloadingState(MainForm form)
                : base(form)
            {
                _form.downloadButton.Text = "Cancel";
                _form.podcastsList.Enabled = false;
            }

            public override void Change()
            {
                //downloadButton.Enabled = true;
                //podcastsList.Enabled = true;
            }
        }

        class StoppedState : State
        {
            public StoppedState(MainForm form)
                : base(form)
            {
                _form.downloadButton.Text = "Download";
                _form.podcastsList.Enabled = true;
            }

            public override void Change()
            {
            }
        }
    }
}
