using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NEAT;
using NEAT.NeuralNetworks;

namespace NEAT.Genetics
{
    public class Phenotype : IPhenotype
    {
        public Genome Genome { get; set; }
        public NeuralNetwork Brain { get; set; }
        public double Fitness { get; set; }

        public Phenotype(int inputs, int outputs, int hidden, IActivationFunction<double> function, ref FastRandom rnd)
        {
            this.Genome = new Genome(inputs, outputs, hidden, function, ref rnd);
            this.Brain = Genome.CreateNetwork();
        }
        public Phenotype(Genome g)
        {
            this.Genome = g;
            this.Brain = g.CreateNetwork();
        }


        public IPhenotype CreateOffspring(IPhenotype partner)
        {
            throw new NotImplementedException();
        }

        public IPhenotype MutateOffspring(ref EvolutionController.SpeciesParameters prms)
        {
            Genome g = new Genome(this.Genome);
            g.Mutate(ref prms);
            return new Phenotype(g); 
        }
    }
}
