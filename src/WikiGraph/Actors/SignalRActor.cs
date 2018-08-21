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
            Receive<CommandProcessorActor.CrawlAttemptFailed>(m => _hub.WriteMessage($"Crawl attempt failed: {m.Reason}"));
            Receive<CrawlJobResult>(result => {
                _hub.SendGraph(result.Graph);
            });
        }
    }
}