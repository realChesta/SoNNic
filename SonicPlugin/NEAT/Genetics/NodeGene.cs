using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace NEAT.Genetics
{
    public class NodeGene
    {
        public int NodeNumber { get; private set; }
        public NodeType Type { get; private set; }

        public Point Position;
        public Size Size;

        public NodeGene(int number, NodeType type) 
            : this(number, type, Point.Empty, System.Drawing.Size.Empty) { }
        public NodeGene(int number, NodeType type, Point pos, Size size)
        {
            this.NodeNumber = number;
            this.Type = type;
            this.Position = pos;
            this.Size = size;
        }
        public NodeGene(NodeGene copyFrom)
        {
            this.NodeNumber = copyFrom.NodeNumber;
            this.Type = copyFrom.Type;
            this.Position = new Point(copyFrom.Position.X, copyFrom.Position.Y);
            this.Size = new Size(copyFrom.Size.Width, copyFrom.Size.Height);
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
            NodeGene toReturn = new NodeGene(int.Parse(gene.Substring(1)), type);

            string[] posAndSize = gene.Substring(2).Split(new char[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries);

            if (posAndSize.Length != 2)
                throw new FormatException("Invalid NodeGene format!");

            string[] pos = posAndSize[0].Split(':');
            toReturn.Position = new Point(int.Parse(pos[0]), int.Parse(pos[1]));

            string[] size = posAndSize[1].Split(':');
            toReturn.Size = new Size(int.Parse(size[0]), int.Parse(size[1]));

            return toReturn;
        }

        public override string ToString()
        {
            //e.g. i1<-5:3><3:5>
            return this.Type.ToString().ToLower()[0] + this.NodeNumber.ToString() + "<" + this.Position.X + ":" + this.Position.Y + "><" + this.Size.Width + ":" + this.Size.Height + ">";
        }
    }
}
