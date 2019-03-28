using GameCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeaBattle.Models
{
    public class MainFunctionsGame
    {
        public Field fieldPlayer;
        public GeneralFunctions functionsPlayer;
        public Field fieldViewEnemy;
        public GeneralFunctions functionViewEnemy;
        public MainFunctionsGame()
        {
            fieldPlayer = new Field();
            functionsPlayer = new GeneralFunctions(fieldPlayer);
            fieldViewEnemy = new Field();
            functionViewEnemy = new GeneralFunctions(fieldViewEnemy);

        }
        //public int[,] GetPlans =Field.Plans;
        //public int GetSizePlans = Field.Size;
    }
}
