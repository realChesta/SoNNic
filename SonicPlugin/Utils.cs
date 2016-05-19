using SonicPlugin.Sonic;
using SonicPlugin.Sonic.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SonicPlugin
{
    public static class Utils
    {

        public static CollisionResponseType GetCollisionResponseType(byte cr)
        {
            if (cr == 0x00)
                return CollisionResponseType.Unknown;

            bool b0 = cr.GetBit(6);
            bool b1 = cr.GetBit(7);

            if (!b0 && !b1)
                return CollisionResponseType.Enemy;
            else if (!b0 && b1)
                return CollisionResponseType.Harm;
            else if (b0 && !b1)
                return CollisionResponseType.CounterIncrement;
            else if (b0 && b1)
                return CollisionResponseType.Special;
            else
                return CollisionResponseType.Unknown;
        }

        public static SolidityStatus GetSolidityStatus(bool b0, bool b1)
        {
            if (!b0 && !b1)
                return SolidityStatus.NonSolid;
            else if (!b0 && b1)
                return SolidityStatus.TopSolid;
            else if (b0 && !b1)
                return SolidityStatus.LeftRightBottomSolid;
            else if (b0 && b1)
                return SolidityStatus.AllSolid;
            else
                return SolidityStatus.Unknown;
        }

        #region Extensions

        public static bool GetBit(this byte b, int index)
        {
            if ((index >= 0) && (index < 8))
                return (b & (1 << index)) != 0;
            else
                throw new IndexOutOfRangeException();
        }
        public static bool GetBit(this ushort b, int index)
        {
            if ((index >= 0) && (index < 16))
                return (b & (1 << index)) != 0;
            else
                throw new IndexOutOfRangeException();
        }

        #endregion
    }
}
