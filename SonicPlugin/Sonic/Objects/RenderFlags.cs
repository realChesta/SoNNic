using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SonicPlugin.Sonic
{
    public class RenderFlags
    {
        public readonly bool HorizontalMirror;
        public readonly bool VerticalMirror;
        public readonly bool AssumeHeight;
        public readonly bool RawMappings;
        public readonly bool SonicRideB;
        public readonly bool OnScreen;

        public RenderFlags(byte b)
        {
            this.HorizontalMirror = b.GetBit(0);
            this.VerticalMirror = b.GetBit(1);
            this.AssumeHeight = b.GetBit(4);
            this.RawMappings = b.GetBit(5);
            this.SonicRideB = b.GetBit(6);
            this.OnScreen = b.GetBit(7);
        }
    }
}
