using System.Collections.Generic;

namespace WikiGraph.Crawler
{
    public class CrawlJobResult
    {
        public IList<Article> LinkedArticles { get; }

        public CrawlJobResult(IList<Article> linkedArticles)
        {
            LinkedArticles = linkedArticles;
        }
    }
}