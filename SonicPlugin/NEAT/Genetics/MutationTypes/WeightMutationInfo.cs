using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEAT.Genetics
{
    public class WeightMutationInfo : IMutationInfo
    {
        public List<WeightMutation> Mutations = new List<WeightMutation>();

        public Genome.MutationType MutationType { get; private set; }

        public double Probability { get; set; }

        public WeightMutationInfo()
            : this(EvolutionController.SpeciesParameters.DefaultConnectionWeightMutationProbability)
        { }
        public WeightMutationInfo(double probability)
        {
            this.MutationType = Genome.MutationType.Weight;
            this.Probability = probability;
        }

        public WeightMutation GetRandomMutation()
        {
            if (Mutations.Count < 1)
                throw new InvalidOperationException("No mutations exist in the list!");
            else
                return Mutations[Utils.RandomSelectIndex(Mutations.Select(m => m.Probability).ToArray())];
        }

        public class WeightMutation
        {
            public WeightMutationType MutationType;
            public Selection Selection;
            public double Probability;
            public double Parameter;

            public WeightMutation(WeightMutationType mutationType, double probability, double parameter, Selection selection)
            {
                this.MutationType = mutationType;
                this.Probability = probability;
                this.Parameter = parameter;
                this.Selection = selection;
            }
            public WeightMutation(WeightMutationType mutationType, double probability, double parameter, double selection, Selection.SelectionType selectionType)
            {
                this.MutationType = mutationType;
                this.Probability = probability;
                this.Parameter = parameter;
                this.Selection = new Selection(selection, selectionType);
            }
        }

        public enum WeightMutationType
        {
            Random, //Assign a random weight in range
            JiggleNormalDistribution, // weight change with delta (normal distribution)
            JiggleEven // weight chance with delta (even distribution)
        }

        public class Selection
    {
        public readonly SelectionType Type;
        public double Value;

        public Selection(double value, SelectionType type)
        {
            this.Type = type;
            this.Value = value;
        }

        public static Selection operator *(Selection s1, Selection s2)
        {
            if (s1.Type != s2.Type)
                throw new InvalidOperationException("Selections must be of the same count!");
            else
                return new Selection(s1.Value * s2.Value, s1.Type);
        }

        public static bool operator ==(Selection s1, Selection s2)
        {
            return ((s1.Type == s2.Type) && (s1.Value == s2.Value));
        }

        public static bool operator !=(Selection s1, Selection s2)
        {
            return ((s1.Type != s2.Type) || (s1.Value != s2.Value));
        }

        public override bool Equals(object obj)
        {
            if (obj is Selection)
                return (this == (Selection)obj);
            else
                return base.Equals(obj);
        }

        public static Selection operator +(Selection s1, Selection s2)
        {
            if (s1.Type != s2.Type)
                throw new InvalidOperationException("Selections must be of the same count!");
            else
                return new Selection(s1.Value + s2.Value, s1.Type);
        }

        public static Selection operator -(Selection s1, Selection s2)
        {
            if (s1.Type != s2.Type)
                throw new InvalidOperationException("Selections must be of the same count!");
            else
                return new Selection(s1.Value - s2.Value, s1.Type);
        }

        public static Selection operator /(Selection s1, Selection s2)
        {
            if (s1.Type != s2.Type)
                throw new InvalidOperationException("Selections must be of the same count!");
            else
                return new Selection(s1.Value / s2.Value, s1.Type);
        }

        public enum SelectionType
        { 
            Absolute,
            Proportional
        }
    }
        public override string ToString()
        {
            return "T:" + this.MutationType.ToString() + " P:" + this.Probability.ToString();
        }
    }
}
