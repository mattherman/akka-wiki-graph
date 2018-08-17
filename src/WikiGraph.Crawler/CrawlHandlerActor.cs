using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Routing;
using WikiGraph.Crawler;

namespace WikiGraph.Crawler
{
    public class CrawlHandlerActor : ReceiveActor
    {
        private CrawlJob _currentJob;
        private IActorRef _crawlers;
        private ISet<string> _articlesPreviouslyCrawled;
        private ISet<string> _articlesPendingCrawl;
        private int _currentDepth;
        private int _pendingCrawlRequests;
        private Dictionary<string, ISet<string>> _graph;

        public CrawlHandlerActor()
        {
            var props = Props.Create<CrawlActor>().WithRouter(new RoundRobinPool(5));
            _crawlers = Context.ActorOf(props, "crawlers");

            AcceptingCrawlJobs();
        }

        private void AcceptingCrawlJobs()
        {
            _currentJob = null;
            _articlesPreviouslyCrawled = new HashSet<string>();
            _articlesPendingCrawl = new HashSet<string>();
            _currentDepth = 1;
            _pendingCrawlRequests = 0;
            _graph = new Dictionary<string, ISet<string>>();

            Receive<CrawlJob>(job => {
                _currentJob = job;

                InitiatePageCrawl(job.Address);

                Become(ProcessingCrawlJob);
            });
        }

        private void InitiatePageCrawl(Uri address)
        {
            _pendingCrawlRequests++;
            _crawlers.Tell(new CrawlActor.PageCrawlRequest(address));
        }

        private void ProcessingCrawlJob()
        {
            Receive<CrawlActor.PageCrawlResult>(result => {
                _articlesPreviouslyCrawled.Add(result.Title);
                _articlesPendingCrawl.UnionWith(result.LinkedArticles);

                UpdateGraph(result.Title, result.LinkedArticles);

                CompleteCrawlRequest();
            });

            Receive<CrawlActor.PageCrawlFailed>(_ => {
                CompleteCrawlRequest();
            });
        }

        private void CompleteCrawlRequest()
        {
            _pendingCrawlRequests--;

            // Have we finished crawling at the current depth?
            if (_pendingCrawlRequests <= 0)
            {
                _currentDepth++;
                if (_currentDepth > _currentJob.Depth)
                {
                    _currentJob.Requestor.Tell(new CrawlJobResult(_graph));
                    Become(AcceptingCrawlJobs);
                }
                else 
                {
                    foreach (var articlePendingCrawl in _articlesPendingCrawl.Except(_articlesPreviouslyCrawled))
                    {
                        var uriString = Uri.EscapeUriString($"http://wikipedia.org/wiki/{articlePendingCrawl}");
                        InitiatePageCrawl(new Uri(uriString));
                    }
                    _articlesPendingCrawl = new HashSet<string>();
                }
            }
        }

        private void UpdateGraph(string parent, IEnumerable<string> children)
        {
            AddEdges(parent, children);
        
            foreach (var child in children)
            {
                AddEdges(child, new []{ parent });
            }
        }

        private void AddEdges(string source, IEnumerable<string> destinations)
        {
            ISet<string> edges;
            var found = _graph.TryGetValue(source, out edges);
            if (!found)
            {
                edges = new HashSet<string>();
            }

            edges.UnionWith(destinations);
            _graph[source] = edges;
        }
    }
}
