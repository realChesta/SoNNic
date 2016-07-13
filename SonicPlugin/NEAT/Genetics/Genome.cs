using NEAT;
using NEAT.NeuralNetworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NEAT.Genetics
{
    public class Genome
    {
        //public NodeGenes Nodes { get; private set; }
        public NodeGeneCollection Nodes { get; private set; }
        public ConnectionGeneCollection Connections { get; private set; }
        private FastRandom Random;
        public IActivationFunction<double> ActivationFunction { get; private set; }
        public double Fitness { get; set; }

        public Genome(int inputs, int outputs, int hiddenNodes, IActivationFunction<double> function, ref FastRandom rnd)
        {
            this.Random = rnd;
            this.ActivationFunction = function;
            this.Nodes = new NodeGeneCollection();
            int nodenr = 0;
            for (; nodenr < inputs; nodenr++)
            {
                this.Nodes.Add(new NodeGene(nodenr, NodeGene.NodeType.Input));
            }
            for (; nodenr < (inputs + outputs); nodenr++)
            {
                this.Nodes.Add(new NodeGene(nodenr, NodeGene.NodeType.Output));
            }
            for (; nodenr < (inputs + outputs + hiddenNodes); nodenr++)
            {
                this.Nodes.Add(new NodeGene(nodenr, NodeGene.NodeType.Hidden));
            }
            this.Connections = new ConnectionGeneCollection();
        }

        public Genome(NodeGeneCollection nodes, IActivationFunction<double> function, ref FastRandom rnd)
        {
            this.Nodes = new NodeGeneCollection(nodes);
            this.Connections = new ConnectionGeneCollection();
            this.ActivationFunction = function;
            this.Random = rnd;
        }

        public Genome(Genome copyFrom)
        {
            this.Nodes = new NodeGeneCollection(copyFrom.Nodes);
            this.Connections = new ConnectionGeneCollection(copyFrom.Connections);
            this.Random = copyFrom.Random;
            this.ActivationFunction = copyFrom.ActivationFunction;
        }

        /// <summary>
        /// Connect every Input to every hidden node, and every hidden node to every output.
        /// </summary>
        /// <param name="minWeight">Minimal weight of the connections</param>
        /// <param name="maxWeight">Maximal weight of the connections</param>
        public void ConnectLayers(double minWeight, double maxWeight, double probability)
        {
            NodeGene[] inputs = Nodes.Inputs;
            NodeGene[] outputs = Nodes.Outputs;
            NodeGene[] hiddenNodes = Nodes.HiddenNodes;

            if (hiddenNodes.Length > 0)
            {
                for (int i = 0; i < inputs.Length; i++)
                {
                    for (int j = 0; j < hiddenNodes.Length; j++)
                    {
                        if (!Connections.Any(cn => (cn.Value.InputNode == inputs[i].NodeNumber) && (cn.Value.OutputNode == hiddenNodes[j].NodeNumber)))
                        {
                                Connections.Add(new ConnectionGene(InnovationGenerator.NextMutationNumber(inputs[i].NodeNumber, hiddenNodes[j].NodeNumber), inputs[i].NodeNumber, hiddenNodes[j].NodeNumber, Random.NextDouble(minWeight, maxWeight), true));
                        }
                    }
                }
                for (int i = 0; i < hiddenNodes.Length; i++)
                {
                    for (int j = 0; j < outputs.Length; j++)
                    {
                        if (!Connections.Any(cn => (cn.Value.InputNode == hiddenNodes[i].NodeNumber) && (cn.Value.OutputNode == outputs[j].NodeNumber)))
                        {
                            if (Random.NextBool(probability))
                                Connections.Add(new ConnectionGene(InnovationGenerator.NextMutationNumber(hiddenNodes[i].NodeNumber, outputs[j].NodeNumber), hiddenNodes[i].NodeNumber, outputs[j].NodeNumber, Random.NextDouble(minWeight, maxWeight), true));
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < inputs.Length; i++)
                {
                    for (int j = 0; j < outputs.Length; j++)
                    {
                        if (!Connections.Any(cn => (cn.Value.InputNode == inputs[i].NodeNumber) && (cn.Value.OutputNode == outputs[j].NodeNumber)))
                        {
                            if (Random.NextBool(probability))
                                Connections.Add(new ConnectionGene(InnovationGenerator.NextMutationNumber(inputs[i].NodeNumber, outputs[j].NodeNumber), inputs[i].NodeNumber, outputs[j].NodeNumber, Random.NextDouble(minWeight, maxWeight), true));
                        }
                    }
                }
            }
        }

        public NeuralNetwork CreateNetwork(double bias = 0)
        {
            NodeGene[] inputs = Nodes.Inputs;
            NodeGene[] outputs = Nodes.Outputs;
            NodeGene[] hiddenNodes = Nodes.HiddenNodes;

            NeuralNetwork network = new NeuralNetwork(inputs.Length, outputs.Length, 1, hiddenNodes.Length);
            
            for (int i = 0; i < inputs.Length; i++)
            {
                network.Inputs[i] = new InputNeuron(inputs[i].NodeNumber);
            }
            for (int i = 0; i < outputs.Length; i++)
            {
                network.Outputs[i] = new OutputNeuron(outputs[i].NodeNumber, bias, ActivationFunction);
            }
            for (int i = 0; i < hiddenNodes.Length; i++)
            {
                network.DeepNodes[0][i] = new Neuron(hiddenNodes[i].NodeNumber, bias, ActivationFunction);
            }

            var nodes = network.GetAllNodesWithNumbers();

            foreach (var entry in Connections)
            {
                if (entry.Value.Enabled)
                {
                    try
                    {
                        ((INeuralInputNode<double>)nodes[entry.Value.InputNode]).AddOutput((INeuralOutputNode<double>)nodes[entry.Value.OutputNode], entry.Value.Weight);
                    }
                    catch
                    { }
                }
            }

            return network;
        }

        public ManualNeuralNetwork CreateManualNetwork(double bias = 0)
        {
            NodeGene[] inputs = Nodes.Inputs;
            NodeGene[] outputs = Nodes.Outputs;
            NodeGene[] hiddenNodes = Nodes.HiddenNodes;

            ManualNeuralNetwork network = new ManualNeuralNetwork(inputs.Length, outputs.Length, 1, hiddenNodes.Length);

            for (int i = 0; i < inputs.Length; i++)
            {
                network.Inputs[i] = new InputNeuron(inputs[i].NodeNumber);
            }
            for (int i = 0; i < outputs.Length; i++)
            {
                network.Outputs[i] = new ManualOutputNeuron(outputs[i].NodeNumber, bias, ActivationFunction);
            }
            for (int i = 0; i < hiddenNodes.Length; i++)
            {
                network.DeepNodes[0][i] = new ManualNeuron(hiddenNodes[i].NodeNumber, bias, ActivationFunction);
            }

            var nodes = network.GetAllNodesWithNumbers();

            foreach (var entry in Connections)
            {
                if (entry.Value.Enabled)
                {
                    try
                    {
                        ((INeuralInputNode<double>)nodes[entry.Value.InputNode]).AddOutput((INeuralOutputNode<double>)nodes[entry.Value.OutputNode], entry.Value.Weight);
                    }
                    catch
                    { }
                }
            }

            return network;
        }

        /// <summary>
        /// Creates genome from neural net. Warning: All genes are enabled!
        /// </summary>
        /// <param name="network"></param>
        /// <returns></returns>
        public static Genome FromNeuralNet(NeuralNetwork network, ref FastRandom rnd)
        {
            Genome gen = new Genome(network.Inputs.Length, network.Outputs.Length, network.DeepNodes.Sum(l => l.Length), network.Outputs[0].ActivationFunction, ref rnd);

            ISynapse<double>[] cons = network.GetAllConnections();

            ulong innoNr = 0;
            for (int i = 0; i < cons.Length; i++)
            {
                gen.Connections.Add(new ConnectionGene(innoNr++, cons[i].InputNode.NodeNumber, cons[i].OutputNode.NodeNumber, cons[i].Weight, true));
            }

            return gen;
        }

        public void Mutate(ref EvolutionController.SpeciesParameters prms)
        {
            IMutationInfo info = prms.PossibleMutations[Utils.RandomSelectIndex(prms.PossibleMutations.Select(im => im.Probability).ToArray())];

            switch (info.MutationType)
            {
                case MutationType.Connection:
                    AddConnectionMutation(prms.MinimumWeight, prms.MaximumWeight);
                    break;

                case MutationType.Node:
                    NodeMutation(prms.MinimumWeight, prms.MaximumWeight);
                    break;

                case MutationType.Weight:
                    WeightMutation((WeightMutationInfo)info, prms.MinimumWeight, prms.MaximumWeight);
                    break;

                case MutationType.RemoveConnection:
                    RemoveConnectionMutation();
                    break;

                case MutationType.AddInput:
                    AddInputMutation();
                    break;
            }
        }

        public void AddConnectionMutation(double minWeight, double maxWeight)
        {
            //int input = Random.Next(0, Nodes._inputs + Nodes._outputs + Nodes._hiddens);

            List<NodeGene> possibleInputs = new List<NodeGene>(this.Nodes.Inputs);
            possibleInputs.AddRange(this.Nodes.HiddenNodes);
            int maxInput = possibleInputs.Max(f => f.NodeNumber);

            NodeGene input = possibleInputs[Random.Next(possibleInputs.Count)];

            while (input.NodeNumber >= maxInput)
                input = possibleInputs[Random.Next(possibleInputs.Count)];

            //Input nodes cannot be outputs
            //while ((input >= Nodes._inputs) && (input < Nodes._inputs + Nodes._outputs))
            //    input = Random.Next(0, Nodes._inputs + Nodes._outputs + Nodes._hiddens);

            List<NodeGene> possibleOutputs = new List<NodeGene>(this.Nodes.Outputs);

            //if (input.Type == NodeGene.NodeType.Input)
            possibleOutputs.AddRange(this.Nodes.HiddenNodes);

            NodeGene output = possibleOutputs[Random.Next(possibleOutputs.Count)];

            //Make sure node doesn't make connections to itself and no cyclic networks are created
            while ((output.Type == NodeGene.NodeType.Hidden) && (output.NodeNumber <= input.NodeNumber))
                output = possibleOutputs[Random.Next(possibleOutputs.Count)];

            //input is an input neuron
            //if (input < Nodes._inputs)
            //{
            //    output = Nodes._inputs + Random.Next(0, Nodes._hiddens + Nodes._outputs);
            //}
            ////input is a hidden neuron
            //else
            //{
            //    output = Random.Next(Nodes._inputs, Nodes._inputs + Nodes._outputs);
            //}

            if (!ConnectionExists(input.NodeNumber, output.NodeNumber))
                Connections.Add(new ConnectionGene(InnovationGenerator.NextMutationNumber(input.NodeNumber, output.NodeNumber), input.NodeNumber, output.NodeNumber, Random.NextDouble(minWeight, maxWeight), true));
        }

        public bool ConnectionExists(int id1, int id2)
        {
            foreach (var kvp in Connections)
            {
                if (((kvp.Value.InputNode == id1) && (kvp.Value.OutputNode == id2)) || ((kvp.Value.InputNode == id2) && (kvp.Value.OutputNode == id1)))
                    return true;
            }
            return false;
        }

        public void NodeMutation(double minWeight, double maxWeight)
        {
            if (this.Connections.Count == 0) return;

            int index = Random.Next(0, Connections.Genes.Length);
            ConnectionGene toSplit = Connections.Genes[index];
            //int newNode = this.Nodes._inputs + this.Nodes._outputs + this.Nodes._hiddens;
            int newNode = this.Nodes.NextNodeNumber();
            //this.Nodes = new NodeGenes(this.Nodes._inputs, this.Nodes._outputs, this.Nodes._hiddens + 1);
            this.Nodes.Add(new NodeGene(newNode, NodeGene.NodeType.Hidden));

            ConnectionGene input = new ConnectionGene(InnovationGenerator.NextMutationNumber(toSplit.InputNode, newNode), toSplit.InputNode, newNode, 1, true); //SharpNeat approach: set to toSplit.Weight
            ConnectionGene output = new ConnectionGene(InnovationGenerator.NextMutationNumber(newNode, toSplit.OutputNode), newNode, toSplit.OutputNode, toSplit.Weight, true); //SharpNeat approach: set to maxWeight

            this.Connections[toSplit.InnovationNumber].Enabled = false;

            this.Connections.Add(input);
            this.Connections.Add(output);
        }

        public void WeightMutation(WeightMutationInfo info, double minWeight, double maxWeight)
        {
            if (this.Connections.Count == 0) return;

            WeightMutationInfo.WeightMutation mutation = info.GetRandomMutation();

            ZigguratGaussianSampler sampler = new ZigguratGaussianSampler(Random);

            ConnectionGene[] selected;
            int count = (int)(mutation.Selection.Type == WeightMutationInfo.Selection.SelectionType.Absolute ? mutation.Selection.Value : (mutation.Selection.Value * (double)Connections.Count));
            selected = new ConnectionGene[count];

            ConnectionGene[] all = Connections.Values.ToArray();
            for (int i = 0; i < count; i++)
            {
                do
                {
                    selected[i] = all[Random.Next(all.Length)];
                }
                while (selected[i].IsMutated);
            }
            all = null;

            switch (mutation.MutationType)
            { 
                case WeightMutationInfo.WeightMutationType.JiggleNormalDistribution:
                    for (int i = 0; i < count; i++)
                    {
                        Connections[selected[i].InnovationNumber].Weight = CheckWeight(selected[i].Weight + sampler.NextSample(0, mutation.Parameter), minWeight, maxWeight);
                        Connections[selected[i].InnovationNumber].IsMutated = true;
                    }
                    break;

                case WeightMutationInfo.WeightMutationType.Random:
                    for (int i = 0; i < count; i++)
                    {
                        Connections[selected[i].InnovationNumber].Weight = CheckWeight(Random.NextDouble(minWeight, maxWeight), minWeight, maxWeight);
                        Connections[selected[i].InnovationNumber].IsMutated = true;
                    }
                    break;

                case WeightMutationInfo.WeightMutationType.JiggleEven:
                    for (int i = 0; i < count; i++)
                    {
                        Connections[selected[i].InnovationNumber].Weight = CheckWeight((selected[i].Weight + (Random.Next() * 2 - 1.0)) * mutation.Parameter, minWeight, maxWeight);
                        Connections[selected[i].InnovationNumber].IsMutated = true;
                    }
                    break;
            }
        }

        public void AddInputMutation()
        {
            if (this.Connections.Count == 0) return;

            NodeGene input = new NodeGene(this.Nodes.NextNodeNumber(), NodeGene.NodeType.Input);
            NodeGene output = this.Nodes.HiddenNodes[Random.Next(this.Nodes.HiddenNodes.Length)];

            ConnectionGene connection = new ConnectionGene(InnovationGenerator.NextMutationNumber(input.NodeNumber, output.NodeNumber), input.NodeNumber, output.NodeNumber, 1, true);

            this.Nodes.Add(input);
            this.Connections.Add(connection);
        }

        public double CheckWeight(double weight, double min, double max)
        {
            if (weight < min) return min;
            else if (weight > max) return max;
            else return weight;
        }

        public void RemoveConnectionMutation()
        {
            Connections.Remove(Connections.Values.ToArray()[Random.Next(Connections.Count)].InnovationNumber);
        }

        public Genome Crossover(Genome partner)
        {
            bool iAmFitter = (this.Fitness > partner.Fitness);
            if (this.Fitness == partner.Fitness) iAmFitter = Random.NextBool();
            Genome offspring;

            //NodeGeneCollection nodes = (iAmFitter) ? this.Nodes : partner.Nodes;
            NodeGeneCollection nodes = new NodeGeneCollection();
            var importantNodes = this.Nodes.Where(n => n.Value.Type != NodeGene.NodeType.Hidden); //Add all inputs/outputs
            foreach (var node in importantNodes)
            {
                nodes.Add(node.Value);
            }

            //if (iAmFitter)
            offspring = new Genome(nodes, (iAmFitter) ? this.ActivationFunction : partner.ActivationFunction, ref this.Random);
            //else
            //    offspring = new Genome(partner.Nodes._inputs, partner.Nodes._outputs, partner.Nodes._hiddens, partner.ActivationFunction, ref partner.Random);

            CrossoverGenesInformation genes = GetGenesInformation(partner);

            int length = genes.MatchingGenes.Length;
            for (int i = 0; i < length; i++)
            {
                offspring.Connections.Add(new ConnectionGene(Random.NextBool() ? genes.MatchingGenes[i].Gene1 : genes.MatchingGenes[i].Gene2));
            }

            length = genes.DisjointGenes.Length;
            if (iAmFitter)
            {
                for (int i = 0; i < length; i++)
                {
                    if (this.Connections.ContainsKey(genes.DisjointGenes[i].InnovationNumber))
                       offspring.Connections.Add(new ConnectionGene(genes.DisjointGenes[i]));
                }

                length = genes.ExcessGenes.Length;
                for (int i = 0; i < length; i++)
                {
                    if (this.Connections.ContainsKey(genes.ExcessGenes[i].InnovationNumber))
                        offspring.Connections.Add(new ConnectionGene(genes.ExcessGenes[i]));
                }
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    if (partner.Connections.ContainsKey(genes.DisjointGenes[i].InnovationNumber))
                        offspring.Connections.Add(new ConnectionGene(genes.DisjointGenes[i]));
                }

                length = genes.ExcessGenes.Length;
                for (int i = 0; i < length; i++)
                {
                    if (partner.Connections.ContainsKey(genes.ExcessGenes[i].InnovationNumber))
                        offspring.Connections.Add(new ConnectionGene(genes.ExcessGenes[i]));
                }
            }

            foreach (var gene in offspring.Connections)
            {
                if (!offspring.Nodes.ContainsKey(gene.Value.InputNode))
                {
                    if (this.Nodes.ContainsKey(gene.Value.InputNode))
                        offspring.Nodes.Add(new NodeGene(this.Nodes[gene.Value.InputNode]));
                    else
                        offspring.Nodes.Add(new NodeGene(partner.Nodes[gene.Value.InputNode]));
                }
                if (!offspring.Nodes.ContainsKey(gene.Value.OutputNode))
                {
                    if (this.Nodes.ContainsKey(gene.Value.OutputNode))
                        offspring.Nodes.Add(new NodeGene(this.Nodes[gene.Value.OutputNode]));
                    else
                        offspring.Nodes.Add(new NodeGene(partner.Nodes[gene.Value.OutputNode]));
                }
            }

            return offspring;
        }

        public enum MutationType
        { 
            Connection,
            Node,
            Weight,
            RemoveConnection,
            AddInput
        }

        public CrossoverGenesInformation.MatchingGene[] MatchinGenes(Genome partner)
        {
            ulong myMaxNumber = (this.Connections.Count > 0) ? this.Connections.InnovationNumbers.Max() : 0;
            ulong partnerMaxNumber = (partner.Connections.Count > 0) ? partner.Connections.InnovationNumbers.Max() : 0;
            Dictionary<ulong, CrossoverGenesInformation.MatchingGene> matching = new Dictionary<ulong, CrossoverGenesInformation.MatchingGene>();

            foreach (var gene in this.Connections)
            {
                if ((gene.Key <= partnerMaxNumber) && (partner.Connections.ContainsInnovationNumber(gene.Key)) && (!matching.ContainsKey(gene.Key)))
                {
                    matching.Add(gene.Key, new CrossoverGenesInformation.MatchingGene(gene.Value, partner.Connections[gene.Key]));
                }
            }
            foreach (var gene in partner.Connections)
            {
                if ((gene.Key <= myMaxNumber) && (this.Connections.ContainsInnovationNumber(gene.Key)) && (!matching.ContainsKey(gene.Key)))
                {
                    matching.Add(gene.Key, new CrossoverGenesInformation.MatchingGene(gene.Value, this.Connections[gene.Key]));
                }
            }

            return matching.Values.ToArray();
        }

        public ConnectionGene[] DisjointGenes(Genome partner)
        {
            ulong myMaxNumber = this.Connections.InnovationNumbers.Max();
            ulong partnerMaxNumber = partner.Connections.InnovationNumbers.Max();
            Dictionary<ulong, ConnectionGene> disjoint = new Dictionary<ulong,ConnectionGene>();

            foreach (var gene in this.Connections)
            {
                if ((gene.Key <= partnerMaxNumber) && (!partner.Connections.ContainsInnovationNumber(gene.Key)) && (!disjoint.ContainsKey(gene.Key)))
                {
                    disjoint.Add(gene.Key, gene.Value);
                }
            }
            foreach (var gene in partner.Connections)
            {
                if ((gene.Key <= myMaxNumber) && (!this.Connections.ContainsInnovationNumber(gene.Key)) && (!disjoint.ContainsKey(gene.Key)))
                {
                    disjoint.Add(gene.Key, gene.Value);
                }
            }

            return disjoint.Values.ToArray();
        }

        public ConnectionGene[] ExcessGenes(Genome partner)
        {
            ulong myMaxNumber = this.Connections.InnovationNumbers.Max();
            ulong partnerMaxNumber = partner.Connections.InnovationNumbers.Max();
            Dictionary<ulong, ConnectionGene> excess = new Dictionary<ulong, ConnectionGene>();

            foreach (var gene in this.Connections)
            {
                if ((gene.Key > partnerMaxNumber) && (!excess.ContainsKey(gene.Key)))
                {
                    excess.Add(gene.Key, gene.Value);
                }
            }

            foreach (var gene in partner.Connections)
            {
                if ((gene.Key > myMaxNumber) && (!excess.ContainsKey(gene.Key)))
                {
                    excess.Add(gene.Key, gene.Value);
                }
            }

            return excess.Values.ToArray();
        }

        /// <summary>
        /// Returns matching, excess, and disjoint genes.
        /// </summary>
        /// <param name="partner"></param>
        /// <returns></returns>
        public CrossoverGenesInformation GetGenesInformation(Genome partner)
        {
            ulong myMaxNumber = (this.Connections.Count > 0) ? this.Connections.InnovationNumbers.Max() : 0;
            ulong partnerMaxNumber = (partner.Connections.Count > 0) ? partner.Connections.InnovationNumbers.Max() : 0;
            Dictionary<ulong, CrossoverGenesInformation.MatchingGene> matching = new Dictionary<ulong, CrossoverGenesInformation.MatchingGene>();
            Dictionary<ulong, ConnectionGene> disjoint = new Dictionary<ulong, ConnectionGene>();
            Dictionary<ulong, ConnectionGene> excess = new Dictionary<ulong, ConnectionGene>();

            foreach (var gene in this.Connections)
            {
                if ((gene.Key <= partnerMaxNumber) && (!matching.ContainsKey(gene.Key)))
                {
                    if (partner.Connections.ContainsInnovationNumber(gene.Key))
                    {
                        if (!matching.ContainsKey(gene.Key))
                            matching.Add(gene.Key, new CrossoverGenesInformation.MatchingGene(gene.Value, partner.Connections[gene.Key]));
                    }
                    else if (!disjoint.ContainsKey(gene.Key))
                    {
                        disjoint.Add(gene.Key, gene.Value);
                    }
                }
                else if ((gene.Key > partnerMaxNumber) && (!excess.ContainsKey(gene.Key)))
                {
                    excess.Add(gene.Key, gene.Value);
                }
            }

            foreach (var gene in partner.Connections)
            {
                if ((gene.Key <= myMaxNumber) && (!matching.ContainsKey(gene.Key)))
                {
                    if (this.Connections.ContainsInnovationNumber(gene.Key))
                    {
                        if (!matching.ContainsKey(gene.Key))
                            matching.Add(gene.Key, new CrossoverGenesInformation.MatchingGene(gene.Value, this.Connections[gene.Key]));
                    }
                    else if (!disjoint.ContainsKey(gene.Key))
                    {
                        disjoint.Add(gene.Key, gene.Value);
                    }
                }
                else if ((gene.Key > myMaxNumber) && (!excess.ContainsKey(gene.Key)))
                {
                    excess.Add(gene.Key, gene.Value);
                }
            }

            return new CrossoverGenesInformation(matching.Values.ToArray(), disjoint.Values.ToArray(), excess.Values.ToArray());
        }

        public double AverageWeightDifference(Genome partner)
        {
            var matchingGenes = MatchinGenes(partner);

            if (matchingGenes.Length == 0) return 0;

            var diffs2 = matchingGenes.Select(mg => Math.Abs(mg.Gene2.Weight - mg.Gene1.Weight));

            //var myMatchingGenes = this.Connections.Values.Where(cn => partner.Connections.ContainsInnovationNumber(cn.InnovationNumber));
            //var differences = myMatchingGenes.Select(gene => Math.Abs(gene.Weight - partner.Connections[gene.InnovationNumber].Weight));

            return (diffs2.Sum() / diffs2.Count());
        }

        public double CompatibilityDistance(Genome partner, double c1, double c2, double c3)
        {
            var D = ((this.Connections.Count > 0) && (partner.Connections.Count > 0)) ? this.DisjointGenes(partner).Length : 0;
            var E = ((this.Connections.Count > 0) && (partner.Connections.Count > 0)) ? this.ExcessGenes(partner).Length : 0;

            int N = 1;
            if ((this.Connections.Count >= 20) && (partner.Connections.Count >= 20))
            {
                N = Math.Max(this.Connections.Count, partner.Connections.Count);
            }

            double W = this.AverageWeightDifference(partner);

            var distance = ((c1 * E) / N) + ((c2 * D) / N) + (c3 * W);

            return distance;
        }

        public override string ToString()
        {
            //{i1-h1-o1-...|sigmoid|[true:0.3:1-2][-3.5:5-3]...}
            StringBuilder builder = new StringBuilder();
            //builder.Append('{');
            foreach (var kvp in this.Nodes)
            {
                builder.Append(kvp.Value.Type.ToString().ToLower()[0]);
                builder.Append(kvp.Value.NodeNumber);
                builder.Append('-');
            }
            builder.Remove(builder.Length - 1, 1);

            builder.Append('|');
            builder.Append(this.ActivationFunction.ToString());
            builder.Append('|');

            foreach (var kvp in this.Connections)
            {
                builder.Append('[');
                builder.Append(kvp.Value.Enabled.ToString().ToLower());
                builder.Append(':');
                builder.Append(kvp.Value.InnovationNumber);
                builder.Append(':');
                builder.Append(kvp.Value.Weight);
                builder.Append(':');
                builder.Append(kvp.Value.InputNode);
                builder.Append('-');
                builder.Append(kvp.Value.OutputNode);
                builder.Append(']');
            }

            //builder.Append('{');

            return builder.ToString();
        }

        public static Genome FromString(string representation)
        {
            //{i1-h1-o1-...|sigmoid|[true:1:0.3:1-2][true:2:-3.5:5-3]...}
            //Genome toReturn = new Genome(
            string[] mainParts = representation.Split(new char[] { '|', '{', '}' }, StringSplitOptions.RemoveEmptyEntries);

            if (mainParts.Length != 3)
                throw new FormatException("String is in wrong format!");

            string[] nodes = mainParts[0].Split(new char[] { '-' });
            NodeGeneCollection genes = new NodeGeneCollection();
            for (int i = 0; i < nodes.Length; i++)
            {
                genes.Add(NodeGene.FromString(nodes[i]));
            }

            IActivationFunction<double> function = ActivationFunctions.FromName(mainParts[1]);

            var matches = Regex.Matches(mainParts[2], @"(?<=\[).+?(?=\])");

            FastRandom random = new FastRandom();
            Genome g = new Genome(genes, function, ref random);
            
            for (int i = 0; i < matches.Count; i++)
            {
                string[] conParts = matches[i].ToString().Split(new char[] { ':' });
                string[] io = conParts[3].Split(new char[] { '-' });
                ConnectionGene gene = new ConnectionGene(ulong.Parse(conParts[1]), int.Parse(io[0]), int.Parse(io[1]), double.Parse(conParts[2]), bool.Parse(conParts[0]));
                g.Connections.Add(gene);
            }

            return g;
        }

        public string OldToString()
        {
            StringBuilder representator = new StringBuilder();
            representator.Append(Nodes.Inputs.Length);
            representator.Append("-");
            representator.Append(Nodes.Outputs.Length);
            representator.Append("-");
            representator.Append(Nodes.HiddenNodes.Length);
            representator.Append("-");
            representator.Append(ActivationFunction.ToString());
            representator.Append("|");
            foreach (var gene in Connections)
            {
                representator.Append(gene.Value.InnovationNumber);
                representator.Append(".");
                representator.Append(gene.Value.InputNode);
                representator.Append(".");
                representator.Append(gene.Value.OutputNode);
                representator.Append(".");
                representator.Append(gene.Value.Weight);
                representator.Append(".");
                representator.Append(gene.Value.Enabled ? 1 : 0);
                representator.Append("-");
            }
            representator.Remove(representator.Length - 1, 1);
            return representator.ToString();
        }

        public static Genome OldFromString(string representation, ref FastRandom rnd)
        { 
            string[] baseString = representation.Split(new char[] { '|' });
            string[] nodes = baseString[0].Split(new char[] { '-' });

            Genome genome = new Genome(int.Parse(nodes[0]), int.Parse(nodes[1]), int.Parse(nodes[2]), ActivationFunctions.FromName(nodes[3]), ref rnd);

            string[] genes = baseString[1].Split(new char[] { '-' });

            for (int i = 1; i < baseString.Length; i++)
            {
                string[] geneString = baseString[i].Split(new char[] { '.' });
                genome.Connections.Add(new ConnectionGene(ulong.Parse(geneString[0]), int.Parse(geneString[1]), int.Parse(geneString[2]), double.Parse(geneString[3]), geneString[4] == "1"));
            }

            return genome;
        }

        public class CrossoverGenesInformation
        {
            public readonly MatchingGene[] MatchingGenes;
            public readonly ConnectionGene[] DisjointGenes;
            public readonly ConnectionGene[] ExcessGenes;

            public CrossoverGenesInformation(MatchingGene[] matching, ConnectionGene[] disjoint, ConnectionGene[] excess)
            {
                this.MatchingGenes = matching;
                this.DisjointGenes = disjoint;
                this.ExcessGenes = excess;
            }

            public class MatchingGene
            {
                /// <summary>
                /// The Gene from parent 1.
                /// </summary>
                public readonly ConnectionGene Gene1;
                /// <summary>
                /// The Gene from parent 2.
                /// </summary>
                public readonly ConnectionGene Gene2;

                public MatchingGene(ConnectionGene gene1, ConnectionGene gene2)
                {
                    this.Gene1 = gene1;
                    this.Gene2 = gene2;
                }
            }
        }
    }
}