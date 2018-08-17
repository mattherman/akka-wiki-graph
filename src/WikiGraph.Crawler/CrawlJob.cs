using System;
using Akka.Actor;

namespace WikiGraph.Crawler
{
    public class CrawlJob
    {
        public Uri Address { get; }

        public int Depth { get; }

        public IActorRef Requestor { get; }

        public CrawlJob(Uri address, int depth, IActorRef requestor)
        {
            Address = address;
            Depth = depth;
            Requestor = requestor;
        }
    }
}