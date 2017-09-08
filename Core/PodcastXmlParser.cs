using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Core
{
    public class PodcastXmlParser
    {
        public IReadOnlyCollection<PodcastItem> GetPodcasts(string xmlPodcastList)
        {
            // TODO: error checking
            var doc = XDocument.Parse(xmlPodcastList);
            var items = 
            (
                from x in doc.Root.Elements("channel").Elements("item")
                select new PodcastItem(x.Element("title").Value, x.Element("guid").Value, DateTime.Parse(x.Element("pubDate").Value))
            );

            return items.ToArray();
        }
    }
}
