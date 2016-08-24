using BizHawk.Emulation.Common;
using SonicPlugin.Sonic.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SonicPlugin.Sonic
{
    public class SonicMap
    {
        public const long LayoutOffset = 0xA400;
        public const long LayoutEnd = 0xA7FF;

        public const long ChunkMappingOffset = 0x0000;
        public const long ChunkMappingEnd = 0xA3FF;

        public CollisionMap CollisionMap;
        
        public Mapping256x256[][] Chunks;
        public uint CollisionTablePointer = 0x00;

        public SonicMap(IMemoryDomains domains)
        {
            this.CollisionTablePointer = domains.MainMemory.PeekDWord(0xF796, true);
            this.CollisionMap = new Map.CollisionMap(domains["MD CART"], CollisionTablePointer);

            MemoryDomain memory = domains.MainMemory;

            Chunks = new Mapping256x256[8][]; //8 lines of Chunks

            for (int i = 0; i < Chunks.Length; i++)
            {
                Chunks[i] = new Mapping256x256[0x40]; //64 Chunks for each line
            }

            Dictionary<byte, Mapping256x256> knownChunks = new Dictionary<byte, Mapping256x256>(); //save known chunks to avoid rereading them from memory

            int ix = 0;
            int iy = 0;

            for (long i = LayoutOffset; i < LayoutEnd; i += 0x80) //0x80 to skip every second line (background-lines)
            {
                for (long j = i; j < (i + 0x40); j++) //read each index
                {
                    byte chunkIndex = memory.PeekByte(j);
                    bool isLoop = (chunkIndex & 0x80) == 0x80;
                    chunkIndex = (byte)(chunkIndex & 0x7F);
                    long address = (SonicMap.ChunkMappingOffset + ((chunkIndex - 1) * 0x200));

                    //if (isLoop)
                    //    log.WriteLine("Chunk is loop!");

                    if (address <= 0xA3FF)
                    {
                        //log.WriteLine("@" + j.ToString("X2") + ": Chunk " + chunkIndex + " @ 0x" + address.ToString("X2"));

                        if (!knownChunks.ContainsKey(chunkIndex))
                        {
                            knownChunks.Add(chunkIndex, new Mapping256x256(memory, chunkIndex, address, isLoop)); //only recreate if nessecary
                        }
                        Chunks[iy][ix] = knownChunks[chunkIndex];
                    }
                    else
                    {
                        //log.WriteLine("Warning: chunk reference is over chunk mapping limit! Chunk " + chunkIndex + " @ 0x" + address.ToString("X2") + " (reference @ 0x" + j.ToString("X2") + ")");
                        Chunks[iy][ix] = knownChunks[0x00];
                    }

                    ix++;
                }
                ix = 0;
                iy++;
            }

            knownChunks.Clear();
        }

        public static SonicMap ReadMap(IMemoryDomains domains)
        {
            return new SonicMap(domains);
        }
    }
}
