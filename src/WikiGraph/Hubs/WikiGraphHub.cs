using Akka.Actor;
using Microsoft.AspNetCore.SignalR;
using WikiGraph.Actors;

namespace WikiGraph.Hubs
{
    public class WikiGraphHub : Hub 
    {
        public void SendMessage(string message)
        {
            SystemActors.SignalRActor.Tell(message, ActorRefs.Nobody);
        }
    }
}