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
        public int CountAllShips(int [,] field)
        {
            int count = 0;
            for (int i = 0; i < Field.Size; i++)
            {
                for (int j = 0; j < Field.Size; j++)
                {
                    if (field[i, j] == (int)FieldType.Used)
                        count++;
                }
            }
            return count;

        }
        public (int[,], int) Shoot(int[,] field, int x, int y)//return fieldType
        {
            int countKill = 0;
            if (Field.CheckLocation(x, y))
            {

                Field.ArroundCords arround = new Field.ArroundCords(x, y);
                int dirX = 0, dirY = 0;
                int XD = 0, YD = 0;
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
                    if (statusShip == 3)
                    {
                        XD = x - dirX;
                        YD = y - dirY;
                        statusShip = CheckHurtShip(field, x, y, XD, YD) ? 2 : 3;
                    }
                    if (statusShip == 1)//OnDecker
                    {
                        countKill = 1;
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
                        countKill = 0;
                        int x1 = x, y1 = y;
                        //#1
                        while (Field.CheckLocation(x1, y1) && field[y1, x1] == (int)FieldType.Shooted)
                        {
                            countKill++;
                            arround = new Field.ArroundCords(x1, y1);
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
                            x1 -= XD;
                            y1 -= YD;
                        }
                        x1 = x;
                        y1 = y;
                        //#2
                        countKill--;
                        while (Field.CheckLocation(x1, y1) && field[y1, x1] == (int)FieldType.Shooted)
                        {
                            countKill++;
                            arround = new Field.ArroundCords(x1, y1);
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
                            x1 += XD;
                            y1 += YD;
                        }
                    }
                }

            }
            return (field, countKill);
        }

        public int[,] DeleteShip(int[,] field, int x, int y)
        {
            int dirX = 0;
            int dirY = 0;
            int XD = 0;
            int YD = 0;
            //field[y, x] = (int)FieldType.Empty;
            Field.ArroundCords arround = new Field.ArroundCords(x, y);
            for (int i = 0; i < 8; i++)
            {
                if (Field.CheckLocation(arround.xx[i],arround.yy[i]) &&  field[arround.yy[i],arround.xx[i]] == (int)FieldType.Used)
                {


                    dirX = arround.xx[i];
                    dirY = arround.yy[i];
                    break;
                }
            }
            XD = x - dirX;
            YD = y - dirY;
            while (Field.CheckLocation(x, y) && field[y, x] == (int)FieldType.Used)
            {
                field[y, x] = (int)FieldType.Empty;
                x -= XD;
                y -= YD;
            }
            return field;

        }
        public bool CheckHurtShip(int[,] field, int x, int y, int XD, int YD)
        {
            while (Field.CheckLocation(x, y) && field[y, x] == (int)FieldType.Shooted)
            {
                x -= XD;
                y -= YD;
            }
            if (Field.CheckLocation(y, x))
            {
                if (field[y, x] == (int)FieldType.Used)
                {
                    return true;
                }
            }

            return false;
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
