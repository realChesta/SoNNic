using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEAT.Genetics
{
    public class NodeGenes
    {
        public int Inputs { get; private set; }
        public int Outputs { get; private set; }
        public int HiddenNodes { get; private set; }

        public NodeGenes(int inputs, int outputs, int hidden)
        {
            this.Inputs = inputs;
            this.Outputs = outputs;
            this.HiddenNodes = hidden;
        }
        public NodeGenes(NodeGenes copyFrom)
        {
            this.Inputs = copyFrom.Inputs;
            this.Outputs = copyFrom.Outputs;
            this.HiddenNodes = copyFrom.HiddenNodes;
        }
    }
}
