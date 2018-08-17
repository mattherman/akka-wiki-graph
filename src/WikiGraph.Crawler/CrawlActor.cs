using System.Collections.Generic;
using Akka.Actor;
using HtmlAgilityPack;
using WikiGraph.Crawler.Messages;

namespace WikiGraph.Crawler
{
    public class CrawlActor : ReceiveActor
    {
        private IActorRef _downloadActor;
        private IActorRef _linkCollectorActor;

        public CrawlActor()
        {
            _downloadActor = Context.ActorOf(Props.Create(() => new DownloadActor()), "download");
            _linkCollectorActor = Context.ActorOf(Props.Create(() => new LinkCollectorActor()), "linkCollector");
            AcceptingCrawlJobs();
        }

        private void AcceptingCrawlJobs()
        {
            Receive<CrawlJob>(job => {
                _downloadActor.Tell(job.Address);
                Become(ProcessingJob);
            });
        }

        private void ProcessingJob()
        {
            Receive<DownloadActor.PageDownloadResult>(result => {
                if (result.Success)
                {
                    var doc = new HtmlDocument();
                    doc.LoadHtml(result.Content);
                    _linkCollectorActor.Tell(doc);
                }
                else
                {
                    Become(AcceptingCrawlJobs);
                }
            });

            Receive<List<Article>>(linkedArticles => {
                Context.Parent.Tell(new CrawlJobResult(linkedArticles));
                Become(AcceptingCrawlJobs);
            });
        }
    }
}