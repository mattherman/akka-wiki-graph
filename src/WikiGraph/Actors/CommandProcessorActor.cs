using System;
using Akka.Actor;
using WikiGraph.Crawler;

namespace WikiGraph.Actors 
{
    public class CommandProcessorActor : ReceiveActor
    {
        public class AttemptCrawl
        {
            public string Address { get; }

            public int Depth { get; }

            public AttemptCrawl(string address, int depth)
            {
                Address = address;
                Depth = depth;
            }
        }

        public class CrawlAttemptFailed
        {
            public string Reason { get; }

            public CrawlAttemptFailed(string reason)
            {
                Reason = reason;
            }
        }

        private IActorRef _jobHandler;

        public CommandProcessorActor()
        {
            _jobHandler = Context.ActorOf(Props.Create(() => new CrawlHandlerActor()), "crawlHandler");
            AcceptCommands();
        }

        private void AcceptCommands()
        {
            Receive<AttemptCrawl>(m => {
                if (Uri.IsWellFormedUriString(m.Address, UriKind.Absolute) && m.Address.Contains("wikipedia.org/wiki"))
                {
                    _jobHandler.Tell(new CrawlJob(new Uri(m.Address), m.Depth, Sender));
                }
                else 
                {
                    Sender.Tell(new CrawlAttemptFailed("Invalid URI string"));
                }
            });
        }
    }
}