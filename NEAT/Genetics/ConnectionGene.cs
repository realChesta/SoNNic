using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEAT.Genetics
{
    public class ConnectionGene
    {
        public int InputNode { get; private set; }
        public int OutputNode { get; private set; }
        public double Weight { get; set; }
        public bool Enabled { get; set; }
        public ulong InnovationNumber { get; private set; }

        public bool IsMutated = false;

        public ConnectionGene(ulong innoNr, int input, int output, double weight, bool enabled)
        {
            this.InnovationNumber = innoNr;
            this.InputNode = input;
            this.OutputNode = output;
            this.Weight = weight;
            this.Enabled = enabled;
        }

        public ConnectionGene(ConnectionGene copyFrom)
        {
            this.InnovationNumber = copyFrom.InnovationNumber;
            this.InputNode = copyFrom.InputNode;
            this.OutputNode = copyFrom.OutputNode;
            this.Weight = copyFrom.Weight;
            this.Enabled = copyFrom.Enabled;
        }

        public override string ToString()
        {
            return "W: " + this.Weight.ToString() + "; Enabled: " + this.Enabled.ToString().ToLower();
        }
    }
}
