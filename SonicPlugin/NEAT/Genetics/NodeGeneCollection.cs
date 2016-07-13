using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEAT.Genetics
{
    public class NodeGeneCollection : Dictionary<int, NodeGene>
    {
        private List<NodeGene> _inputs = new List<NodeGene>();
        private List<NodeGene> _outputs = new List<NodeGene>();
        private List<NodeGene> _hiddens = new List<NodeGene>();
        public NodeGene[] Inputs
        {
            get { return _inputs.AsReadOnly().ToArray(); }
        }
        public NodeGene[] Outputs
        {
            get { return _outputs.AsReadOnly().ToArray(); }
        }
        public NodeGene[] HiddenNodes
        {
            get { return _hiddens.AsReadOnly().ToArray(); }
        }

        public NodeGeneCollection()
            : base()
        { }

        public NodeGeneCollection(IDictionary<int, NodeGene> dictionary)
            : base()
        {
            foreach (var kvp in dictionary)
            {
                this.Add(new NodeGene(kvp.Value));
            }
        }

        public NodeGeneCollection(int capacity)
            : base(capacity)
        { }

        private new Dictionary<int, NodeGene>.KeyCollection Keys
        {
            get
            {
                return base.Keys;
            }
        }

        private new Dictionary<int, NodeGene>.ValueCollection Values
        {
            get
            {
                return base.Values;
            }
        }

        public int[] NodeNumbers
        {
            get
            {
                return base.Keys.ToArray();
            }
        }

        public NodeGene[] Genes
        {
            get
            {
                return base.Values.ToArray();
            }
        }

        public new NodeGene this[int nodeNr]
        {
            get
            {
                return base[nodeNr];
            }
        }

        public int NextNodeNumber()
        {
            int nr = this.Count;

            while (this.ContainsKey(nr))
                nr++;

            return nr;
        }

        public bool TryGetGene(int nodeNr, out NodeGene gene)
        { return base.TryGetValue(nodeNr, out gene); }

        public void Add(NodeGene gene)
        {
            //if (_outputs.Count >= 1 && gene.Type == NodeGene.NodeType.Output)

            switch (gene.Type)
            {
                case NodeGene.NodeType.Input:
                    this._inputs.Add(gene);
                    break;
                case NodeGene.NodeType.Output:
                    this._outputs.Add(gene);
                    break;
                case NodeGene.NodeType.Hidden:
                    this._hiddens.Add(gene);
                    break;
            }
            base.Add(gene.NodeNumber, gene);
        }

        public new void Clear()
        { base.Clear(); }

        private new bool ContainsKey(int nr)
        { return base.ContainsKey(nr); }
        private new bool ContainsValue(NodeGene gene)
        { return base.ContainsValue(gene); }

        public bool ContainsNodeNumber(int nr)
        { return base.ContainsKey(nr); }

        public new bool Remove(int nodeNr)
        {
            switch (this[nodeNr].Type)
            {
                case NodeGene.NodeType.Input:
                    if (_inputs.Contains(this[nodeNr]))
                        _inputs.Remove(this[nodeNr]);
                    break;

                case NodeGene.NodeType.Output:
                    if (_outputs.Contains(this[nodeNr]))
                        _outputs.Remove(this[nodeNr]);
                    break;

                case NodeGene.NodeType.Hidden:
                    if (_hiddens.Contains(this[nodeNr]))
                        _hiddens.Remove(this[nodeNr]);
                    break;
            }
            return base.Remove(nodeNr);
        }
    }
}
