using GameCore;
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

        public IActionResult Index(string id = null)
        {
            Field field = new Field();
            field.SetRandomShips();
            DateViewModel model = new DateViewModel { fieldPlayer = field.Plans, ConnectionID = id };
            return View(model);
        }


    }
}
