using Akka.Actor;
using Microsoft.AspNetCore.SignalR;
using WikiGraph.Actors;

namespace WikiGraph.Hubs
{
    public class WikiGraphHub : Hub 
    {
        public void SubmitAddress(string address)
        {
            SystemActors.SignalRActor.Tell(address, ActorRefs.Nobody);
        }
    }
}