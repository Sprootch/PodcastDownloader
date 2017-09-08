using Core;
using System;
using System.Diagnostics;
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
        private State _currentState;
        private CancellationTokenSource _cts;

        public MainForm()
        {
            InitializeComponent();

            _currentState = new StoppedState(this);
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            progressBar.Style = ProgressBarStyle.Marquee;
            object[] podcasts = await Task.Run(() => GetLastPodcasts(10));
            progressBar.Style = ProgressBarStyle.Blocks;

            podcastsList.Items.AddRange(podcasts);
            podcastsList.SelectedIndex = 0;
        }

        private static PodcastItem[] GetLastPodcasts(int numberOfItems)
        {
            //private const string Url = "https://www.rtl.fr/podcast/les-grosses-tetes.xml";
            //private const string Url = "https://rss.art19.com/the-curiosity-podcast";
            //private const string Url = "http://feeds.djpod.com/dj-hs";

            var items = new PodcastXmlParser()
                .GetPodcasts("https://www.rtl.fr/podcast/les-grosses-tetes.xml")
                .OrderByDescending(i => i.PublishedDate)
                .Take(numberOfItems)
                .ToArray();

            return items;
        }

        private async void downloadButton_Click(object sender, EventArgs e)
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
                using (var downloader = new Downloader())
                {
                    var progress = new Progress<int>(progressPercentage => progressBar.Value = progressPercentage);
                    await downloader.DownloadPodcastAsync(podcast,
                        @"C:\Temp\", _cts.Token, progress);
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

        private static void StartMusicPlayer(PodcastItem podcastItem)
        {
            var playerPath = PodcastDownloaderConfiguration.Instance.MusicPlayer.Path;
            if (string.IsNullOrWhiteSpace(playerPath) || 
                !File.Exists(playerPath)) return;

            var process = new Process
            {
                StartInfo =
                {
                    FileName = playerPath,
                    Arguments = string.Format("/play \"{0}\"", podcastItem.LocalPath),
                    WorkingDirectory = Path.GetDirectoryName(playerPath) ?? "."
                }
            };
            process.Start();
        }

        private void podcastsList_SelectedIndexChanged(object sender, EventArgs e)
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


                _form._currentState = new StoppedState(_form);

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


                _form._currentState = new DownloadingState(_form);
            }
        }
    }
}
