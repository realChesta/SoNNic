using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEAT.NeuralNetworks
{
    public class SynapseCollection<T> : List<ISynapse<T>>
    {
        public SynapseCollection()
            : base()
        { }

        public SynapseCollection(IEnumerable<ISynapse<T>> synapses)
            : base(synapses)
        { }

        public SynapseCollection(int capacity)
            : base(capacity)
        { }

        public new ISynapse<T> this[int index]
        {
            get
            {
                return base[index];
            }
            set
            {
                base[index] = value;
            }
        }

        public static explicit operator SynapseCollection<T>(SynapseCollection<double> v)
        {
            throw new NotImplementedException();
        }

        public new void Add(ISynapse<T> synapse)
        {
            base.Add(synapse);
        }

        public new void AddRange(IEnumerable<ISynapse<T>> synapses)
        {
            base.AddRange(synapses);
        }

        public new void Insert(int index, ISynapse<T> synapse)
        {
            base.Insert(index, synapse);
        }

        public new void InsertRange(int index, IEnumerable<ISynapse<T>> synapses)
        {
            base.InsertRange(index, synapses);
        }

        public new void Remove(ISynapse<T> synapse)
        {
            base.Remove(synapse);
        }

        public new void RemoveAll(Predicate<ISynapse<T>> match)
        {
            base.RemoveAll(match);
        }

        public new void RemoveAt(int index)
        {
            base.RemoveAt(index);
        }

        public new void RemoveRange(int index, int count)
        {
            base.RemoveRange(index, count);
        }
    }
}
