using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analyzer
{
    public class Subject
    {
        public readonly int Fitness;
        public readonly TimeSpan Runtime;
        public readonly string NeuralNetwork;

        public Subject(int fitness, double seconds, string network)
        {
            this.Fitness = fitness;
            this.Runtime = TimeSpan.FromSeconds(seconds);
            this.NeuralNetwork = network;
        }
    }
}
