using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEAT.NeuralNetworks
{
    public interface INeuralHiddenNode<T> : INeuralInputNode<T>, INeuralOutputNode<T>
    {
        T Bias { get; }
    }
}