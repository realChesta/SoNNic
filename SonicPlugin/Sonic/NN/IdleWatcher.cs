namespace SonicPlugin.Sonic.NN
{
    public class IdleWatcher
    {
        private int idleFrames;
        private double lastFitness;
        public readonly int Limit;

        public IdleWatcher(int limit)
        {
            this.Limit = limit;
            this.lastFitness = -1;
            this.idleFrames = 0;
        }

        /// <summary>
        /// Returns true if limit has been reached.
        /// </summary>
        /// <param name="fitness"></param>
        /// <returns></returns>
        public bool Next(double fitness)
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
