using System;
using System.Collections.Generic;
using Akka.Actor;
using WikiGraph.Crawler.Messages;

namespace WikiGraph.Crawler
{
    public class CrawlHandlerActor : ReceiveActor
    {
        private IDictionary<IActorRef, IActorRef> _crawlers;

        public CrawlHandlerActor()
        {
            _crawlers = new Dictionary<IActorRef, IActorRef>();
            AcceptingCrawlJobs();
        }

        private void AcceptingCrawlJobs()
        {
            Receive<CrawlJob>(job => {
                var newCrawler = Context.ActorOf(Props.Create(() => new CrawlActor()));
                _crawlers.Add(newCrawler, Sender);

                newCrawler.Tell(job);
            });

            Receive<CrawlJobResult>(result => {
                var originalSender = _crawlers[Sender];
                _crawlers.Remove(Sender);
                Sender.Tell(PoisonPill.Instance);

                originalSender.Tell(result);
            });
        }
    }
}
