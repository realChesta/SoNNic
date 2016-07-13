using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEAT.NeuralNetworks
{
    public interface INeuralNode<T>
    {
        T OutputValue { get; }
        int NodeNumber { get; }
    }
}
