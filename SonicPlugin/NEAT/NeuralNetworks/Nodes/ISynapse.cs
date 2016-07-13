using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEAT.NeuralNetworks
{
    public interface ISynapse<T>
    {
        INeuralInputNode<T> InputNode { get; }
        INeuralOutputNode<T> OutputNode { get; }
        T Weight { get; }
        T OutputValue { get; }
    }
}
