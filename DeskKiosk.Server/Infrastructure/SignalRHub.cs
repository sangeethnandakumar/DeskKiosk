using Microsoft.AspNetCore.SignalR;
using System.Text;

namespace DeskKiosk.Server.Infrastructure
{
    public class SignalRHub : Hub
    {
        public async Task InvokeAsync(string component, string methodName, object payload)
        {
            await Clients.All.SendAsync($"{component}_{methodName}", payload);
        }

        public async Task AppPage_OnTextChange(string message)
        {
            await InvokeAsync("AppPage", "OnTextChange", Convert.ToBase64String(Encoding.UTF8.GetBytes(message)));
            if(message.Length > 5)
            {
                await InvokeAsync("AppPage", "OnAlert", Convert.ToBase64String(Encoding.UTF8.GetBytes(message)));
            }
        }
    }
}
