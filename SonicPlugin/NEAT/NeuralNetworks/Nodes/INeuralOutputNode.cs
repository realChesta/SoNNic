using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEAT.NeuralNetworks
{
    public interface INeuralOutputNode<T> : INeuralNode<T>
    {
        ISynapse<T>[] Inputs { get; }

        void AddInput(ISynapse<T> synapse);
        ISynapse<T> AddInput(INeuralInputNode<T> neuron, T weight);
        IActivationFunction<T> ActivationFunction { get; }

        T OutputValue { get; }
    }
}
