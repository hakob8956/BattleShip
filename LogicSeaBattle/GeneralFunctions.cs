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

        public (int[,], int) Shoot(int[,] field, int x, int y)//return fieldType
        {
            int countKill = 0;
            if (Field.CheckLocation(x, y))
            {

                Field.ArroundCords arround = new Field.ArroundCords(x, y);
                int dirX = 0, dirY = 0;
                int value = GetValue(field[y, x]);//Get current Value
                field[y, x] = value;
                if (value == (int)FieldType.Shooted)
                {
                    
                    int statusShip = 1;

                    for (int i = 0; i < 8; i++)
                    {
                        if (Field.CheckLocation(arround.xx[i], arround.yy[i]))
                        {
                            if (field[arround.yy[i], arround.xx[i]] == (int)FieldType.Used)
                            {
                                statusShip = 2;//
                                break;
                            }
                            else if (field[arround.yy[i], arround.xx[i]] == (int)FieldType.Shooted)
                            {
                                statusShip = 3;
                                dirX = arround.xx[i];
                                dirY = arround.yy[i];
                            }
                        }
                    }
                    if (statusShip == 1)//OnDecker
                    {
                        countKill++;
                        for (int i = 0; i < 8; i++)
                        {
                            if (Field.CheckLocation(arround.xx[i], arround.yy[i]))
                            {
                                field[arround.yy[i], arround.xx[i]] = (int)FieldType.Missed;
                            }
                        }
                    }
                    else if (statusShip == 2)//hurt
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            if (Field.CheckLocation(arround.xx[i], arround.yy[i]))
                            {
                                field[arround.yy[i], arround.xx[i]] = (int)FieldType.Missed;
                            }
                        }

                    }
                    else if (statusShip == 3)//last ship
                    {

                        int XD = x - dirX;
                        int YD = y - dirY;
                        countKill = 0;
                        while (Field.CheckLocation(x, y) && field[y, x] == (int)FieldType.Shooted)
                        {
                            countKill++;
                            arround = new Field.ArroundCords(x, y);
                            for (int i = 0; i < 8; i++)
                            {
                                if (Field.CheckLocation(arround.xx[i], arround.yy[i]))
                                {
                                    if (field[arround.yy[i], arround.xx[i]] == (int)FieldType.Empty)
                                    {
                                        field[arround.yy[i], arround.xx[i]] = (int)FieldType.Missed;
                                    }
                                }
                            }
                            x -= XD;
                            y -= YD;
                        }


                    }
                }

            }
            return (field, countKill);
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
