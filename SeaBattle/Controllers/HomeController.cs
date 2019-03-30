using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SeaBattle.Models;

namespace SeaBattle.Controllers
{
    public class HomeController : Controller
    {
        MainFunctionsGame FunctionsGame = new MainFunctionsGame();
        public IActionResult Index()
        {
            FunctionsGame.fieldPlayer.ClearField();
            FunctionsGame.fieldPlayer.SetRandomShips();

            FunctionsGame.fieldViewEnemy.ClearField();
            FunctionsGame.fieldViewEnemy.SetRandomShips();

            return View(FunctionsGame);
        }

    }
}
