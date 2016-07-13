using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NEAT.Genetics;
using NEAT.NeuralNetworks;

namespace NEAT
{
    public class EvolutionController
    {
        public SpeciesCollection Population = new SpeciesCollection();
        private FastRandom Random = new FastRandom();
        public Dictionary<int, Species> NonImprovingSpecies;
        public int InitialPopulationSize;

        public uint Generation{ get; private set; }

        public double MinConnectionWeight
        {
            get
            { return Population.Parameters.MinimumWeight; }
            set
            { Population.Parameters.MinimumWeight = value; }
        }
        public double MaxConnectionWeight
        {
            get
            { return Population.Parameters.MaximumWeight; }
            set
            { Population.Parameters.MaximumWeight = value; }
        }

        public double CompatibilityCoefficient1;
        public double CompatibilityCoefficient2;
        public double CompatibilityCoefficient3;

        public double CompatibilityThreshold;

        public EvolutionController(double minWeight, double maxWeight, double c1 = 1, double c2 = 1, double c3 = 1, double compThreshold = 3)
        {
            this.MinConnectionWeight = minWeight;
            this.MaxConnectionWeight = maxWeight;
            this.CompatibilityCoefficient1 = c1;
            this.CompatibilityCoefficient2 = c2;
            this.CompatibilityCoefficient3 = c3;
            this.CompatibilityThreshold = compThreshold;
        }

        /// <summary>
        /// Start with given initialPopulation (all phenotypes should have 0 hidden nodes!)
        /// </summary>
        /// <param name="initialPopulation">The initial population with 0 hidden nodes.</param>
        public void Start(Genome[] initialPopulation)
        {
            this.Population.Clear();

            this.InitialPopulationSize = initialPopulation.Length;

            //Connect every input to every output
            for (int i = 0; i < initialPopulation.Length; i++)
            {
                initialPopulation[i].ConnectLayers(MinConnectionWeight, MaxConnectionWeight, Population.Parameters.InitialConnectionProportion);
            }

            this.Population = SortGenomesIntoSpecies(initialPopulation);
        }

        public void Start(int populationCount, int inputs, int outputs, IActivationFunction<double> function)
        {
            Genome[] initialPopulation = new Genome[populationCount];
            for (int i = 0; i < populationCount; i++)
            {
                initialPopulation[i] = new Genome(inputs, outputs, 0, function, ref Random);
            }
            this.Start(initialPopulation);
        }

        public void NextGeneration()
        {
            //Clear registered Mutations
            InnovationGenerator.ClearRegisteredMutations();

            //Calculate stats
            Species.Stats[] stats = Population.CalculateSpeciesStats(InitialPopulationSize);

            //Remove very weak species and recalculate stats without them (if any were removed)
            if (RemovePoorSpecies(stats))
                stats = Population.CalculateSpeciesStats(InitialPopulationSize);

            //Get offspring
            List<Genome> newGeneration = CreateGenerationOffspring(stats);

            //Trim existing population to elites
            bool emptySpeciesExist = this.Population.TrimToElites(stats);

            //adjust species count
            bool rebuildSpecies = AdjustCompatibilityThreshold(stats, Population.Parameters.MinimumSpeciesCount, Population.Parameters.MaximumSpeciesCount);

            //If there are empty species after the elite-trimming, we're going to re-speciate all genomes
            if (emptySpeciesExist || rebuildSpecies)
            {
                //Add elites to offspring
                newGeneration.AddRange(this.Population.GetAll());

                this.Population.Clear();

                //Re-speciate all genomes
                this.Population = SortGenomesIntoSpecies(newGeneration.ToArray());
            }
            else
            {
                SortGenomesIntoExistingSpecies(newGeneration.ToArray());
            }

            //Sort all genomes in all species by their fitness (fittest first)
            this.Population.SortGenomesByFitness();

            Generation++;
        }

        /// <summary>
        /// Remove species with target size of 0.
        /// </summary>
        /// <param name="stats"></param>
        private bool RemovePoorSpecies(Species.Stats[] stats)
        {
            bool toReturn = false;
            for (int i = 0; i < stats.Length; i++)
            {
                if (stats[i].RoundedTargetPopulation == 0)
                {
                    this.Population.Remove(stats[i].SpeciesID);
                    toReturn = true;
                }
            }
            return toReturn;
        }

