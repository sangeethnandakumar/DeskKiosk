using Microsoft.AspNetCore.SignalR;
using System.Text;

namespace DeskKiosk.Server.Infrastructure
{
    public class BridgeCommand
    {
        public string Command { get; set; }
        public dynamic Params { get; set; }
    }

    public class SignalRHub : Hub
    {
        public async Task SendNotificationAsync(BridgeCommand message)
        {
            await Clients.All.SendAsync("NotificationListner", message);
        } 
        
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("NotificationListner", Convert.ToBase64String(Encoding.UTF8.GetBytes(message)));
        }
    }
}
