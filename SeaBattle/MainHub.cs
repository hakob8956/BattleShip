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

        private static Dictionary<string, ConnectionModel> userRepo = new Dictionary<string, ConnectionModel>();//ClientId,ConnectionModel(groupId,field)
        private static Dictionary<string, SortStepModel> userGroupRepo = new Dictionary<string, SortStepModel>();//connectionId(groupId),SortStepModel(currentTurn,currentConnectionIdThisTurn)
        private GeneralFunctions GeneralFunctions = new GeneralFunctions();

        public async Task Enter(string connectionId = null)//JoinGroup,Merge date players
        {
            Field field = new Field();
            field.SetRandomShips();
            var existUserRepo = userRepo.Where(p => p.Key == Context.ConnectionId && p.Value != null).Count();
            if (existUserRepo != 0)
            {
                if (GeneralFunctions.CountAllShips(userRepo[Context.ConnectionId].field) != 20)
                {
                    await Clients.Client(Context.ConnectionId).SendAsync("InvaildField");
                    return;
                }
            }
            if (userRepo.Where(p => p.Value?.connectionId == connectionId).Count() >= 2) connectionId = null;//the user of group is'nt  more than two
            if (String.IsNullOrEmpty(connectionId))//Genereic new connectionId
            {
                Random rnd = new Random();
                connectionId = Convert.ToString(rnd.Next(10000000, 99999999));//GENERATE UNIQ ID
                await Groups.AddToGroupAsync(Context.ConnectionId, connectionId);
                await Clients.Group(connectionId).SendAsync("GetConnectionID", connectionId);

                await Clients.Group(connectionId).SendAsync("Notify", $"Waiting for opponent");
            }
            else
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, connectionId);
                await Clients.Group(connectionId).SendAsync("Notify", $"Waiting for opponent");
            }
            if (existUserRepo != 0)
            {
                userRepo[Context.ConnectionId].connectionId = connectionId;

            }
            else
            {

                userRepo[Context.ConnectionId] = new ConnectionModel()
                {
                    connectionId = connectionId,
                    field = field.Plans
                };
            }

            if (userRepo.Where(p => p.Value.connectionId == connectionId).Count() == 2)
            {
                var anotherUserConnectionID = userRepo.FirstOrDefault(p => p.Value.connectionId == connectionId && p.Key != Context.ConnectionId).Key;
                userGroupRepo[connectionId] = new SortStepModel
                {
                    currentConnectionIdThisTurn = anotherUserConnectionID,
                    currentTurn = true
                };

                await Clients.OthersInGroup(connectionId).SendAsync("StartGame", JsonConvert.SerializeObject(userRepo[anotherUserConnectionID].field), connectionId);
                await Clients.Client(Context.ConnectionId).SendAsync("StartGame", JsonConvert.SerializeObject(userRepo[Context.ConnectionId].field), connectionId);

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
                var (newEnemyField, countKill) = GeneralFunctions.Shoot(enemyfield, x, y);//Set new EnemyField

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
                await Clients.Client(Context.ConnectionId).SendAsync("TakeStatus", jsonNewField, userGroupRepo[connectionId].currentTurn, message1, countKill, win);
                await Clients.OthersInGroup(connectionId).SendAsync("SetStatus", jsonNewField, !userGroupRepo[connectionId].currentTurn, message2, countKill, win);//Send current User
            }

        }

        public async Task DeleteShip(int x, int y)
        {
            Field field = new Field();
            if (userRepo.ContainsKey(Context.ConnectionId) && userRepo[Context.ConnectionId] != null //Check userRepo null
                && !string.IsNullOrEmpty(userRepo[Context.ConnectionId].ToString()))
            {
                userRepo[Context.ConnectionId].field = GeneralFunctions.DeleteShip(userRepo[Context.ConnectionId].field, x, y);
            }
            await Clients.Client(Context.ConnectionId).SendAsync("DeleteShip");

        }

        public async Task DeleteField()
        {
            if ((userRepo.ContainsKey(Context.ConnectionId)))
            {
                userRepo[Context.ConnectionId] = null;
            }
            await Clients.Client(Context.ConnectionId).SendAsync("DeleteField");
        }
        public async Task CanSetShip(int shipType, int orentation, int x, int y, int oldX, int oldY, string elementId, bool changeDir)
        {

            Field field = new Field();
            bool canSet = false;
            var oldOrentation = orentation;
            switch (orentation)
            {
                case (int)Orentation.Horizontal:
                    oldOrentation = (int)Orentation.Vertical;
                    break;
                case (int)Orentation.Vertical:
                    oldOrentation = (int)Orentation.Horizontal;
                    break;
                default:
                    break;
            }
            if (userRepo.ContainsKey(Context.ConnectionId))
            {
                field.Plans = userRepo[Context.ConnectionId].field;
                canSet = field.SetShip(shipType, orentation, y, x);
                if (!canSet)
                {
                    if (changeDir)
                    {
                        field.SetShip(shipType, oldOrentation, y, x);
                    }
                    else
                    {
                        field.SetShip(shipType, orentation, oldY, oldX);
                    }

                }
                userRepo[Context.ConnectionId].field = field.Plans;
            }
            else
            {
                canSet = field.SetShip(shipType, orentation, y, x);
                if (!canSet)
                {
                    if (changeDir)
                    {
                        field.SetShip(shipType, oldOrentation, y, x);
                    }
                    else
                    {
                        field.SetShip(shipType, orentation, oldY, oldX);
                    }
                }
                userRepo[Context.ConnectionId] = new ConnectionModel()
                {
                    connectionId = "",
                    field = field.Plans
                };
            }
            await Clients.Client(Context.ConnectionId).SendAsync("CanSetShip", canSet, elementId, x, y, changeDir);
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)//IF One user out at group delete all members of group!
        {
            string groupname = userRepo.FirstOrDefault(p => p.Key == Context.ConnectionId).Value?.connectionId;//take user groupname(connectionId)
            string anotheruserId = userRepo.FirstOrDefault(p => p.Value?.connectionId == groupname && p.Key != Context.ConnectionId).Key;//Take another ClientId      
            foreach (var item in userRepo.Where(p => p.Value.connectionId == groupname))//disconnect all members in group
            {
                Groups.RemoveFromGroupAsync(item.Key, groupname);
                Clients.Client(item.Key).SendAsync("Disconnect");
            }
            if (anotheruserId != null) userRepo.Remove(anotheruserId);
            userRepo.Remove(Context.ConnectionId);
            userGroupRepo.Remove(groupname);


            return base.OnDisconnectedAsync(exception);
        }


    }
}
