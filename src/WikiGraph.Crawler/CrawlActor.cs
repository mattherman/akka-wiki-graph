using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using WikiGraph.Crawler.Messages;

namespace WikiGraph.Crawler
{
    public class CrawlActor : ReceiveActor
    {
        public class PageCrawlRequest
        {
            public Uri Address { get; }
            
            public PageCrawlRequest(Uri address)
            {
                Address = address;
            }
        }

        public class PageCrawlResult
        {
            public string Title { get; }
            public ICollection<string> LinkedArticles { get; }

            public PageCrawlResult(string title, ICollection<string> linkedArticles)
            {
                Title = title;
                LinkedArticles = linkedArticles;
            }
        }

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
            Receive<PageCrawlRequest>(request => {
                _downloadActor.Tell(request.Address);
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

            Receive<ArticleParserActor.ArticleParseResult>(result => {
                Context.Parent.Tell(new PageCrawlResult(result.Title, result.LinkedArticles));
                Become(AcceptingCrawlJobs);
            });
        }
    }
}