        private bool adjustingThreshold = false;
        private double thresholdDelta = 0;
        private const double thresholdDeltaAcceleration = 1.05;
        private double lastComplexity = 0;
        private bool AdjustCompatibilityThreshold(Species.Stats[] stats, int minSpeciesCount, int maxSpeciesCount)
        {
            //adjustingThreshold = true;
            bool redetermineSpecies = false;
            int currentCount = stats.Length;
            if (currentCount < minSpeciesCount)
            {
                //too few species -> reduce threshold
                if (adjustingThreshold)
                {
                    if (thresholdDelta < 0)
                        thresholdDelta *= thresholdDeltaAcceleration;
                    else
                        thresholdDelta *= -0.5;
                }
                else
                {
                    adjustingThreshold = true;
                    thresholdDelta = -Math.Max(0.1, CompatibilityThreshold * 0.01);
                }

                CompatibilityThreshold += thresholdDelta;
                CompatibilityThreshold = Math.Max(0.01, CompatibilityThreshold);

                redetermineSpecies = true;
            }
            else if (currentCount > maxSpeciesCount)
            {
                if (adjustingThreshold)
                {
                    if (thresholdDelta < 0)
                        thresholdDelta *= -0.5;
                    else
                        thresholdDelta *= thresholdDeltaAcceleration;
                }
                else
                {
                    adjustingThreshold = true;
                    thresholdDelta = Math.Max(0.1, CompatibilityThreshold * 0.01);
                }

                CompatibilityThreshold += thresholdDelta;

                redetermineSpecies = true;
            }
            else
            {
                adjustingThreshold = false;
            }

            double avgComplexity = (stats.Sum(s => s.AverageComplexity) / stats.Length);
            if (!redetermineSpecies)
            {
                double complexity = Math.Abs((avgComplexity - lastComplexity) / lastComplexity);

                if (complexity > 0.05)
                {
                    redetermineSpecies = true;

                    lastComplexity = avgComplexity;
                }
            }
            else
            { 

            }

            return redetermineSpecies;
        }

        public List<Genome> CreateGenerationOffspring(Species.Stats[] stats)
        {
            List<Genome> newGeneration = new List<Genome>();

            List<int> emptySpecies = new List<int>();
            foreach (var species in Population)
            {
                if (species.Value.Count < 1)
                    emptySpecies.Add(species.Key);
            }
            for (int i = 0; i < emptySpecies.Count; i++)
            {
                Population.Remove(emptySpecies[i]);
            }

            int speciesCount = stats.Length;

            int offspring = stats.Sum(s => s.OffspringCount);
            double[][] genomeWeights = new double[speciesCount][];

            int nonZeroSpecies = 0;
            for (int i = 0; i < speciesCount; i++)
            {
                if (stats[i].SelectionCount != 0)
                    nonZeroSpecies++;

                Species species = Population[stats[i].SpeciesID];
                int count = species.Count;
                genomeWeights[i] = new double[count];

                for (int j = 0; j < count; j++)
                {
                    genomeWeights[i][j] = species[j].Fitness;
                }
            }

            for (int i = 0; i < speciesCount; i++)
            {
                Species currentSpecies = Population[stats[i].SpeciesID];
                int interspeciesOffspring = (int)((double)stats[i].MatingOffspringCount * Population.Parameters.InterspeciesOffspringProportion);
                stats[i].MatingOffspringCount -= interspeciesOffspring;

                //Do mutations and add newly created genomes to the new generation
                for (int j = 0; j < stats[i].MutationOffspringCount; j++)
                {
                    Genome chosen = currentSpecies[Utils.RandomSelectIndex(genomeWeights[i])];
                    Genome offspringGenome = new Genome(chosen); //create a copy to mutate
                    offspringGenome.Mutate(ref Population.Parameters);
                    newGeneration.Add(offspringGenome);
                }

                int totalMatingOffspring = 0;

                //Do interspecies mating
                for (; totalMatingOffspring < interspeciesOffspring; totalMatingOffspring++)
                {
                    Genome g1 = currentSpecies[Utils.RandomSelectIndex(genomeWeights[i])];
                    int species = Utils.RandomInt(Population.Count);
                    Genome g2 = Population[stats[species].SpeciesID][Utils.RandomSelectIndex(genomeWeights[species])];

                    newGeneration.Add(g1.Crossover(g2));
                }

                //Do normal, species-intern mating
                if (currentSpecies.Count < 2) //when there's only one genome for breeding
                {
                    for (; totalMatingOffspring < stats[i].MatingOffspringCount; totalMatingOffspring++)
                    {
                        Genome offspringGenome = new Genome(currentSpecies[0]);
                        offspringGenome.Mutate(ref Population.Parameters);
                        newGeneration.Add(offspringGenome);
                    }
                }
                else
                {
                    for (; totalMatingOffspring < stats[i].MatingOffspringCount; totalMatingOffspring++)
                    {
                        int i1 = Utils.RandomSelectIndex(genomeWeights[i]);
                        Genome g1 = currentSpecies[i1];

                        List<double> weights = genomeWeights[i].ToList();
                        weights.RemoveAt(i1);
                        if (weights.FirstOrDefault(w => w != 0) == 0)
                        {
                            //no other genomes with probability > 0
                            //-> do mutations instead of matings
                            for (; totalMatingOffspring < stats[i].MatingOffspringCount; totalMatingOffspring++)
                            {
                                Genome offspringGenome = new Genome(g1);
                                offspringGenome.Mutate(ref Population.Parameters);
                                newGeneration.Add(offspringGenome);
                            }
                            break;
                        }
                        else
                        {
                            Genome g2 = currentSpecies[Utils.RandomSelectIndex(genomeWeights[i])];

                            while (g1 == g2)
                                g2 = currentSpecies[Utils.RandomSelectIndex(genomeWeights[i])];

                            newGeneration.Add(g1.Crossover(g2));
                        }
                    }
                }
            }

            int offspringSum = stats.Sum(s => s.OffspringCount);
            double targetPop = stats.Sum(s => s.TargetPopulation);
            double initialPop = stats.Sum(s => s.InitialPopulation);
            double offspringSumTotal = stats.Sum(s => s.MatingOffspringCount + s.MutationOffspringCount);
            int eliteSum = stats.Sum(s => s.EliteCount);
            return newGeneration;
        }

