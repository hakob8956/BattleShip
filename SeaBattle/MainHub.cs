using GameCore;
using Microsoft.AspNetCore.SignalR;
using SeaBattle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace SeaBattle
{
    public class MainHub : Hub
    {
        //TODO CREATE INTERFACE FOR Dictinoray-->(Singleton)
        private static Dictionary<string, string> Connections = new Dictionary<string, string>();//ClientId,GroupName
        private GeneralFunctions GeneralFunctions = new GeneralFunctions();

        public async Task Enter(string connectionId = null)//JoinGroup,Merge date players
        {
            if (Connections.Where(p => p.Value == connectionId).Count() >= 2) connectionId = null;//the user of group is'nt  more than two
            if (String.IsNullOrEmpty(connectionId))//Genereic new connectionId
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
            Connections.Add(Context.ConnectionId, connectionId);//Add Clients

        }
        //public void ReadyConnection()
        //{

        //}
        public async Task SendStatus(int value, int x, int y, string connectionId)
        {
            int newValue = GeneralFunctions.Shoot(value, x, y);
            var anotherConnectionId = Connections.FirstOrDefault(p => p.Value == connectionId && p.Key != Context.ConnectionId).Key;
            await Clients.OthersInGroup(connectionId).SendAsync("TakeStatus", newValue);
            await Clients.GroupExcept(connectionId, anotherConnectionId).SendAsync("SetStatus", newValue, x, y, connectionId);

        }
        public async Task SendCordinateEnemy(int x, int y, string connectionId)
        {
            await Clients.OthersInGroup(connectionId).SendAsync("SendRequestTakeValue", x, y, connectionId);
        }
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)//IF One user out at group delete all members of group!
        {
            string groupName = Connections.FirstOrDefault(p => p.Key == Context.ConnectionId).Value;//Take user GroupName
            Connections.Remove(Context.ConnectionId);//Delete currentUser
            foreach (var user in Connections.Where(p => p.Value == groupName))//Delete users of Group
            {
                Groups.RemoveFromGroupAsync(user.Key, user.Value);//user.Key->ConnectionID,user.Value->GroupName
                Connections.Remove(user.Key);
            }
            return base.OnDisconnectedAsync(exception);
        }


    }
}
