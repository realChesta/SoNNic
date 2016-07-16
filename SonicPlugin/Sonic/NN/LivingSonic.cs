using System.Collections.Generic;
using System.Drawing;
using NEAT.NeuralNetworks;
using SonicPlugin.Sonic.Map;
using NEAT.Genetics;

namespace SonicPlugin.Sonic.NN
{
    public class LivingSonic
    {
        public const int Height = 40;
        public const int Width = 20;

        public readonly Genome Genome;
        public ManualNeuralNetwork Brain;
        private Dictionary<int, WorldInput> Inputs;

        public double Fitness
        {
            get { return Genome.Fitness; }
            set { Genome.Fitness = value; }
        }

        public bool A => DtB(Brain.Outputs[1].OutputValue);

        public bool PadUp => DtB(Brain.Outputs[2].OutputValue);
        public bool PadDown => DtB(Brain.Outputs[3].OutputValue);
        public bool PadLeft => DtB(Brain.Outputs[4].OutputValue);
        public bool PadRight => DtB(Brain.Outputs[5].OutputValue);

        public LivingSonic(Genome genome, ref MapDrawer map, int sensorSize, int sensorRangeX, int sensorRangeY)
        {
            this.Genome = genome;
            this.Brain = genome.CreateManualNetwork();

            //TODO: then start implementing steps, fitness, etc.

            Inputs = new Dictionary<int, WorldInput>();
            Size size = new Size(sensorSize, sensorSize);
            for (int i = 1; i < Brain.Inputs.Length; i++)
            {
                WorldInput input = new WorldInput(
                    ref map,
                    new Point(
                        Utils.Random.Next(-sensorRangeX, sensorRangeX),
                        Utils.Random.Next(-sensorRangeY, sensorRangeY)),
                    size);
                Inputs.Add(i, input);
            }
        }

        //DoubleToBoolean
        private static bool DtB(double input)
        {
            return input >= 0.5D;
        }
         
        public void Step(Point sonic, SonicObject[] objects)
        {
            foreach (var kvp in Inputs)
            {
                Brain.Inputs[kvp.Key].InputValue = (int)kvp.Value.GetValue(sonic, objects);
            }

            Brain.RelaxNetwork(10, 0.1);
        }
    }
}