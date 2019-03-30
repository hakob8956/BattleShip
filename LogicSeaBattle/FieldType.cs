using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore
{
   public enum FieldType
    {
        Empty,
        Used,
        Missed = -1,
        Neutral = -2,
        Shooted = 2
    }
}
