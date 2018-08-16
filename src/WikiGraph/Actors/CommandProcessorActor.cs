using System;
using Akka.Actor;

namespace WikiGraph.Actors 
{
    public class CommandProcessorActor : ReceiveActor
    {
        public class AttemptCrawl
        {
            public string Address { get; }

            public AttemptCrawl(string address)
            {
                Address = address;
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

        public CommandProcessorActor()
        {
            AcceptCommands();
        }

        private void AcceptCommands()
        {
            Receive<AttemptCrawl>(m => {
                if (!Uri.IsWellFormedUriString(m.Address, UriKind.Absolute))
                {
                    Sender.Tell(new CrawlAttemptFailed("Invalid URI string"));
                    return;
                }

                var uri = new Uri(m.Address);
                if (!uri.Host.Contains("wikipedia.org"))
                {
                    Sender.Tell(new CrawlAttemptFailed("URI host must contain 'wikipedia.org'"));
                    return;
                }
            });
        }
    }
}