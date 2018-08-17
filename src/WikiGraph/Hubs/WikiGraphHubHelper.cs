using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using WikiGraph.Actors;
using WikiGraph.Crawler;

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

        internal void WriteMessage(string message)
        {
            _hub.Clients.All.SendAsync("ReceiveDebugInfo", message);
        }

        internal void SendGraph(IDictionary<string, ISet<string>> graph)
        {
            _hub.Clients.All.SendAsync("ReceiveGraph", graph);
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