        public SpeciesCollection SortGenomesIntoSpecies(Genome[] genomes)
        {
            SpeciesCollection newPopulation = new SpeciesCollection() { Parameters = this.Population.Parameters };
            SpeciesCollection oldPopulation = new SpeciesCollection(this.Population);

            for (int i = 0; i < genomes.Length; i++)
            {
                bool found = false;
                foreach (var species in oldPopulation)
                {
                    if (species.Value.Count == 0) continue;
                    //Choose a random representative for the species
                    Genome representative = species.Value[0];//species.Value[Random.Next(species.Value.Count)];

                    //Check whether the phenotype matches this species
                    if (genomes[i].CompatibilityDistance(representative, CompatibilityCoefficient1, CompatibilityCoefficient2, CompatibilityCoefficient3) < CompatibilityThreshold)
                    {
                        if (newPopulation.ContainsKey(species.Key))
                            newPopulation[species.Key].Add(genomes[i]);
                        else
                            newPopulation.Add(new Species(species.Key, new Genome[] { genomes[i] }));
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    int key = oldPopulation.Count;
                    while (oldPopulation.ContainsKey(key))
                        key++;
                    Species s = new Species(key, new Genome[] { genomes[i] });
                    newPopulation.Add(s);
                    oldPopulation.Add(s);
                }
            }

            return newPopulation;
        }

        public void SortGenomesIntoExistingSpecies(Genome[] genomes)
        {
            for (int i = 0; i < genomes.Length; i++)
            {
                bool found = false;
                foreach (var species in this.Population)
                {
                    if (species.Value.Count == 0) continue;
                    //Choose a random representative for the species
                    Genome representative = species.Value[0];//species.Value[Random.Next(species.Value.Count)];

                    //Check whether the phenotype matches this species
                    if (genomes[i].CompatibilityDistance(representative, CompatibilityCoefficient1, CompatibilityCoefficient2, CompatibilityCoefficient3) < CompatibilityThreshold)
                    {
                        species.Value.Add(genomes[i]);
                        //if (newPopulation.ContainsKey(species.Key))
                        //    newPopulation[species.Key].Add(genomes[i]);
                        //else
                        //    newPopulation.Add(new Species(species.Key, new Genome[] { genomes[i] }));
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    int key = this.Population.Count;
                    while (this.Population.ContainsKey(key))
                        key++;
                    this.Population.Add(new Species(key, new Genome[] { genomes[i] }));
                }
            }
        }

        public double CompatibilityDistance(Genome genome1, Genome genome2)
        {
            var D = genome1.DisjointGenes(genome2).Length;
            var E = genome1.ExcessGenes(genome2).Length;

            int N = 1;
            if ((genome1.Connections.Count >= 20) && (genome2.Connections.Count >= 20))
            {
                N = Math.Max(genome1.Connections.Count, genome2.Connections.Count);
            }

            double W = genome1.AverageWeightDifference(genome2);

            var distance = ((CompatibilityCoefficient1 * E) / N) + ((CompatibilityCoefficient2 * D) / N) + (CompatibilityCoefficient3 * W);

            return distance;
        }

        public void Reset()
        {
            this.Population.Clear();
            //this.NonImprovingSpecies.Clear();
            InnovationGenerator.ClearRegisteredMutations();
            Generation = 0;
            GC.Collect();
        }

        public class Species : List<Genome>
        {
            public int ID { get; private set; }

            public Species(int id) : base()
            {
                this.ID = id;
            }
            public Species(int id, Genome[] initialPopulation) : base(initialPopulation)
            {
                this.ID = id;
            }

            public double AdjustedFitness(Genome g)
            {
                return g.Fitness / (double)this.Count;
            }

            public double AverageFitness(bool adjusted = true)
            {
                return (this.Sum(g => adjusted ? AdjustedFitness(g) : g.Fitness) / (double)this.Count);
            }

            public double AverageComplexity()
            {
                return (this.Sum(g => g.Connections.Count + g.Nodes.Count) / this.Count);
            }

            public Stats GetStatsWithFitness()
            {
                return new Stats(this.ID, this.Count) { AverageFitness = this.AverageFitness(false), 
                                                        AverageComplexity = this.AverageComplexity() };
            }

            public struct Stats
            {
                public readonly int SpeciesID;
                public double InitialPopulation;
                public double AverageFitness;
                public double TargetPopulation;
                public double AverageComplexity;
                public int RoundedTargetPopulation;
                public int EliteCount;
                public int OffspringCount;
                public int MutationOffspringCount;
                public int MatingOffspringCount;
                public int SelectionCount;

                public Stats(int id, int initialPopulation)
                {
                    this.SpeciesID = id;
                    this.InitialPopulation = initialPopulation;
                    this.AverageFitness = 0;
                    this.TargetPopulation = 0;
                    this.RoundedTargetPopulation = 0;
                    this.EliteCount = 0;
                    this.OffspringCount = 0;
                    this.MutationOffspringCount = 0;
                    this.MatingOffspringCount = 0;
                    this.SelectionCount = 0;
                    this.AverageComplexity = 0;
                }
            }
        }

        public class SpeciesCollection : Dictionary<int, Species>
        {
            public SpeciesParameters Parameters;

            public SpeciesCollection()
                : base()
            {
                this.Parameters = new SpeciesParameters();
            }

            public SpeciesCollection(IDictionary<int, Species> dictionary)
                : base(dictionary)
            {
                this.Parameters = new SpeciesParameters();
            }

            public SpeciesCollection(int capacity)
                : base(capacity)
            {
                this.Parameters = new SpeciesParameters();
            }

            public SpeciesCollection(SpeciesParameters prms)
                : base()
            {
                this.Parameters = prms;
            }

            private new Dictionary<int, Species>.KeyCollection Keys
            {
                get
                {
                    return base.Keys;
                }
            }

            private new Dictionary<int, Species>.ValueCollection Values
            {
                get
                {
                    return base.Values;
                }
            }

            public int[] SpeciesIDs
            {
                get
                {
                    return base.Keys.ToArray();
                }
            }

            public Species[] Species
            {
                get
                {
                    return base.Values.ToArray();
                }
            }

            public new Species this[int id]
            {
                get
                {
                    return base[id];
                }
            }

            public Genome[] GetAll()
            {
                Species[] species = Species;
                if (species.Length > 0)
                {
                    IEnumerable<Genome> population = species[0];
                    if (species.Length > 1)
                    {
                        for (int i = 1; i < species.Length; i++)
                        {
                            population = population.Concat(species[i]);
                        }
                    }
                    return population.ToArray();
                }
                else
                { return new Genome[0]; }
            }

            public bool TryGetSpecies(int id, out Species species)
            { return base.TryGetValue(id, out species); }

            public void Add(Species species)
            {
                base.Add(species.ID, species);
            }
            public new void Add(int id, Species species)
            {
                this.Add(species);
            }

            public new void Clear()
            { base.Clear(); }

            private new bool ContainsKey(int id)
            { return base.ContainsKey(id); }
            private new bool ContainsValue(Species gene)
            { return base.ContainsValue(gene); }

            public bool ContainsInnovationNumber(int id)
            { return base.ContainsKey(id); }

            public new bool Remove(int id)
            { return base.Remove(id); }

            public Species.Stats[] CalculateSpeciesStats(int populationSize)
            {
                //Get stats objects with average fitnesses 
                Species.Stats[] stats = this.Species.Select(s => s.GetStatsWithFitness()).OrderByDescending(s => s.AverageFitness).ToArray();
                if (stats.Length < 1) return stats;
                int speciesCount = stats.Length;
                //int populationSize = this.Species.Sum(s => s.Count);
                //Genome bestGenome = this.GetAll().OrderByDescending(g => g.Fitness).FirstOrDefault();
                //Species containsBestGenome = this.Values.FirstOrDefault(s => s.Contains(bestGenome));
                //Species.Stats bestGenomeStats = stats.FirstOrDefault(s => s.SpeciesID == containsBestGenome.ID);
                Species.Stats bestGenomeSpeciesStats = stats.FirstOrDefault(s => s.SpeciesID == this.Values.OrderByDescending(sp => sp.OrderByDescending(ph => ph.Fitness).FirstOrDefault().Fitness).FirstOrDefault().ID); //Get the species containing the best genome
                int bestIndex = Array.IndexOf(stats, bestGenomeSpeciesStats);

                double fitnessSum = stats.Sum(s => s.AverageFitness);

                int targetPopulationSum = 0;

                #region adjust targetPopulation
                if (fitnessSum > 0)
                {
                    for (int i = 0; i < speciesCount; i++)
                    {
                        stats[i].TargetPopulation = (stats[i].AverageFitness / fitnessSum) * (double)populationSize;
                        stats[i].RoundedTargetPopulation = (int)Utils.ProbabilisticRound(stats[i].TargetPopulation);

                        if (i == bestIndex)
                            bestGenomeSpeciesStats = stats[i];

                        targetPopulationSum += stats[i].RoundedTargetPopulation;
                    }
                }
                else //if all species/genomes have 0 fitness
                {
                    double targetSize = (double)populationSize / (double)speciesCount;

                    for (int i = 0; i < speciesCount; i++)
                    {

                        stats[i].TargetPopulation = targetSize;
                        stats[i].RoundedTargetPopulation = (int)Utils.ProbabilisticRound(targetSize);

                        if (i == bestIndex)
                            bestGenomeSpeciesStats = stats[i];

                        targetPopulationSum += stats[i].RoundedTargetPopulation;
                    }
                }

                int targetPopulationDelta = targetPopulationSum - populationSize; //check if targetPopulation differs from populationSize because of probalistic rounding

                if (targetPopulationDelta == -1)
                {
                    //if we're lacking one organism, add one to the most promising species (contains the best organism, could also be done by using average fitness)
                    bestGenomeSpeciesStats.RoundedTargetPopulation++;
                    targetPopulationSum++;
                    stats[bestIndex] = bestGenomeSpeciesStats;
                }
                else if (targetPopulationDelta != 0)
                {
                    double[] weights = stats.Select(s => (Math.Max(0D, s.TargetPopulation - (double)s.RoundedTargetPopulation) + 1)).ToArray();

                    if (targetPopulationDelta < 0) //targetPopulation is smaller than initially wanted
                    {
                        targetPopulationDelta *= -1;

                        for (int i = 0; i < targetPopulationDelta; i++)
                        {
                            stats[Utils.RandomSelectIndex(weights)].RoundedTargetPopulation++;
                        }
                    }
                    else //targetPopulation is bigger than initially wanted
                    {
                        for (int i = 0; i < targetPopulationDelta; )
                        {
                            int index = Utils.RandomSelectIndex(weights);

                            if (stats[index].RoundedTargetPopulation > 0)
                            {
                                stats[index].RoundedTargetPopulation--;
                                i++;
                            }
                        }
                    }
                }
                #endregion

                #region Preserve best genome
                //Ensure that the most promising species doesn't have 0 offspring -> preserve best genome
                if (bestGenomeSpeciesStats.RoundedTargetPopulation == 0)
                {
                    stats[bestIndex].RoundedTargetPopulation++;

                    //Choose a random other species to adjust the total population back to the desired number
                    int chosen = Utils.RandomInt(speciesCount);
                    if (chosen == bestIndex)
                        chosen++;
                    if (chosen >= speciesCount)
                        chosen = 0;
                    int started = chosen;

                    //Avoid creating empty species
                    while (stats[chosen].RoundedTargetPopulation < 2)
                    {
                        if (chosen >= (speciesCount - 1))
                            chosen = 0;
                        else
                            chosen++;

                        if (chosen == started)
                            throw new Exception("Could not find a species to adjust!");

                        if (chosen == bestIndex)
                            chosen++;
                    }
                    stats[chosen].RoundedTargetPopulation--;
                }
                #endregion

                #region determine elites/offspring
                for (int i = 0; i < speciesCount; i++)
                {

                    if (stats[i].RoundedTargetPopulation == 0)
                    {
                        stats[i].EliteCount = 0;
                        continue;
                    }

                    //Calculate elite count based on the proportions while preventing it from getting bigger than the targetPopulation of this species
                    stats[i].EliteCount = Math.Min(stats[i].RoundedTargetPopulation, (int)Utils.ProbabilisticRound((double)stats[i].InitialPopulation * this.Parameters.EliteProportion));

                    //Make sure best genome is preserved, even if its species consists of e.g. 1 genome
                    if ((i == bestIndex) && (bestGenomeSpeciesStats.EliteCount < 1))
                        bestGenomeSpeciesStats.EliteCount = 1;

                    stats[i].OffspringCount = stats[i].RoundedTargetPopulation - stats[i].EliteCount;

                    stats[i].MutationOffspringCount = (int)Utils.ProbabilisticRound((double)stats[i].OffspringCount * this.Parameters.MutationOffspringProportion);
                    stats[i].MatingOffspringCount = stats[i].OffspringCount - stats[i].MutationOffspringCount;

                    stats[i].SelectionCount = Math.Max(1, (int)Utils.ProbabilisticRound(stats[i].InitialPopulation * this.Parameters.SelectionProportion));

                    if (i == bestIndex)
                        bestGenomeSpeciesStats = stats[i];
                }
                #endregion

                if (bestIndex != -1)
                    stats[bestIndex] = bestGenomeSpeciesStats;

                int sum = stats.Sum(s => s.RoundedTargetPopulation);
                int offspringSum = stats.Sum(s => s.MatingOffspringCount);
                int mutationSum = stats.Sum(s => s.MutationOffspringCount);
                int eliteSum = stats.Sum(s => s.EliteCount);

                return stats;
            }

            public bool TrimToElites(Species.Stats[] stats)
            {
                Species[] species = this.Species;
                bool emptySpeciesExist = false;

                for (int i = 0; i < species.Length; i++)
                {
                    List<Genome> population = species[i].OrderByDescending(g => g.Fitness).ToList();
                    int toKeep = stats.FirstOrDefault(s => s.SpeciesID == species[i].ID).EliteCount;
                    int origCount = population.Count;
                    population.RemoveRange(toKeep, population.Count - toKeep);

                    this.Remove(species[i].ID);

                    if (population.Count < 1)
                        emptySpeciesExist = true;

                    this.Add(new Species(species[i].ID, population.ToArray()));
                }

                return emptySpeciesExist;
            }

            public void SortGenomesByFitness()
            {
                Species[] species = this.Species;
                this.Clear();
                for (int i = 0; i < species.Length; i++)
                {
                    this.Add(new Species(species[i].ID, species[i].OrderByDescending(g => g.Fitness).ToArray()));
                }
            }
        }

        public class SpeciesParameters
        {
            public const double DefaultEliteProportion = 0.2;
            public const double DefaultSelectionProportion = 0.2;

            public const double DefaultMutationOffspringProportion = /*0.5; //*/0.25;
            public const double DefaultMatingOffspringProportion = /*0.5; //*/0.75;
            public const double DefaultInterspeciesOffspringProportion = 0.1;

            public const double DefaultDisjointExcessGenesRecombineProbability = 0.1;
            public const double DefaultConnectionWeightMutationProbability = 0.988; //*/0.8;
            public const double DefaultAddNodeMutationProbability = 0.001; //*/0.03;
            public const double DefaultAddConnectionMutationProbability = 0.01; //*/0.05;
            public const double DefaultRemoveConnectionMutationProbability = 0.001;
            //public const double DefaultReEnableGenesProbability = 0.01;
            public const double DefaultInitialConnectionProportion = 0.05;

            public const int DefaultMinimumSpeciesCount = 6;
            public const int DefaultMaximumSpeciesCount = 10;

            public const double DefaultMinimumWeight = -5;
            public const double DefaultMaximumWeight = 5;


            public double EliteProportion;
            public double SelectionProportion;

            public double MutationOffspringProportion;
            public double MatingOffspringProportion;
            public double InterspeciesOffspringProportion;

            public double DisjointExcessGenesRecombineProbability;
            public double ConnectionWeightMutationProbability;
            public double AddNodeMutationProbability;
            public double AddConnectionMutationProbability;
            public double RemoveConnectionMutationProbability;
            public double InitialConnectionProportion;

            public double MinimumWeight;
            public double MaximumWeight;

            public int MinimumSpeciesCount;
            public int MaximumSpeciesCount;

            public IMutationInfo[] PossibleMutations;

            public SpeciesParameters()
            {
                this.EliteProportion = DefaultEliteProportion;
                this.SelectionProportion = DefaultSelectionProportion;

                this.MutationOffspringProportion = DefaultMutationOffspringProportion;
                this.MatingOffspringProportion = DefaultMatingOffspringProportion;
                this.InterspeciesOffspringProportion = DefaultInterspeciesOffspringProportion;

                this.DisjointExcessGenesRecombineProbability = DefaultDisjointExcessGenesRecombineProbability;
                this.ConnectionWeightMutationProbability = DefaultConnectionWeightMutationProbability;
                this.AddNodeMutationProbability = DefaultAddNodeMutationProbability;
                this.AddConnectionMutationProbability = DefaultAddConnectionMutationProbability;
                this.RemoveConnectionMutationProbability = DefaultRemoveConnectionMutationProbability;
                this.InitialConnectionProportion = DefaultInitialConnectionProportion;

                this.MinimumWeight = DefaultMinimumWeight;
                this.MaximumWeight = DefaultMaximumWeight;

                this.MinimumSpeciesCount = DefaultMinimumSpeciesCount;
                this.MaximumSpeciesCount = DefaultMaximumSpeciesCount;

                this.PossibleMutations = GetDefaultMutations2();
            }

            public SpeciesParameters(double minWeight, double maxWeight, double eliteProportion, double selectionProportion, double mutationOffspringProportion,
                                     double matingOffspringProportion, double interspeciesOffspringProportion,
                                     double disjointExcessRecombineProbability, double connectionWeightMutationProbability,
                                     double addNodeProbability, double addConnectionProbability, double removeConnectionProbability, IMutationInfo[] mutations)
            {
                this.EliteProportion = eliteProportion;
                this.SelectionProportion = selectionProportion;
                this.MutationOffspringProportion = mutationOffspringProportion;
                this.MatingOffspringProportion = matingOffspringProportion;
                this.InterspeciesOffspringProportion = interspeciesOffspringProportion;
                this.DisjointExcessGenesRecombineProbability = disjointExcessRecombineProbability;
                this.ConnectionWeightMutationProbability = connectionWeightMutationProbability;
                this.AddNodeMutationProbability = addConnectionProbability;
                this.AddConnectionMutationProbability = addConnectionProbability;
                this.RemoveConnectionMutationProbability = removeConnectionProbability;
                this.MinimumWeight = minWeight;
                this.MaximumWeight = maxWeight;
                this.PossibleMutations = mutations;
            }

            public IMutationInfo[] GetDefaultMutations(bool allowNodeRemoval = false)
            {
                List<IMutationInfo> toReturn = new List<IMutationInfo>();

                toReturn.Add(new AddNodeMutationInfo());
                toReturn.Add(new AddConnectionMutationInfo());

                if (allowNodeRemoval)
                    toReturn.Add(new RemoveConnectionMutationInfo());

                #region Weight Mutations

                WeightMutationInfo weightMutation = new WeightMutationInfo();

                //Jiggles/deltas
                //sigma = 0.02
                weightMutation.Mutations.Add(new WeightMutationInfo.WeightMutation(WeightMutationInfo.WeightMutationType.JiggleNormalDistribution, 0.11375, 0.02, 2, WeightMutationInfo.Selection.SelectionType.Absolute));
                weightMutation.Mutations.Add(new WeightMutationInfo.WeightMutation(WeightMutationInfo.WeightMutationType.JiggleNormalDistribution, 0.11375, 0.02, 3, WeightMutationInfo.Selection.SelectionType.Absolute));
                weightMutation.Mutations.Add(new WeightMutationInfo.WeightMutation(WeightMutationInfo.WeightMutationType.JiggleNormalDistribution, 0.11375, 0.02, 1, WeightMutationInfo.Selection.SelectionType.Absolute));

                //sigma = 0.02
                weightMutation.Mutations.Add(new WeightMutationInfo.WeightMutation(WeightMutationInfo.WeightMutationType.JiggleNormalDistribution, 0.11375, 0.02, 0.02, WeightMutationInfo.Selection.SelectionType.Proportional));

                //sigma = 1
                weightMutation.Mutations.Add(new WeightMutationInfo.WeightMutation(WeightMutationInfo.WeightMutationType.JiggleNormalDistribution, 0.11375, 1, 1, WeightMutationInfo.Selection.SelectionType.Absolute));
                weightMutation.Mutations.Add(new WeightMutationInfo.WeightMutation(WeightMutationInfo.WeightMutationType.JiggleNormalDistribution, 0.11375, 1, 2, WeightMutationInfo.Selection.SelectionType.Absolute));
                weightMutation.Mutations.Add(new WeightMutationInfo.WeightMutation(WeightMutationInfo.WeightMutationType.JiggleNormalDistribution, 0.11375, 1, 3, WeightMutationInfo.Selection.SelectionType.Absolute));

                //sigma = 1
                weightMutation.Mutations.Add(new WeightMutationInfo.WeightMutation(WeightMutationInfo.WeightMutationType.JiggleNormalDistribution, 0.11275, 1, 0.02, WeightMutationInfo.Selection.SelectionType.Proportional));

                weightMutation.Mutations.Add(new WeightMutationInfo.WeightMutation(WeightMutationInfo.WeightMutationType.JiggleNormalDistribution, 0.03, 1, 1, WeightMutationInfo.Selection.SelectionType.Absolute));
                weightMutation.Mutations.Add(new WeightMutationInfo.WeightMutation(WeightMutationInfo.WeightMutationType.JiggleNormalDistribution, 0.03, 1, 2, WeightMutationInfo.Selection.SelectionType.Absolute));
                weightMutation.Mutations.Add(new WeightMutationInfo.WeightMutation(WeightMutationInfo.WeightMutationType.JiggleNormalDistribution, 0.03, 1, 3, WeightMutationInfo.Selection.SelectionType.Absolute));

                //random mutation
                weightMutation.Mutations.Add(new WeightMutationInfo.WeightMutation(WeightMutationInfo.WeightMutationType.Random, 0.001, 0, 0.02, WeightMutationInfo.Selection.SelectionType.Proportional));

                toReturn.Add(weightMutation);

                #endregion

                return toReturn.ToArray();
            }

            public IMutationInfo[] GetDefaultMutations2()
            {
                List<IMutationInfo> toReturn = new List<IMutationInfo>();

                toReturn.Add(new AddNodeMutationInfo());
                toReturn.Add(new AddConnectionMutationInfo());

                #region Weight mutations

                WeightMutationInfo weightMutation = new WeightMutationInfo();

                weightMutation.Mutations.Add(new WeightMutationInfo.WeightMutation(WeightMutationInfo.WeightMutationType.JiggleEven, 0.125, 0.05, new WeightMutationInfo.Selection(0.5, WeightMutationInfo.Selection.SelectionType.Proportional)));
                weightMutation.Mutations.Add(new WeightMutationInfo.WeightMutation(WeightMutationInfo.WeightMutationType.JiggleEven, 0.125, 0.05, new WeightMutationInfo.Selection(0.1, WeightMutationInfo.Selection.SelectionType.Proportional)));
                weightMutation.Mutations.Add(new WeightMutationInfo.WeightMutation(WeightMutationInfo.WeightMutationType.JiggleEven, 0.125, 0.05, new WeightMutationInfo.Selection(1, WeightMutationInfo.Selection.SelectionType.Absolute)));

                weightMutation.Mutations.Add(new WeightMutationInfo.WeightMutation(WeightMutationInfo.WeightMutationType.Random, 0.5, 0, 0.02, WeightMutationInfo.Selection.SelectionType.Proportional));
                weightMutation.Mutations.Add(new WeightMutationInfo.WeightMutation(WeightMutationInfo.WeightMutationType.Random, 0.125, 0, 1, WeightMutationInfo.Selection.SelectionType.Absolute));

                toReturn.Add(weightMutation);

                #endregion

                return toReturn.ToArray();
            }
        }
    }
}
