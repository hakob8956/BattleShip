using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace SeaBattle
{
    public class MainHub:Hub
    {
        public async Task Enter(string connectionId = null)
        {
            if (String.IsNullOrEmpty(connectionId))
            {
                Random rnd = new Random();
                connectionId = Convert.ToString(rnd.Next(10000000, 99999999));//GENERATE UNIQ ID
                await Groups.AddToGroupAsync(Context.ConnectionId, connectionId);
                await Clients.Group(connectionId).SendAsync("Notify", $"{connectionId} вошел в чат");
                await Clients.Group(connectionId).SendAsync("GetConnectionID", connectionId);

            }
            else
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, connectionId);
                await Clients.Group(connectionId).SendAsync("Notify", $"{connectionId} вошел в чат");
            }
        }

        public async Task Send(string message, string username, string connectionID)
        {

            await Clients.Group(Context.ConnectionId).SendAsync(username, message);
        }

    }
}
