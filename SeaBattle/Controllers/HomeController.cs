using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SeaBattle.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SeaBattle.Controllers
{
    public class HomeController : Controller
    {
        IHubContext<MainHub> hubContext;
        public HomeController(IHubContext<MainHub> hubContext)
        {
            this.hubContext = hubContext;
        }
        MainFunctionsGame FunctionsGame = new MainFunctionsGame();
        public IActionResult Index(string id = null)
        {
            FunctionsGame.fieldPlayer.ClearField();
            FunctionsGame.fieldPlayer.SetRandomShips();

            FunctionsGame.fieldViewEnemy.ClearField();
            FunctionsGame.fieldViewEnemy.SetRandomShips();
            DateViewModel model = new DateViewModel { FunctionsGame = FunctionsGame, ConnectionID = id };
            return View(model);
        }

    }
}
