using System;

namespace WikiGraph.Crawler
{
    public class Article
    {
        public string Name { get; }
        public Uri Address { get; }

        public Article(string name, Uri address)
        {
            Name = name;
            Address = address;
        }
    }
}