using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace NEAT.NeuralNetworks
{
    public class InputNeuron : INeuralInputNode<double>
    {
        private SynapseCollection<double> _outputs = new SynapseCollection<double>();

        public ISynapse<double>[] Outputs
        {
            get { return _outputs.ToArray(); }
        }

        public int NodeNumber { get; private set; }

        public Point Position;
        public Size Size;

        public InputNeuron(int number)
        {
            this.NodeNumber = number;
        }

        public ISynapse<double> AddOutput(INeuralOutputNode<double> neuron, double weight)
        {
            return new Synapse(this, neuron, weight);
        }
        public void AddOutput(ISynapse<double> s)
        {
            _outputs.Add(s);
        }

        public double OutputValue
        {
            get { return _inputValue; }
        }

        private double _inputValue;
        public double InputValue
        {
            get
            {
                return _inputValue;
            }
            set
            {
                _inputValue = value;
            }
        }
    }
}
