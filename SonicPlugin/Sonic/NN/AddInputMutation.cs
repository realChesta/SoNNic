using NEAT.Genetics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SonicPlugin.Sonic.NN
{
    public class AddInputMutation : IMutationInfo
    {
        public AddInputMutation(double probability)
        {
            this.Probability = probability;
        }

        public Genome.MutationType MutationType
        {
            get
            {
                return Genome.MutationType.AddInput;
            }
        }

        public double Probability { get; set; }

        public override string ToString()
        {
            return "T:" + this.MutationType.ToString() + " P:" + this.Probability.ToString();
        }
    }
}
