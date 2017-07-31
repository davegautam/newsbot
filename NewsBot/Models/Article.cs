using System;

namespace NewsBot.Models
{
    public class Article
    {
        public string Author { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Uri Url { get; set; }
        public Uri UrlToImage { get; set; }
        public string PublishedAt { get; set; }
    }
}