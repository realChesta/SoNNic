using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SonicPlugin.Sonic
{
    //Incomplete implementation
    public class ArtTile
    {
        //drawn normally when false
        public readonly bool VerticalFlip;
        //drawn on low plane when false
        public readonly bool HighPlaneDraw;

        public ArtTile(byte[] b)
        {
            if (b.Length != 2)
                throw new InvalidOperationException("Art tile field must be 2 bytes long!");

            this.VerticalFlip = b[1].GetBit(4);
            this.HighPlaneDraw = b[1].GetBit(7);
        }
    }
}
