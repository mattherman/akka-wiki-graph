using Akka.Actor;
using Microsoft.AspNetCore.SignalR;
using WikiGraph.Actors;

namespace WikiGraph.Hubs
{
    public class WikiGraphHub : Hub 
    {
        public void SubmitAddress(string address, int depth)
        {
            SystemActors.SignalRActor.Tell(new SignalRActor.InitiateCrawl(address, depth), ActorRefs.Nobody);
        }
    }
}