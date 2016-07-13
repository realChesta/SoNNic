using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEAT.NeuralNetworks
{
    public class Synapse : ISynapse<double>
    {
        public INeuralInputNode<double> InputNode { get; private set; }
        public INeuralOutputNode<double> OutputNode { get; private set; }
        public double Weight { get; private set; }

        public double OutputValue
        {
            get
            {
                return (InputNode.OutputValue * Weight);
            }
        }

        public Synapse(INeuralInputNode<double> input, INeuralOutputNode<double> output, double weight)
        {
            this.Weight = weight;

            this.InputNode = input;
            this.OutputNode = output;

            this.InputNode.AddOutput(this);
            this.OutputNode.AddInput(this);
        }
    }
}
