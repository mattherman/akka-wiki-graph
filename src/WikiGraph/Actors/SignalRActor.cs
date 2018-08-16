using Akka.Actor;
using Microsoft.AspNetCore.SignalR;
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

        public SignalRActor()
        {
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
            Receive<string>(str => _hub.WriteRawMessage(str));
        }
    }
}