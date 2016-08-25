using System.Drawing;
using System.Windows.Forms;

namespace SonicPlugin
{
    public class ControllerBox : PictureBox
    {
        private bool a;
        private bool b;
        private bool c;

        private bool up;
        private bool down;
        private bool left;
        private bool right;

        public bool A
        {
            get
            {
                return a;
            }
            set
            {
                if (value != a)
                {
                    a = value;
                    this.Refresh();
                }
            }
        }
        public bool B
        {
            get
            {
                return b;
            }
            set
            {
                if (value != b)
                {
                    b = value;
                    this.Refresh();
                }
            }
        }
        public bool C
        {
            get
            {
                return c;
            }
            set
            {
                if (value != c)
                {
                    c = value;
                    this.Refresh();
                }
            }
        }

        public bool PadUp
        {
            get
            {
                return up;
            }
            set
            {
                if (value != up)
                {
                    up = value;
                    this.Refresh();
                }
            }
        }
        public bool PadDown
        {
            get
            {
                return down;
            }
            set
            {
                if (value != down)
                {
                    down = value;
                    this.Refresh();
                }
            }
        }
        public bool PadLeft
        {
            get
            {
                return left;
            }
            set
            {
                if (value != left)
                {
                    left = value;
                    this.Refresh();
                }
            }
        }
        public bool PadRight
        {
            get
            {
                return right;
            }
            set
            {
                if (value != right)
                {
                    right = value;
                    this.Refresh();
                }
            }
        }

        private readonly SolidBrush ButtonBrush = new SolidBrush(Color.FromArgb(150, 0, 0));
        private readonly SolidBrush PadBrush = new SolidBrush(Color.FromArgb(100, 0, 0));

        protected override void OnPaint(PaintEventArgs pe)
        {
            pe.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            base.OnPaint(pe);
            DrawButtons(pe.Graphics);
        }

        private void DrawButtons(Graphics g)
        {
            float factor = g.DpiX / 96f;
            int correction = 0;
            if (g.DpiX == 144f)
                correction = 3;

            if (a)
                g.FillRectangle(ButtonBrush, new Rectangle((int)(210 * factor), (int)(68 * factor) + correction, (int)(19 * factor), (int)(19 * factor)));
            if (b)                                            
                g.FillRectangle(ButtonBrush, new Rectangle((int)(235 * factor), (int)(62 * factor) + correction, (int)(19 * factor), (int)(19 * factor)));
            if (c)                                            
                g.FillRectangle(ButtonBrush, new Rectangle((int)(260 * factor), (int)(55 * factor) + correction, (int)(19 * factor), (int)(19 * factor)));
            if (up)
                g.FillRectangle(PadBrush, new Rectangle((int)(53 * factor), (int)(55 * factor) + correction, (int)(19 * factor), (int)(19 * factor)));
            if (down)                                      
                g.FillRectangle(PadBrush, new Rectangle((int)(53 * factor), (int)(93 * factor) + correction, (int)(19 * factor), (int)(19 * factor)));
            if (left)                                      
                g.FillRectangle(PadBrush, new Rectangle((int)(35 * factor), (int)(74 * factor) + correction, (int)(19 * factor), (int)(19 * factor)));
            if (right)                                     
                g.FillRectangle(PadBrush, new Rectangle((int)(72 * factor), (int)(74 * factor) + correction, (int)(19 * factor), (int)(19 * factor)));
        }
    }
}