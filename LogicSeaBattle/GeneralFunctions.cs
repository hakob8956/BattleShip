using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameCore
{
    public class GeneralFunctions
    {

        public int Shoot(int value, int x, int y)//return fieldType
        {
            if (x >= 0 && y >= 0 && x < 10 && y < 10)
            {
               switch (value)
                {
                    case (int)GameCore.FieldType.Empty:
                        value = (int)GameCore.FieldType.Missed;
                        break;
                    case (int)GameCore.FieldType.Used:
                        value = (int)GameCore.FieldType.Shooted;
                        break;
                    default:
                        break;
                }
            }
            return value;
        }
        public bool Win(int[,] field)
        {
            //For Optimiz.  (FieldType.Used = sum(ships * count));
            for (int i = 0; i < Field.Size; i++)
            {
                for (int j = 0; j < Field.Size; j++)
                {
                    if (field[i, j] == (int)GameCore.FieldType.Used)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
