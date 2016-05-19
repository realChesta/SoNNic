using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SonicPlugin.Sonic.Map
{
    public class Mapping16x16
    {
        public SolidityStatus Solidity;
        public ushort BlockReferenceID;
        public bool HorizontalFlip;
        public bool VerticalFlip;

        public Mapping16x16(ushort value)
        {
            if (value != 0)
            {
                this.Solidity = Utils.GetSolidityStatus(value.GetBit(0xD), value.GetBit(0xE));
                this.HorizontalFlip = value.GetBit(0xB);
                this.VerticalFlip = value.GetBit(0xC);
                this.BlockReferenceID = (ushort)(value & 0x3FF); //Get last 10 bits
            }
        }

        private static Mapping16x16 _emptyMapping = new Mapping16x16(0);
        public static Mapping16x16 EmptyMapping
        {
            get
            { return _emptyMapping; }
        }
    }
}
