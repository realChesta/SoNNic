using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEAT.Genetics
{
    public class ConnectionGeneCollection : Dictionary<ulong, ConnectionGene>
    {
        public ConnectionGeneCollection()
            : base()
        { }

        public ConnectionGeneCollection(IDictionary<ulong, ConnectionGene> dictionary)
            : base()
        {
            foreach (var kvp in dictionary)
            {
                this.Add(new ConnectionGene(kvp.Value));
            }
        }
        public ConnectionGeneCollection(IEnumerable<ConnectionGene> genes)
            : base()
        {
            foreach (ConnectionGene gene in genes)
            {
                this.Add(new ConnectionGene(gene));
            }
        }

        public ConnectionGeneCollection(int capacity)
            : base(capacity)
        { }

        private new Dictionary<ulong, ConnectionGene>.KeyCollection Keys 
        {
            get
            {
                return base.Keys;
            }
        }

        private new Dictionary<ulong, ConnectionGene>.ValueCollection Values
        {
            get
            {
                return base.Values;
            }
        }

        public ulong[] InnovationNumbers
        {
            get
            {
                return base.Keys.ToArray();
            }
        }

        public ConnectionGene[] Genes
        {
            get
            {
                return base.Values.ToArray();
            }
        }

        public new ConnectionGene this[ulong innoNr]
        {
            get
            {
                return base[innoNr];
            }
        }

        public bool TryGetGene(ulong innoNr, out ConnectionGene gene)
        { return base.TryGetValue(innoNr, out gene); }

        public void Add(ConnectionGene gene)
        {
            base.Add(gene.InnovationNumber, gene);
        }
        public new void Add(ulong innoNr, ConnectionGene gene)
        {
            this.Add(gene);
        }

        public new void Clear()
        { base.Clear(); }

        private new bool ContainsKey(ulong nr)
        { return base.ContainsKey(nr); }
        private new bool ContainsValue(ConnectionGene gene)
        { return base.ContainsValue(gene); }

        public bool ContainsInnovationNumber(ulong nr)
        { return base.ContainsKey(nr); }

        public new bool Remove(ulong innoNr)
        { return base.Remove(innoNr); }
    }
}
