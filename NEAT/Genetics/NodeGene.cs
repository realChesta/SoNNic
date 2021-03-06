﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEAT.Genetics
{
    public class NodeGene
    {
        public int NodeNumber { get; private set; }
        public NodeType Type { get; private set; }

        public NodeGene(int number, NodeType type)
        {
            this.NodeNumber = number;
            this.Type = type;
        }
        public NodeGene(NodeGene copyFrom)
        {
            this.NodeNumber = copyFrom.NodeNumber;
            this.Type = copyFrom.Type;
        }

        public enum NodeType
        { 
            Input,
            Output,
            Hidden
        }

        public static NodeGene FromString(string gene)
        {
            NodeType type;
            switch (gene[0])
            { 
                case 'i':
                    type = NodeType.Input;
                    break;

                case 'o':
                    type = NodeType.Output;
                    break;

                case 'h':
                    type = NodeType.Hidden;
                    break;

                default:
                    throw new FormatException("Invalid format!");
            }
            return new NodeGene(int.Parse(gene.Substring(1)), type);
        }

        public override string ToString()
        {
            return this.Type.ToString().ToLower()[0] + this.NodeNumber.ToString();
        }
    }
}
