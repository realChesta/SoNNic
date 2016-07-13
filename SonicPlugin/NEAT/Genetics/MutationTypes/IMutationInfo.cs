using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEAT.Genetics
{
    public interface IMutationInfo
    {
        Genome.MutationType MutationType { get; }
        double Probability { get; set; }
    }
}
