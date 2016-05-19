using BizHawk.Emulation.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SonicPlugin.Sonic.Map
{
    public class CollisionMap
    {
        public MemoryDomain ROMmemory;
        public CollisionMapMode Mode;

        public const long CollisionArrayOffset = 0x062A00;
        public const long CollisionArrayEnd = 0x0639FF;

        public readonly uint CollisionTablePointer;

        public CollisionMap(MemoryDomain romMemory, CollisionMapMode mode)
            : this(romMemory, romMemory.PeekDWord((long)mode, true))
        { }
        public CollisionMap(MemoryDomain romMemory, uint tablePointer)
        {
            this.CollisionTablePointer = tablePointer;
            this.ROMmemory = romMemory;
        }

        public byte GetCollisionID(ushort blockReferenceID)
        {
            if (blockReferenceID == 0x00)
                return 0;

            return ROMmemory.PeekByte(CollisionTablePointer + blockReferenceID);
        }

        public CollisionBlock GetCollisionBlock(ushort blockReferenceID)
        {
            return GetCollisionBlock(GetCollisionID(blockReferenceID));
        }
        public CollisionBlock GetCollisionBlock(byte collisionID)
        {
            if (collisionID == 0x00)
                return new CollisionBlock(new byte[0x10]);

            long memPos = CollisionArrayOffset + (collisionID * 0x10);

            byte[] data = new byte[] //read 0x10 (16) bytes
            {
                ROMmemory.PeekByte(memPos++), ROMmemory.PeekByte(memPos++), 
                ROMmemory.PeekByte(memPos++), ROMmemory.PeekByte(memPos++), 
                ROMmemory.PeekByte(memPos++), ROMmemory.PeekByte(memPos++), 
                ROMmemory.PeekByte(memPos++), ROMmemory.PeekByte(memPos++), 
                ROMmemory.PeekByte(memPos++), ROMmemory.PeekByte(memPos++), 
                ROMmemory.PeekByte(memPos++), ROMmemory.PeekByte(memPos++), 
                ROMmemory.PeekByte(memPos++), ROMmemory.PeekByte(memPos++), 
                ROMmemory.PeekByte(memPos++), ROMmemory.PeekByte(memPos++)
            };

            return new CollisionBlock(data);
        }
    }
}
