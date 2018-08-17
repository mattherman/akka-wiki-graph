using System.Collections.Generic;
using Akka.Actor;
using WikiGraph.Crawler.Messages;

namespace WikiGraph.Crawler
{
    public class CrawlActor : ReceiveActor
    {
        private IActorRef _downloadActor;
        private IActorRef _articleParserActor;

        public CrawlActor()
        {
            _downloadActor = Context.ActorOf(Props.Create(() => new DownloadActor()), "download");
            _articleParserActor = Context.ActorOf(Props.Create(() => new ArticleParserActor()), "articleParser");
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
                    _articleParserActor.Tell(new ArticleParserActor.HtmlDocument(result.Content));
                }
                else
                {
                    Become(AcceptingCrawlJobs);
                }
            });

            Receive<Article>(article => {
                Context.Parent.Tell(new CrawlJobResult(new List<Article> { article }));
                Become(AcceptingCrawlJobs);
            });
        }
    }
}