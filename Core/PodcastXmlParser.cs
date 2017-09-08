using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Core
{
    public class PodcastXmlParser
    {
        public IReadOnlyCollection<PodcastItem> GetPodcasts(string url)
        {
            string index;
            using (var dl = new Downloader())
            {
                index = dl.DownloadString(url);
            }

            // TODO: refactor
            var doc = XDocument.Parse(index);
            var items = 
            (
                from x in doc.Root.Elements("channel").Elements("item")
                select new PodcastItem(x.Element("title").Value, x.Element("guid").Value, DateTime.Parse(x.Element("pubDate").Value))
            ).ToArray();

            return items;
        }
    }
}
