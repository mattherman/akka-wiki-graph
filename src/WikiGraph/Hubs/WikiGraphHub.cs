using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace WikiGraph.Hubs
{
    public class WikiGraphHub : Hub {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}