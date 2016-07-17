using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;

namespace SonicPlugin.Sonic.NN
{
    public class IdleWatcher
    {
        private int idleFrames;
        private int lastFitness;
        public readonly int Limit;

        public IdleWatcher(int limit)
        {
            this.Limit = limit;
            this.lastFitness = -1;
            this.idleFrames = 0;
        }

        public bool Next(int fitness)
        {
            if (lastFitness != -1)
            {
                if (lastFitness != fitness)
                    idleFrames = 0;
                else
                    idleFrames++;
            }

            lastFitness = fitness;

            return idleFrames >= Limit;
        }
    }
}
