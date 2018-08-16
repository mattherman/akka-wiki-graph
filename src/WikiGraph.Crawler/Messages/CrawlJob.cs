using System;

namespace WikiGraph.Crawler.Messages
{
    public class CrawlJob
    {
        public Uri Address { get; }

        public CrawlJob(Uri address)
        {
            Address = address;
        }
    }
}