using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEAT.NeuralNetworks
{
    public interface INeuralNetwork<T>
    {
        INeuralInputNode<T>[] Inputs { get; }
        INeuralOutputNode<T>[] Outputs { get; }
        INeuralHiddenNode<T>[][] DeepNodes { get; }
    }
}
