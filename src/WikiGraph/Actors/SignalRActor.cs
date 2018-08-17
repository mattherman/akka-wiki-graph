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
            Receive<string>(addr => _commandProcessor.Tell(new CommandProcessorActor.AttemptCrawl(addr)));
            Receive<CommandProcessorActor.CrawlAttemptFailed>(m => _hub.WriteMessage($"Crawl attempt failed: {m.Reason}"));
            Receive<CrawlJobResult>(result => {
                foreach (var article in result.LinkedArticles)
                {
                    _hub.SendLink(article.Address, article.Name);
                }
            });
        }
    }
}