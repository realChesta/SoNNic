using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NEAT;
using NEAT.Genetics;

namespace SonicPlugin.NEAT.Genetics.MutationTypes
{
    public class AddInputMutation : IMutationInfo
    {
        public AddInputMutation()
            : this(EvolutionController.SpeciesParameters.DefaultAddNodeMutationProbability)
        { }
        public AddInputMutation(double probability)
        {
            this.MutationType = Genome.MutationType.AddInput;
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
