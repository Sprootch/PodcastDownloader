using Core;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PodcastDownloaderWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<PodcastItem> Podcasts { get; set; }

        public MainWindow()
        {
            this.Loaded += MyWindow_Loaded;
            InitializeComponent();
        }

        private async void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ProgressBar.IsIndeterminate = true;
            Podcasts = await Task.Run(() => GetLastPodcasts(10));
            ProgressBar.IsIndeterminate = false;

            PodcastsList.ItemsSource = Podcasts;
            PodcastsList.DisplayMemberPath = "Title";
            PodcastsList.SelectedIndex = 0;
        }

        private ObservableCollection<PodcastItem> GetLastPodcasts(int numberOfItems)
        {
            var index = new Downloader().DownloadString("https://www.rtl.fr/podcast/les-grosses-tetes.xml");
            var items = new PodcastXmlParser()
                .GetPodcasts(index)
                .OrderByDescending(i => i.PublishedDate)
                .Take(numberOfItems);

            return new ObservableCollection<PodcastItem>(items);
        }

        private void DownloadButton_OnClick(object sender, RoutedEventArgs e)
        {
            var selectedPodcast = PodcastsList.SelectedItem as PodcastItem;
        }

        private void PodcastsList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DownloadButton.IsEnabled = PodcastsList.SelectedItem != null;
        }
    }
}
