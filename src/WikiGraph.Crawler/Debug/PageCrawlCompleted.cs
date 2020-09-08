using System;

namespace WikiGraph.Crawler.Debug
{
    public class PageCrawlCompleted
    {
        public string Title { get; private set; }
        public int LinkedArticleCount { get; private set; }

        public PageCrawlCompleted(string title, int linkedArticleCount)
        {
            Title = title;
            LinkedArticleCount = linkedArticleCount;
        }
    }
}
