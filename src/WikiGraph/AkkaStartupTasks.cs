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

            var commandProcessorActor = SystemActors.ActorSystem.ActorOf(Props.Create(() => new CommandProcessorActor()), "commandProcessor");

            SystemActors.SignalRActor = SystemActors.ActorSystem.ActorOf(Props.Create(() => new SignalRActor(commandProcessorActor)), "signalr");

            return SystemActors.ActorSystem;
        }
    }
}