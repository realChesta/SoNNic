using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEAT.NeuralNetworks
{
    public interface INeuralInputNode<T> : INeuralNode<T>
    {
        ISynapse<T>[] Outputs { get; }

        void AddOutput(ISynapse<T> synapse);
        ISynapse<T> AddOutput(INeuralOutputNode<T> neuron, T weight);

        T InputValue { set; get; }
    }
}
