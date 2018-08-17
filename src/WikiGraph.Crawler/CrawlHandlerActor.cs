using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using WikiGraph.Crawler.Messages;

namespace WikiGraph.Crawler
{
    public class CrawlHandlerActor : ReceiveActor
    {
        private ISet<IActorRef> _crawlers;
        private CrawlJob _currentJob;

        public CrawlHandlerActor()
        {
            _crawlers = new HashSet<IActorRef>();
            AcceptingCrawlJobs();
        }

        private void AcceptingCrawlJobs()
        {
            Receive<CrawlJob>(job => {
                _currentJob = job;

                var newCrawler = Context.ActorOf(Props.Create(() => new CrawlActor()));
                _crawlers.Add(newCrawler);

                newCrawler.Tell(new CrawlActor.PageCrawlRequest(job.Address));
            });

            Receive<CrawlActor.PageCrawlResult>(result => {
                _crawlers.Remove(Sender);
                Sender.Tell(PoisonPill.Instance);

                _currentJob.Requestor.Tell(
                    new CrawlJobResult(
                        new List<Article> 
                        { 
                            new Article(result.Title, 
                                result.LinkedArticles
                                    .Select(a => new Article(a, new List<Article>()))
                                    .ToList()
                            )
                        } 
                    )
                );
                _currentJob = null;
            });
        }
    }
}
