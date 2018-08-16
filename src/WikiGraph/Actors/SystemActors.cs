using Akka.Actor;

namespace WikiGraph.Actors
{
    /// <summary>
    /// Static class used to work around weird SignalR constructors
    /// 
    /// (need to learn how to wire this up properly in signalr)
    /// </summary>
    public static class SystemActors
    {
        public static ActorSystem ActorSystem;

        public static IActorRef SignalRActor = ActorRefs.Nobody;
    }
}