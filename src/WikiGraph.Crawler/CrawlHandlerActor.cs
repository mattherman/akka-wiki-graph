using System;
using Akka.Actor;
using WikiGraph.Crawler.Messages;

namespace WikiGraph.Crawler
{
    public class CrawlHandlerActor : ReceiveActor
    {
        public CrawlHandlerActor()
        {
            AcceptingCrawlJobs();
        }

        private void AcceptingCrawlJobs()
        {
            Receive<CrawlJob>(job => {

            });
        }
    }
}
