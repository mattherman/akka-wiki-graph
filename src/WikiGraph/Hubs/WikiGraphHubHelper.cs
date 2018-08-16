using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using WikiGraph.Actors;

namespace WikiGraph.Hubs
{
    /// <inheritdoc />
    /// <summary>
    /// Necessary for getting access to a hub and passing it along to our actors
    /// </summary>
    public class WikiGraphHubHelper : IHostedService
    {
        private readonly IHubContext<WikiGraphHub> _hub;

        public WikiGraphHubHelper(IHubContext<WikiGraphHub> hub)
        {
            _hub = hub;
        }

        internal void WriteRawMessage(string msg)
        {
            WriteMessage(msg);
        }

        internal void WriteMessage(string message)
        {
            _hub.Clients.All.SendAsync("ReceiveMessage", message);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            AkkaStartupTasks.StartAkka();
            SystemActors.SignalRActor.Tell(new SignalRActor.SetHub(this));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}