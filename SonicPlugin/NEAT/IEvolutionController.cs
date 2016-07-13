using NEAT.Genetics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEAT
{
    public interface IEvolutionController
    {
        IPhenotype[] Population { get; }
    }
}
