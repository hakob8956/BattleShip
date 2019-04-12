using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GameCore
{
    public class GeneralFunctions
    {
        private int GetValue(int value)
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
            return value;
        }
        public int[,] Shoot(int[,] field, int x, int y)//return fieldType
        {
            if (Field.CheckLocation(x, y))
            {
                int value = GetValue(field[y, x]);//Get current Value
                field[y, x] = value;
                if (value == (int)FieldType.Shooted)
                {
                    int statusShip = 1;
                    int[] xx = new int[8], yy = new int[8];
                    xx[0] = x + 1; yy[0] = y + 1;
                    xx[1] = x - 1; yy[1] = y + 1;
                    xx[2] = x + 1; yy[2] = y - 1;
                    xx[3] = x - 1; yy[3] = y - 1;
                    xx[4] = x; yy[4] = y + 1;
                    xx[5] = x + 1; yy[5] = y;
                    xx[6] = x - 1; yy[6] = y;
                    xx[7] = x; yy[7] = y - 1;
                    for (int i = 0; i < 8; i++)
                    {
                        if (Field.CheckLocation(xx[i], yy[i]))
                        {
                            if (field[yy[i], xx[i]] == (int)FieldType.Used)
                            {
                                statusShip = 2;//
                                break;
                            }
                            else if (field[yy[i], xx[i]] == (int)FieldType.Shooted)
                            {
                                statusShip = 3;
                            }
                        }
                    }
                    if (statusShip == 1)//OnDecker
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            if (Field.CheckLocation(xx[i], yy[i]))
                            {
                                field[yy[i], xx[i]] = (int)FieldType.Missed;
                            }
                        }
                    }
                    else if (statusShip == 2)//hurt
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            if (Field.CheckLocation(xx[i], yy[i]))
                            {
                                field[yy[i], xx[i]] = (int)FieldType.Missed;
                            }
                        }

                    }
                    else if (statusShip == 3)//last ship
                    {

                    }

                }
            }
            return field;
        }
        public bool Win(int[,] field)
        {
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
