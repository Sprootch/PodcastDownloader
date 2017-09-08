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
            Filename = System.IO.Path.GetFileName(DownloadUrl) ?? System.IO.Path.GetRandomFileName();
        }

        public string Title { get; }
        public string DownloadUrl { get; }
        public DateTime PublishedDate { get; }
        public string Filename { get; }

        public string Path { get; set; }

        public bool IsDownloaded => File.Exists(Path);

        public override string ToString() => Title;
    }
}
