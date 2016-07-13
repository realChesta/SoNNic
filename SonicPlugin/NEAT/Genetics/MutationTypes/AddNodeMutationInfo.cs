using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEAT.Genetics
{
    public class AddNodeMutationInfo : IMutationInfo
    {
        public AddNodeMutationInfo()
            : this(EvolutionController.SpeciesParameters.DefaultAddNodeMutationProbability)
        { }
        public AddNodeMutationInfo(double probability)
        {
            this.MutationType = Genome.MutationType.Node;
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
