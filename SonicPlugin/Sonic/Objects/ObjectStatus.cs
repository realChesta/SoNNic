using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SonicPlugin.Sonic
{
    public class ObjectStatus
    {
        public OrientationType Orientation;
        public bool UpsideDown;

        public bool SonicIsStandingOnMe;
        public bool SonicIsPushingMe;

        public ObjectStatus(byte b)
        {
            this.Orientation = b.GetBit(0) ? OrientationType.Right : OrientationType.Left;
            this.UpsideDown = b.GetBit(1);
            this.SonicIsStandingOnMe = b.GetBit(3);
            this.SonicIsPushingMe = b.GetBit(5);
        }

        public enum OrientationType
        { 
            Left,
            Right
        }
    }
}
