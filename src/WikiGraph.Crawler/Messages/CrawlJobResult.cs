using System.Collections.Generic;

namespace WikiGraph.Crawler
{
    public class CrawlJobResult
    {
        public IList<Article> Articles { get; }

        public CrawlJobResult(IList<Article> articles)
        {
            Articles = articles;
        }
    }
}