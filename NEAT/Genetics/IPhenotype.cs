using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEAT.Genetics
{
    public interface IPhenotype
    {
        Genome Genome { get; set; }
        NEAT.NeuralNetworks.NeuralNetwork Brain { get; set; }
        double Fitness { get; set; }
        IPhenotype CreateOffspring(IPhenotype partner);
        IPhenotype MutateOffspring(ref EvolutionController.SpeciesParameters prms);
    }
}
