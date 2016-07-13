using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEAT.NeuralNetworks
{
    public class ManualNeuron : INeuralHiddenNode<double>
    {
        private SynapseCollection<double> _inputs = new SynapseCollection<double>();
        public ISynapse<double>[] Inputs
        {
            get { return _inputs.ToArray(); }
        }

        private SynapseCollection<double> _outputs = new SynapseCollection<double>();
        public ISynapse<double>[] Outputs
        {
            get { return _outputs.ToArray(); }
        }

        public double Bias { get; set; }

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

        public IActivationFunction<double> ActivationFunction { get; private set; }

        public int NodeNumber { get; private set; }

        public double OutputValue
        {
            get;
            //{
            //    return ActivationFunc(_inputValue + Bias);
            //}
            set;
        }

        public double InputSum
        {
            get
            {
                return _inputs.Sum(s => s.OutputValue);
            }
        }

        public ManualNeuron(int number, double bias, IActivationFunction<double> function)
        {
            this.ActivationFunction = function;
            this.NodeNumber = number;
            this.Bias = bias;
        }

        public ISynapse<double> AddOutput(INeuralOutputNode<double> neuron, double weight)
        {
            return new Synapse(this, neuron, weight);
        }
        public void AddOutput(ISynapse<double> s)
        {
            _outputs.Add(s);
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
