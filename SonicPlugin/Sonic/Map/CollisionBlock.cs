using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SonicPlugin.Sonic.Map
{
    public class CollisionBlock
    {
        public byte[] data;

        public CollisionBlock(byte[] data)
        {
            if (data.Length != 0x10)
                throw new InvalidOperationException("Invalid collision data size!");

            this.data = data;
        }

        //FORMAT:
        // X = solid
        // - = air/non-solid

        // 0 0 0 0 0 0 0-1-2 3 1 2 2 3 4 6
        // - - - - - - - X X - - - - - - -
        // - - - - - - - - X - - - - - - -
        // - - - - - - - - - - - - - - - -
        // - - - - - - - - - - - - - - - -
        // - - - - - - - - - - - - - - - -
        // - - - - - - - - - - - - - - - -
        // - - - - - - - - - - - - - - - -
        // - - - - - - - - - - - - - - - -
        // - - - - - - - - - - - - - - - -
        // - - - - - - - - - - - - - - - -
        // - - - - - - - - - - - - - - - X
        // - - - - - - - - - - - - - - - X
        // - - - - - - - - - - - - - - X X
        // - - - - - - - - - X - - - X X X
        // - - - - - - - - - X - X X X X X
        // - - - - - - - - - X X X X X X X
    }
}
