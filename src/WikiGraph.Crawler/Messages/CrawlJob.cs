using System;
using Akka.Actor;

namespace WikiGraph.Crawler.Messages
{
    public class CrawlJob
    {
        public Uri Address { get; }

        public IActorRef Requestor { get; }

        public CrawlJob(Uri address, IActorRef requestor)
        {
            Address = address;
            Requestor = requestor;
        }
    }
}