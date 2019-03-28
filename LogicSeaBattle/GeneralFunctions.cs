using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GameCore
{
    public class GeneralFunctions
    {
        Field field;
        public GeneralFunctions(Field _field)
        {
            field = _field;
        }

        public void Shoot(int x,int y)
        {
            if (field.CheckLocation(x, y))
            {
                int status=field.Plans[x,y];
                switch (status)
                {
                    case (int)GameCore.FieldType.Empty:
                        status = (int)GameCore.FieldType.Missed;
                        break;
                    case (int)GameCore.FieldType.Used:
                        status = (int)GameCore.FieldType.Shooted;
                        break;                 
                    default:
                        break;
                }
                field.Plans[x, y] = status;
            }
        }
        public bool Win()
        {
            //For Optimiz.  (FieldType.Used = sum(ships * count));
            for (int i = 0; i < Field.Size; i++)
            {
                for (int j = 0; j < Field.Size; j++)
                {
                    if (field.Plans[i,j] == (int)GameCore.FieldType.Used)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
