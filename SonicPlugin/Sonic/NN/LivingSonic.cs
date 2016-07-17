using System.Collections.Generic;
using System.Drawing;
using BizHawk.Client.Common;
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
        private Dictionary<int, WorldInput> Inputs = new Dictionary<int, WorldInput>();

        public double Fitness
        {
            get { return Genome.Fitness; }
            set { Genome.Fitness = value; }
        }

        public bool A => DtB(Brain.Outputs[0].OutputValue);

        public bool PadUp => DtB(Brain.Outputs[1].OutputValue);
        public bool PadDown => DtB(Brain.Outputs[2].OutputValue);
        public bool PadLeft => DtB(Brain.Outputs[3].OutputValue);
        public bool PadRight => DtB(Brain.Outputs[4].OutputValue);

        public LivingSonic(Genome genome, ref MapDrawer map, int sensorSize, int sensorRangeX, int sensorRangeY)
        {
            this.Genome = genome;
            this.Brain = genome.CreateManualNetwork();
            this.Brain.Inputs[0].InputValue = 1; //bias

            //TODO: then start implementing steps, fitness, etc.

           CheckInputs(ref map, sensorSize, sensorRangeX, sensorRangeY);
        }

        public void CheckInputs(ref MapDrawer map, int sensorSize, int sensorRangeX, int sensorRangeY)
        {
            Size size = new Size(sensorSize, sensorSize);
            for (int i = 1; i < Brain.Inputs.Length; i++)
            {
                if (!Inputs.ContainsKey(i))
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
        }

        //DoubleToBoolean
        private static bool DtB(double input)
        {
            return input >= 0.5D;
        }

        public void Step(Point sonicPos, SonicObject[] objects)
        {
            foreach (var kvp in Inputs)
            {
                Brain.Inputs[kvp.Key].InputValue = (int)kvp.Value.GetValue(sonicPos, objects);
            }

            if (Brain.RelaxNetwork(10, 0.1))
                PressButtons();
        }

        public void PressButtons()
        {
            if (this.A)
                Global.ClickyVirtualPadController.Click("P1 A");
            else
                Global.ClickyVirtualPadController.Unclick("P1 A");
            if (this.PadUp)
                Global.ClickyVirtualPadController.Click("P1 Up");
            else
                Global.ClickyVirtualPadController.Unclick("P1 Up");
            if (this.PadDown)
                Global.ClickyVirtualPadController.Click("P1 Down");
            else
                Global.ClickyVirtualPadController.Unclick("P1 Down");
            if (this.PadLeft)
                Global.ClickyVirtualPadController.Click("P1 Left");
            else
                Global.ClickyVirtualPadController.Unclick("P1 Left");
            if (this.PadRight)
                Global.ClickyVirtualPadController.Click("P1 Right");
            else
                Global.ClickyVirtualPadController.Unclick("P1 Right");
        }
    }
}