namespace SonicPlugin.Sonic.NN
{
    public class IdleWatcher
    {
        private int idleFrames;
        private double lastFitness;
        private double bestFitness;
        public readonly int Limit;
        private bool oneFrameTolerance;

        public int IdleFrames { get { return idleFrames; } }

        public IdleWatcher(int limit)
        {
            this.Limit = limit;
            this.lastFitness = -1;
            this.bestFitness = -1;
            this.idleFrames = 0;
        }

        /// <summary>
        /// Returns true if limit has been reached.
        /// </summary>
        /// <param name="fitness"></param>
        /// <returns></returns>
        public bool Next(double fitness)
        {
            if (fitness <= bestFitness)
            {
                if (lastFitness != -1)
                {
                    if (fitness <= lastFitness)
                    {
                        if (lastFitness == fitness)
                        {
                            if (oneFrameTolerance)
                                idleFrames++;
                            else
                                oneFrameTolerance = true;
                        }
                        else
                            idleFrames++;
                    }
                    else
                        oneFrameTolerance = false;
                }
            }
            else
            {
                idleFrames = 0;
                bestFitness = fitness;
                oneFrameTolerance = false;
            }

            lastFitness = fitness;

            //if ((lastFitness != -1) && (previousFitness != -1))
            //{
            //    if ((lastFitness < fitness) && (previousFitness < fitness) && (fitness > minFitness))
            //        idleFrames = 0;
            //    else
            //        idleFrames++;
            //}

            //if (minFitness == -1 || minFitness > fitness)
            //    minFitness = fitness;

            //previousFitness = lastFitness;
            //lastFitness = fitness;

            if (idleFrames >= Limit)
            {
                idleFrames = 0;
                bestFitness = -1;
                lastFitness = -1;
                return true;
            }
            else return false;
        }
    }
}
