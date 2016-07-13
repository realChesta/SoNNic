using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEAT.NeuralNetworks
{
    public interface IActivationFunction<T>
    {
        double GetValue(T x);

        string ToString();
    }
}
