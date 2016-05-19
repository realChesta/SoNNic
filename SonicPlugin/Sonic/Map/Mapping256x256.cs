using BizHawk.Emulation.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SonicPlugin.Sonic.Map
{
    public class Mapping256x256
    {
        public byte Index;

        public bool IsLoop;
        public Mapping256x256 LoopChunk;

        public Mapping16x16[][] Chunks;

        public Mapping256x256(MemoryDomain memory, byte index, long address, bool loop)
        {
            this.Index = index;
            this.IsLoop = loop;

            if (index == 0x00) //Check for 00-Chunk (empty chunk)
            {
                //Create empty chunk
                Chunks = new Mapping16x16[16][];
                for (int i = 0; i < Chunks.Length; i++)
                {
                    Chunks[i] = new Mapping16x16[16];

                    for (int j = 0; j < Chunks[i].Length; j++)
                    {
                        Chunks[i][j] = Mapping16x16.EmptyMapping;
                    }
                }
            }
            else
            {
                //LogForm log = new LogForm("MemChunk 0x" + this.Index.ToString("X2"));
                //log.Show();

                //log.WriteLine("MemPos: 0x" + memPos.ToString("X2"));

                Chunks = new Mapping16x16[16][];

                int o = 0;
                for (int i = 0; i < Chunks.Length; i++)
                {
                    Chunks[i] = new Mapping16x16[16];

                    for (int j = 0; j < Chunks[i].Length; j++)
                    {
                        //Read the two respective bytes of each 16x16 chunk and increment overall counter by two
                        //log.WriteLine("pos of [" + i + "][" + j + "]: 0x" + (memPos + o).ToString("X2"));

                        //if (value != 0)
                        //{
                            //bool[] values = new bool[16];
                            //for (int k = 0; k < 16; k++)
                            //{
                            //    values[k] = value.GetBit(k);
                            //}
                            //System.Windows.Forms.MessageBox.Show("{ " + string.Join(", ", values.Select(v => v ? "1" : "0")) + " }", "values @ 0x" + (address + o).ToString("X2") + " (chunk: 0x" + address.ToString("X2") + ")");

                            Chunks[i][j] = new Mapping16x16(memory.PeekWord(address + o, true));
                            o += 2;
                        //}
                    }
                }

                //log.Close();
                //log.Dispose();
            }
        }
    }
}
