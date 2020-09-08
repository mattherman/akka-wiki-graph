using System;

namespace WikiGraph.Crawler.Debug
{
    public class PageCrawlStarted
    {
        public Uri Address { get; private set; }
        public int Depth { get; private set; }

        public PageCrawlStarted(Uri address, int depth)
        {
            Address = address;
            Depth = depth;
        }
    }
}
