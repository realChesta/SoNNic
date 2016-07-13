using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEAT.Genetics
{
    public class RemoveConnectionMutationInfo : IMutationInfo
    {
        public RemoveConnectionMutationInfo()
            : this(EvolutionController.SpeciesParameters.DefaultRemoveConnectionMutationProbability)
        { }
        public RemoveConnectionMutationInfo(double probability)
        {
            this.MutationType = Genome.MutationType.RemoveConnection;
            this.Probability = probability;
        }

        public Genome.MutationType MutationType { get; private set; }

        public double Probability { get; set; }

        public override string ToString()
        {
            return "T:" + this.MutationType.ToString() + " P:" + this.Probability.ToString();
        }
    }
}
