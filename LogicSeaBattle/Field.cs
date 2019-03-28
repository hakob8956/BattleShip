using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameCore
{
    public class Field
    {
        public static int Size { get; } = 10;
        private int[,] plans = new int[Size, Size];
        public int[,] Plans
        {//check for  sec.
            get { return plans; }
            set { plans = value; }
        }
        public Field()
        {
            plans = new int[Size, Size];
        }
        public void SetRandomShips()
        {
            //1->4
            //2->3     //4+6+6+4=20
            //3->2
            //4->1
            int[] array = new int[3];//{1,2,3,4}
            Random random = new Random();

            int count = 0;

            //for one more for accept 20 ships(100%)
            //Afraid Deadlock
            //while (CountAllShips() != 20)
            //{
            //    ClearField();
            for (int j = 1; j <= array.Length; j++) { array[j] = j; }//Init Array

            for (int i = 0; i < array.Length; i++)
            {
                while (array[i] != 0)
                {
                    int x = random.Next(0, 9);
                    int y = random.Next(0, 9);
                    int or = random.Next(0, 1);
                    if (SetOneShip(x, y, array.Length - i, or))
                    {
                        count++;
                        array[i]--;

                    }

                }
                Console.WriteLine(count);
                //count = 0;
            }

            //            }



        }
        private int CountAllShips()
        {
            int count = 0;
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    if (plans[i, j] == (int)FieldType.Used)
                    {
                        count++;
                    }
                }
            }
            return count;

        }
        public bool SetOneShip(int x, int y, int ShipType, int orentation = (int)Orentation.Horizontal)
        {
            if (!CheckLocation(x, y))//Check Loacation x and y,if out array return false
                return false;
            int xx = x, yy = y;
            if (orentation == (int)(Orentation.Vertical))
            {
                bool up = true, down = true;
                int moveUp = 0, moveDown = 0;
                if (yy - ShipType < 0)
                    up = false;
                if (yy + ShipType > Size)
                    down = false;
                //Vertical Rigth Or Up

                for (int i = 0; i < ShipType; i++)
                {
                    moveUp = yy - i;
                    moveDown = yy + i;
                    if (up == true && plans[moveUp, xx] != (int)FieldType.Empty)
                        up = false;

                    if (down == true && plans[moveDown, xx] != (int)FieldType.Empty)
                        down = false;

                }
                if (up)
                {
                    for (int i = 0; i < ShipType; i++)
                    {

                        plans[yy - i, xx] = (int)FieldType.Used;
                    }
                }
                else if (down)
                {
                    for (int j = 0; j < ShipType; j++)
                    {
                        plans[yy + j, yy] = (int)FieldType.Used;
                    }
                }
                return up || down;


            }
            else if (orentation == (int)Orentation.Horizontal)
            {

                bool right = true, left = true;
                if (xx + ShipType > Size)
                    right = false;
                if (xx - ShipType < 0)
                    left = false;

                int moveRigth = 0, moveLeft = 0;
                //Horizontal Up Or Down
                for (int j = 0; j < ShipType; j++)
                {
                    moveRigth = xx + j;
                    moveLeft = xx - j;

                    if (right == true && plans[yy, moveRigth] != (int)FieldType.Empty)
                        right = false;

                    if (left == true && plans[yy, moveLeft] != (int)FieldType.Empty)
                        left = false;
                }
                if (right)
                {
                    for (int i = 0; i < ShipType; i++)
                    {
                        plans[yy, xx + i] = (int)FieldType.Used;
                    }
                }
                else if (left)
                {
                    for (int j = 0; j < ShipType; j++)
                    {
                        plans[yy, xx - j] = (int)FieldType.Used;
                    }
                }
                return right || left;
            }

            return false;
        }
        public bool CheckLocation(int x, int y)
        {
            if (!(x >= 0 && y >= 0 && x < Size && y < Size))
                return false;

            else
                return true;

        }
        //public void SetNeutralZone(int x,int y)
        //{
        //    if (true)
        //    {

        //    }

        //}
        public void ClearField()
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    plans[i, j] = 0;
                }
            }
        }
        public void Display()
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    Console.Write(plans[i, j] + "  ");
                }
                Console.WriteLine();
            }
        }

    }
}
