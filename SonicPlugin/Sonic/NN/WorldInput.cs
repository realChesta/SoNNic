using System.Drawing;
using System.Windows;
using SonicPlugin.Sonic.Map;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;
using NEAT.NeuralNetworks;
using System;

namespace SonicPlugin.Sonic.NN
{
    public class WorldInput : INeuralInputNode<double>
    {
        public Point RelativePosition;
        //private MapDrawer CurrentMap;
        public static readonly Color SolidColor = Color.FromArgb(169, 169, 169);
        public readonly Size Size;

        private SynapseCollection<double> _outputs = new SynapseCollection<double>();
        public ISynapse<double>[] Outputs
        {
            get
            {
                return _outputs.ToArray();
            }
        }

        public double InputValue { get; set; }

        public double OutputValue
        {
            get
            {
                return (int)GetValue(SonicHub.SonicPos, SonicHub.CurrentObjects);
            }
        }

        public int NodeNumber { get; private set; }

        public WorldInput(/*ref MapDrawer map, */Point pos, Size size)
        {
            //this.CurrentMap = map;
            this.RelativePosition = pos;
            this.Size = size;
        }
        public WorldInput(/*ref MapDrawer map, */int x, int y, int width, int height)
            : this(/*ref map, */new Point(x, y), new Size(width, height))
        { }

        public Point Position(Point relative)
        {
            return relative + (Size)RelativePosition;
        }

        public CollisionType GetValue(Point sonicPos, SonicObject[] sonicObjects)
        {
            Point position = this.Position(sonicPos);

            if (position.X < 0 || position.Y < 0)
                return 0;

            Rect inputRect = new Rect(position.X, position.Y, Size.Width, Size.Height);

            bool harm = false;
            bool solid = false;
            for (int i = 0; i < sonicObjects.Length; i++)
            {
                SonicObject s = sonicObjects[i];
                if (new Rect(s.Position_X - s.NewHitbox_HorizontalRadius, s.Position_Y - s.NewHitbox_VerticalRadius, s.NewHitbox_HorizontalRadius * 2, s.NewHitbox_VerticalRadius * 2).IntersectsWith(inputRect))
                {
                    solid = (s.ObjectType != SonicObjectType.Ring);
                    if (harm = (s.CollisionResponse == CollisionResponseType.Enemy || s.CollisionResponse == CollisionResponseType.Harm))
                    {
                        break;
                    }
                }
            }

            if (harm)
                return CollisionType.Harm;
            else if (IsCubeSolid(position) || solid)
                return CollisionType.Solid;
            else 
                return CollisionType.None;
        }

        //public bool IsSolid(Point pos)
        //{
        //    return this.IsPointSolid(pos);
        //}

        //private bool IsPointSolid(Point pos)
        //{
        //    //first, get containing 256-chunk

        //    int chunk256x = (int)Math.Floor((double)pos.X / 0x100);
        //    int chunk256y = (int)Math.Floor((double)pos.Y / 0x100);

        //    var chunk256 = CurrentMap.Chunks[chunk256y][chunk256x];

        //    int chunk16x = (int)Math.Floor((double)(pos.X - (chunk256x * 0x100)) / 0x10);
        //    int chunk16y = (int)Math.Floor((double)(pos.Y - (chunk256y * 0x100)) / 0x10);

        //    var chunk16 = chunk256.Chunks[chunk16y][chunk16x];

        //    return chunk16.Solidity != SolidityStatus.NonSolid && chunk16.Solidity != SolidityStatus.Unknown;
        //}

        public bool IsPointSolid(Point pos)
        {
            return SonicHub.CurrentMap.MapBitmap.GetPixel(pos.X, pos.Y).Equals(SolidColor);
        }

        public bool IsCubeSolid(Point pos)
        {
            return
                IsPointSolid(pos) ||
                IsPointSolid(pos + new Size(this.Size.Width, 0)) ||
                IsPointSolid(pos + new Size(0, this.Size.Height)) ||
                IsPointSolid(pos + this.Size);
        }

        public ISynapse<double> AddOutput(INeuralOutputNode<double> neuron, double weight)
        {
            return new Synapse(this, neuron, weight);
        }
        public void AddOutput(ISynapse<double> s)
        {
            _outputs.Add(s);
        }
    }
}
