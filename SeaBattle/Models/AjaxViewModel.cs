using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeaBattle.Models
{
    public class AjaxViewModel
    {
        public int x { get; set; }
        public int y { get; set; }
        public int[,] field { get; set; }
        public string connectionId { get; set; }
    }
}
