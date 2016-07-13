using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEAT.NeuralNetworks
{
    public class OutputNeuron : INeuralOutputNode<double>
    {
        private SynapseCollection<double> _inputs = new SynapseCollection<double>();
        public ISynapse<double>[] Inputs
        {
            get { return _inputs.ToArray(); }
        }
        public double Bias { get; private set; }
        public IActivationFunction<double> ActivationFunction { get; private set; }
        public int NodeNumber { get; private set; }

        public double OutputValue
        {
            get { return /*_inputs.Sum(i => i.OutputValue + Bias); }//*/ActivationFunc(_inputs.Sum(i => i.OutputValue + Bias)); }
        }

        public OutputNeuron(int number, double bias, IActivationFunction<double> function)
        {
            this.NodeNumber = number;
            this.Bias = bias;
            this.ActivationFunction = function;
        }

        public ISynapse<double> AddInput(INeuralInputNode<double> neuron, double weight)
        {
            return new Synapse(neuron, this, weight);
        }
        public void AddInput(ISynapse<double> s)
        {
            _inputs.Add(s);
        }

        public double ActivationFunc(double g)
        {
            return ActivationFunction.GetValue(g);
        }
    }
}
