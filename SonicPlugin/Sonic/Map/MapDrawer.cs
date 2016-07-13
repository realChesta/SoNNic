using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SonicPlugin.Sonic.NN;
using Image = System.Windows.Controls.Image;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace SonicPlugin.Sonic.Map
{
    public class MapDrawer
    {
        private Canvas Canvas;
        private TranslateTransform Transform;
        public SonicMap CurrentMap;

        private SolidColorBrush BlockFill = new SolidColorBrush(Colors.DarkGray);
        private SolidColorBrush ObjectFill = new SolidColorBrush(Colors.Black);
        private SolidColorBrush RedFill = new SolidColorBrush(Colors.Red);
        private SolidColorBrush GreenFill = new SolidColorBrush(Colors.Green);
        private List<Shape> objectShapes = new List<Shape>();
        private Dictionary<byte, CollisionBlock> collisionCache = new Dictionary<byte, CollisionBlock>();
        private Image MapImg;
        public Bitmap MapBitmap;
        //private StreamWriter Log;

        public readonly byte[] SolidColor = { 169, 169, 169 };

        public MapDrawer(Canvas canvas, SonicMap map)
        {
            this.Canvas = canvas;
            this.Transform = new TranslateTransform(0, 0);
            this.Canvas.RenderTransform = Transform;
            this.CurrentMap = map;
        }

        private Bitmap BitmapFromWriteableBitmap(WriteableBitmap writeBmp)
        {
            Bitmap bmp;
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create((BitmapSource)writeBmp));
                enc.Save(outStream);
                bmp = new Bitmap(outStream);
            }
            return bmp;
        }

        public void DrawMap()
        {
            this.Canvas.Children.Clear();

            for (int i = 0; i < CurrentMap.Chunks.Length; i++)
            {
                for (int j = 0; j < CurrentMap.Chunks[i].Length; j++)
                {
                    Draw256Mapping(CurrentMap.Chunks[i][j], i, j);
                }
            }
        }
        public void DrawMap(SonicMap map)
        {
            this.CurrentMap = map;
            this.DrawMap();
        }

        public void DrawMapFast()
        {
            //Log = File.CreateText("mapDrawer.log");

            WriteableBitmap bitmap = new WriteableBitmap(
                this.CurrentMap.Chunks[0].Length * 0x100,
                this.CurrentMap.Chunks.Length * 0x100,
                96, 96, PixelFormats.Bgra32, null);

            for (int i = 0; i < CurrentMap.Chunks.Length; i++)
            {
                for (int j = 0; j < CurrentMap.Chunks[i].Length; j++)
                {
                    Draw256Mapping(CurrentMap.Chunks[i][j], i, j, bitmap);

                    //if (CurrentMap.Chunks[i][j].IsLoop)
                    //    Draw256Mapping(CurrentMap.Chunks[i][j].LoopChunk, i, j, bitmap);
                }
            }

            this.collisionCache.Clear();
            this.Canvas.Children.Clear();

            MapImg = new Image
            {
                Stretch = Stretch.None,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Source = bitmap
            };

            RenderOptions.SetBitmapScalingMode(MapImg, BitmapScalingMode.NearestNeighbor);
            RenderOptions.SetEdgeMode(MapImg, EdgeMode.Aliased);

            MapBitmap = BitmapFromWriteableBitmap(bitmap);

            this.Canvas.Children.Add(MapImg);

            //Log.Close();
        }
        public void DrawMapFast(SonicMap map)
        {
            this.CurrentMap = map;
            this.DrawMapFast();
        }

        private void Draw256Mapping(Mapping256x256 mapping, int line, int row)
        {
            if (mapping.Index != 0x00)
            {
                for (int i = 0; i < mapping.Chunks.Length; i++)
                {
                    for (int j = 0; j < mapping.Chunks[i].Length; j++)
                    {
                        int x = (row * 256) + (j * 16);
                        int y = (line * 256) + (i * 16);

                        Draw16Mapping(mapping.Chunks[i][j], x, y);
                    }
                }
            }
        }
        private void Draw256Mapping(Mapping256x256 mapping, int line, int row, WriteableBitmap bitmap)
        {
            if (mapping.Index != 0x00)
            {
                for (int i = 0; i < mapping.Chunks.Length; i++)
                {
                    for (int j = 0; j < mapping.Chunks[i].Length; j++)
                    {
                        int x = (row * 256) + (j * 16);
                        int y = (line * 256) + (i * 16);

                        Draw16Mapping(mapping.Chunks[i][j], x, y, bitmap);
                    }
                }
            }
        }

        private void Draw16Mapping(Mapping16x16 mapping, int x, int y)
        {
            switch (mapping.Solidity)
            { 
                case SolidityStatus.NonSolid:
                case SolidityStatus.Unknown:
                    break;

                default:
                    {
                        Rectangle rect = new Rectangle 
                        {
                            Width = 16,
                            Height = 16,
                            Fill = BlockFill
                        };
                        Canvas.SetLeft(rect, x);
                        Canvas.SetTop(rect, y);
                        this.Canvas.Children.Add(rect);
                    }
                    break;
            }
        }
        private void Draw16Mapping(Mapping16x16 mapping, int x, int y, WriteableBitmap bitmap)
        {
            switch (mapping.Solidity)
            { 
                case SolidityStatus.NonSolid:
                case SolidityStatus.Unknown:
                    return;
            }

            byte cId = CurrentMap.CollisionMap.GetCollisionID(mapping.BlockReferenceID);

            CollisionBlock block;

            if (!collisionCache.ContainsKey(cId))
            {
                block = CurrentMap.CollisionMap.GetCollisionBlock(cId);
                collisionCache.Add(cId, block);
            }
            else
            {
                block = collisionCache[cId];
            }

            for (int i = 0; i < block.data.Length; i++)
            {
                byte d = block.data[i];

                if (d != 0x00)
                {
                    bool positive = (d <= 0x10) && (d > 0x00);

                    //Log.WriteLine("======================");
                    //Log.WriteLine("x: " + x);
                    //Log.WriteLine("y: " + y);
                    //Log.WriteLine("d[" + i + "]: " + d);
                    int h = positive ? d : (0xFF - d);
                    //Log.WriteLine("h: " + h);

                    int yr = (mapping.VerticalFlip ? !positive : positive) ? ((y + 0x10) - h) : y; //get starting point - based on whether the solidity is positive or negative (starting from above or from the ground)
                    //Log.WriteLine("yr: " + yr);
                    int xr = mapping.HorizontalFlip ? (x + (0x0F - i)) : (x + i);

                    Int32Rect rect = new Int32Rect(xr, yr, 1, h);
                    byte[] pixels = new byte[h * 4];

                    // Setup the pixel array
                    for (int j = 0; j < h; ++j)
                    {
                        pixels[j * 4 + 0] = 169;   // Blue
                        pixels[j * 4 + 1] = 169;   // Green
                        pixels[j * 4 + 2] = 169;   // Red
                        pixels[j * 4 + 3] = 255;   // Alpha
                    }

                    //Log.WriteLine("Writing pixels...");

                    bitmap.WritePixels(rect, pixels, 4, 0);
                }
            }
        }

        public void DrawObjects(SonicObject[] objects, bool clearObjects = true)
        {
            if (clearObjects)
                this.ClearObjects();

            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].NewHitbox_HorizontalRadius > 0 && objects[i].NewHitbox_VerticalRadius > 0)
                {
                    Rectangle e = new Rectangle
                    {
                        Width = objects[i].NewHitbox_HorizontalRadius * 2,
                        Height = objects[i].NewHitbox_VerticalRadius * 2,
                        Fill = (objects[i].CollisionResponse == CollisionResponseType.Harm || objects[i].CollisionResponse == CollisionResponseType.Enemy) ? RedFill : ObjectFill
                    };

                    Canvas.SetLeft(e, objects[i].Position_X - objects[i].NewHitbox_HorizontalRadius);
                    Canvas.SetTop(e, objects[i].Position_Y - objects[i].NewHitbox_VerticalRadius);

                    this.Canvas.Children.Add(e);
                    objectShapes.Add(e);

                    if (objects[i].ObjectType == SonicObjectType.Sonic)
                    {
                        //MessageBox.Show("H: " + Canvas.ActualHeight + ", W: " + Canvas.ActualWidth);
                        CenterOn(objects[i].Position_X, objects[i].Position_Y);
                    }
                }
            }
        }

        public void DrawCheckPoint(WorldInput input, System.Drawing.Point sonicPos, SonicObject[] sonicObjects)
        {
            var position = input.Position(sonicPos);
            Rectangle checkPoint = new Rectangle
            {
                Width = input.Size.Width,
                Height = input.Size.Height,
                //Fill = input.IsCubeSolid(position) ? RedFill : ObjectFill
            };

            CollisionType response = input.GetValue(sonicPos, sonicObjects);

            switch (response)
            {
                case CollisionType.Solid:
                    checkPoint.Fill = GreenFill;
                    break;
                case CollisionType.Harm:
                    checkPoint.Fill = RedFill;
                    break;
                default:
                    checkPoint.Fill = ObjectFill;
                    break;
            }

            Canvas.SetLeft(checkPoint, position.X);
            Canvas.SetTop(checkPoint, position.Y);

            this.Canvas.Children.Add(checkPoint);
            objectShapes.Add(checkPoint);
        }

        public void CenterOn(int x, int y)
        {
            Transform.X = -x + (Canvas.ActualWidth / 2);
            Transform.Y = -y + (Canvas.ActualHeight / 2);
        }

        public void ClearObjects()
        {
            foreach (Shape s in objectShapes)
            {
                this.Canvas.Children.Remove(s);
            }
            this.objectShapes.Clear();
        }
    }
}
