using GameCore;
using System;

namespace TestGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Field field = new Field();
            GeneralFunctions functions = new GeneralFunctions();
            field.SetRandomShips();
            field.Display();
            while (!functions.Win())
            {

            
            int x = int.Parse(Console.ReadLine());
            int y = int.Parse(Console.ReadLine());
            functions.Shoot(x,y);
            field.Display();
             
            }


            //Console.WriteLine();
            //field.Display();
        }
    }
}
