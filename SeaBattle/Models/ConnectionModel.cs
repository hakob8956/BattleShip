using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeaBattle.Models
{
    public class ConnectionModel
    {
        public string connectionId { get; set; }
        public int[,] field { get; set; }
    }
}
