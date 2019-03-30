using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR
{
    public class ChatHub:Hub
    {
    
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.All.SendAsync("Notify", $"{Context.ConnectionId} Exit in chat");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task Send(List<string> items)
        {

            await Clients.Caller.SendAsync("Receive", items);
        }


    }
}
