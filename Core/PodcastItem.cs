using System;
using System.IO;

namespace Core
{
    public class PodcastItem
    {
        public PodcastItem(string title, string downloadUrl, DateTime publishedDate)
        {
            Title = title;
            DownloadUrl = downloadUrl;
            PublishedDate = publishedDate;
            Filename = Path.GetFileName(DownloadUrl) ?? Path.GetRandomFileName();
        }

        public string Title { get; private set; }
        public string DownloadUrl { get; private set; }
        public DateTime PublishedDate { get; private set; }
        public string Filename { get; private set; }

        public string LocalPath { get; set; }

        public bool IsDownloaded
        {
            get { return !string.IsNullOrWhiteSpace(LocalPath); }
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
