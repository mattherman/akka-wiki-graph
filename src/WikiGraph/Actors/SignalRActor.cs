using Akka.Actor;
using Microsoft.AspNetCore.SignalR;
using WikiGraph.Crawler;
using WikiGraph.Hubs;

namespace WikiGraph.Actors
{
    public class SignalRActor : ReceiveActor, IWithUnboundedStash
    {
        public class SetHub
        {
            public WikiGraphHubHelper Hub { get; }
            
            public SetHub(WikiGraphHubHelper hub)
            {
                Hub = hub;
            }
        }

        public class InitiateCrawl
        {
            public string Address { get; }
            public int Depth { get; }

            public InitiateCrawl(string address, int depth)
            {
                Address = address;
                Depth = depth;
            }
        }

        public IStash Stash { get; set; }
        private WikiGraphHubHelper _hub;
        private IActorRef _commandProcessor;

        public SignalRActor(IActorRef commandProcessor)
        {
            _commandProcessor = commandProcessor;
            WaitingForHub();
        }

        private void WaitingForHub()
        {
            Receive<SetHub>(h =>
            {
                _hub = h.Hub;
                Become(HubAvailable);
                Stash.UnstashAll();
            });

            ReceiveAny(_ => Stash.Stash());
        }

        private void HubAvailable()
        {
            Receive<InitiateCrawl>(initCrawl => _commandProcessor.Tell(new CommandProcessorActor.AttemptCrawl(initCrawl.Address, initCrawl.Depth)));
            Receive<CommandProcessorActor.CrawlAttemptFailed>(m => {
                var msg = new DebugMessage($"Crawl attempt failed: {m.Reason}", MessageType.Error);
                _hub.WriteMessage(msg);
            });
            Receive<CrawlJobResult>(result => {
                var msg = new DebugMessage($"Crawl complete!", MessageType.Informational);
                _hub.WriteMessage(msg);
                _hub.SendGraph(result.Graph);
            });

            Receive<Crawler.Debug.PageCrawlStarted>(m => {
                var msg = new DebugMessage($"Started page crawl: depth = {m.Depth}, address = {m.Address}", MessageType.Informational);
                _hub.WriteMessage(msg);
            });
            Receive<Crawler.Debug.PageCrawlCompleted>(m => {
                var msg = new DebugMessage($"Finished page crawl: title = {m.Title}, numLinks = {m.LinkedArticleCount}", MessageType.Informational);
                _hub.WriteMessage(msg);
            });
            Receive<Crawler.Debug.PageCrawlFailed>(m => {
                var msg = new DebugMessage($"Failed page crawl!", MessageType.Error);
            });
        }
    }
}