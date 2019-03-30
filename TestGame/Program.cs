using GameCore;
using System;

namespace TestGame
{
    class Program
    {
        static void Main(string[] args)
        {

            Field field = new Field();
            GeneralFunctions functions = new GeneralFunctions(field);

            field.SetRandomShips();
            field.Display();
            Console.WriteLine(field.CountAllShips());
        }
    }
}
