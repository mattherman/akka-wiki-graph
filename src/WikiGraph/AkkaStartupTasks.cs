using Akka.Actor;
using Akka.Routing;
using WikiGraph.Actors;

namespace WikiGraph
{
    public static class AkkaStartupTasks
    {
        public static ActorSystem StartAkka()
        {
            SystemActors.ActorSystem = ActorSystem.Create("wikiGraph");
            SystemActors.CommandProcessorActor = SystemActors.ActorSystem.ActorOf(Props.Create(() => new CommandProcessorActor()), "commandProcessor");
            SystemActors.SignalRActor = SystemActors.ActorSystem.ActorOf(Props.Create(() => new SignalRActor(SystemActors.CommandProcessorActor)), "signalr");
            return SystemActors.ActorSystem;
        }
    }
}