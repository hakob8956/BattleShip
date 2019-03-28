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
            int count = 0;
            Random random = new Random();
            while (true)
            {
                field.ClearField();
                //int x = random.Next(0, 9);
                //int y = random.Next(0, 9);
                //int or = random.Next(0, 1);
                //field.SetOneShip(x,y,1,or);
                field.SetRandomShips();
                field.Display();
                
                for (int i = 0; i < Field.Size; i++)
                {
                    for (int j = 0; j < Field.Size; j++)
                    {
                        if (field.Plans[i,j]==1)
                        {
                            count++;
                        }
                    }
                    
                }
                Console.WriteLine();
                Console.WriteLine("Count = " + count);
                count = 0;
                Console.ReadKey();

            }







            //Console.WriteLine();
            //field.Display();
        }
    }
}
