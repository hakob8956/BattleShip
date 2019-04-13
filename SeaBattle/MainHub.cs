using GameCore;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using SeaBattle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace SeaBattle
{
    public class MainHub : Hub
    {
     
        private static Dictionary<string, ConnectionModel> userRepo = new Dictionary<string, ConnectionModel>();//ClientId,ConnectionModel(id,field)
        private static Dictionary<string, SortStepModel> userGroupRepo = new Dictionary<string, SortStepModel>();//connectionId(groupId),SortStepModel(currentTurn,currentConnectionIdThisTurn)
        private GeneralFunctions GeneralFunctions = new GeneralFunctions();

        public async Task Enter(string connectionId = null)//JoinGroup,Merge date players
        {
            Field field = new Field();
            field.SetRandomShips();

            if (userRepo.Where(p => p.Value.connectionId == connectionId).Count() >= 2) connectionId = null;//the user of group is'nt  more than two
            if (String.IsNullOrEmpty(connectionId))//Genereic new connectionId
            {
                Random rnd = new Random();
                connectionId = Convert.ToString(rnd.Next(10000000, 99999999));//GENERATE UNIQ ID
                await Groups.AddToGroupAsync(Context.ConnectionId, connectionId);
                await Clients.Group(connectionId).SendAsync("Notify", $"Waiting for opponent");
                await Clients.Group(connectionId).SendAsync("GetConnectionID", connectionId);
            }
            else
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, connectionId);
                await Clients.Group(connectionId).SendAsync("Notify", $"Waiting for opponent");
            }
            userRepo[Context.ConnectionId] = new ConnectionModel()
            {
                connectionId = connectionId,
                field = field.Plans
            };
            if (userRepo.Where(p => p.Value.connectionId == connectionId).Count() == 2)
            {
                var anotherUserConnectionID = userRepo.FirstOrDefault(p => p.Value.connectionId == connectionId && p.Key != Context.ConnectionId).Key;
                userGroupRepo[connectionId] = new SortStepModel
                {
                    currentConnectionIdThisTurn = anotherUserConnectionID,
                    currentTurn = true
                };
                
                await Clients.OthersInGroup(connectionId).SendAsync("StartGame", JsonConvert.SerializeObject(userRepo[anotherUserConnectionID].field),connectionId);
                await Clients.Client(Context.ConnectionId).SendAsync("StartGame", JsonConvert.SerializeObject(userRepo[Context.ConnectionId].field),connectionId);

                await Clients.OthersInGroup(connectionId).SendAsync("ChangeTurn", userGroupRepo[connectionId].currentTurn);

                await Clients.OthersInGroup(connectionId).SendAsync("Notify", "The game started, your turn.");
                await Clients.Client(Context.ConnectionId).SendAsync("Notify", "Opponent's turn, please wait.");

            }
        }
        public async Task SendCordinateEnemy(int x, int y, string connectionId)
        {
            if (userGroupRepo[connectionId].currentConnectionIdThisTurn == Context.ConnectionId)//Check Current Turn
            {
                var anotherConnectionId = userRepo.FirstOrDefault(p => p.Value.connectionId == connectionId && p.Key != Context.ConnectionId).Key;

                var enemyfield = userRepo[anotherConnectionId].field;//Take EnemyField
                var (newEnemyField, countKill) =  GeneralFunctions.Shoot(enemyfield, x, y);//Set new EnemyField

                bool win = GeneralFunctions.Win(newEnemyField);
                var jsonNewField = JsonConvert.SerializeObject(newEnemyField);

                if (newEnemyField[y, x] != (int)FieldType.Shooted)
                {
                    userGroupRepo[connectionId].currentConnectionIdThisTurn = anotherConnectionId;
                    userGroupRepo[connectionId].currentTurn = false;
                }
                else
                    userGroupRepo[connectionId].currentTurn = true;

                //TODO FIX WIN
                string message1 = userGroupRepo[connectionId].currentTurn ? "Your turn." : "Opponent's turn, please wait.";
                string message2 = !userGroupRepo[connectionId].currentTurn ? "Your turn." : "Opponent's turn, please wait.";
                await Clients.Client(Context.ConnectionId).SendAsync("TakeStatus", jsonNewField, userGroupRepo[connectionId].currentTurn, message1,countKill, win);
                await Clients.OthersInGroup(connectionId).SendAsync("SetStatus", jsonNewField, !userGroupRepo[connectionId].currentTurn, message2,countKill,win);//Send current User
            }

        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override  Task OnDisconnectedAsync(Exception exception)//IF One user out at group delete all members of group!
        {
            string groupname = userRepo.FirstOrDefault(p => p.Key == Context.ConnectionId).Key;//take user groupname(connectionId)
            foreach (var item in userRepo.Where(p=>p.Key==groupname && p.Value.connectionId!=Context.ConnectionId))//disconnect all members in group
            {
                 Groups.RemoveFromGroupAsync(item.Value.connectionId, groupname);
                
            }
            var anotheruserId = userRepo.FirstOrDefault(p => p.Key == groupname && p.Value.connectionId != Context.ConnectionId).Value.connectionId;
            if (anotheruserId == null) userRepo.Remove(anotheruserId);
            userRepo.Remove(Context.ConnectionId);
           
            userGroupRepo.Remove(groupname);
            return base.OnDisconnectedAsync(exception);
        }


    }
}
