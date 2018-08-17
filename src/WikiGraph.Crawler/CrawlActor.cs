using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using WikiGraph.Crawler.Messages;

namespace WikiGraph.Crawler
{
    public class CrawlActor : ReceiveActor, IWithUnboundedStash
    {
        public class PageCrawlRequest
        {
            public Uri Address { get; }
            
            public PageCrawlRequest(Uri address)
            {
                Address = address;
            }
        }

        public class PageCrawlFailed { }

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
        private IActorRef _currentSender;

        public IStash Stash { get; set; }

        public CrawlActor()
        {
            _downloadActor = Context.ActorOf(Props.Create(() => new DownloadActor()), "download");
            _articleParserActor = Context.ActorOf(Props.Create(() => new ArticleParserActor()), "articleParser");
            AcceptingCrawlRequests();
        }

        private void AcceptingCrawlRequests()
        {
            Receive<PageCrawlRequest>(request => {
                _currentSender = Sender;
                _downloadActor.Tell(request.Address);
                Become(ProcessingJob);
            });
        }

        private void BecomeAcceptingCrawlRequests()
        {
            Become(AcceptingCrawlRequests);
            Stash.UnstashAll();
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
                    _currentSender.Tell(new PageCrawlFailed());
                    BecomeAcceptingCrawlRequests();
                }
            });

            Receive<ArticleParserActor.ArticleParseResult>(result => {
                _currentSender.Tell(new PageCrawlResult(result.Title, result.LinkedArticles));
                BecomeAcceptingCrawlRequests();
            });

            ReceiveAny(_ => Stash.Stash());
        }
    }
}