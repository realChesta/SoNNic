using System.Collections.Generic;
using System.Drawing;
using NEAT.Genetics;
using NEAT.NeuralNetworks;
using SonicPlugin.Sonic.Map;

namespace SonicPlugin.Sonic.NN
{
    public class LivingSonic
    {
        public const int Height = 40;
        public const int Width = 20;

        private ManualNeuralNetwork Brain;
        private Dictionary<int, WorldInput> Inputs; 

        public LivingSonic(ref MapDrawer map, int sensorSize, int sensorRangeX, int sensorRangeY)
        {
            //TODO: create neural network
            //TODO: then start implementing steps, fitness, etc.

            Size size = new Size(sensorSize, sensorSize);
            for (int i = 0; i < Brain.Inputs.Length; i++)
            {
                WorldInput input = new WorldInput(
                    ref map, 
                    new Point(
                        Utils.Random.Next(-sensorRangeX, sensorRangeX), 
                        Utils.Random.Next(-sensorRangeY, sensorRangeY)), 
                    size);
                Inputs.Add(Brain.Inputs[i].NodeNumber, input);
            }
        }
    }
}
