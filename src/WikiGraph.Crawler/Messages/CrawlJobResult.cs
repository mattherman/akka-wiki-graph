using System.Collections.Generic;

namespace WikiGraph.Crawler
{
    public class CrawlJobResult
    {
        public IDictionary<string, ISet<string>> Graph { get; }

        public CrawlJobResult(IDictionary<string, ISet<string>> graph)
        {
            Graph = graph;
        }
    }
}