using System.Collections.Generic;
using NEAT.NeuralNetworks;

namespace SonicPlugin.Sonic.NN
{
    public class LivingSonic
    {
        public const int Height = 40;
        public const int Width = 20;

        private ManualNeuralNetwork Brain;
        private Dictionary<int, WorldInput> Inputs; 

        public LivingSonic()
        {
            
        }
    }
}
