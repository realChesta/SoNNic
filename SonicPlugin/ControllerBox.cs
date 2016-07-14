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
                a = value;
                this.Refresh();
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
                b = value;
                this.Refresh();
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
                c = value;
                this.Refresh();
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
                up = value;
                this.Refresh();
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
                down = value;
                this.Refresh();
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
                left = value;
                this.Refresh();
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
                right = value;
                this.Refresh();
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
            if (a)
                g.FillRectangle(ButtonBrush, new Rectangle(210, 68, 19, 19));
            if (b)                                            
                g.FillRectangle(ButtonBrush, new Rectangle(235, 62, 19, 19));
            if (c)                                            
                g.FillRectangle(ButtonBrush, new Rectangle(260, 55, 19, 19));
            if (up)
                g.FillRectangle(PadBrush, new Rectangle(53, 55, 19, 19));
            if (down)                                      
                g.FillRectangle(PadBrush, new Rectangle(53, 93, 19, 19));
            if (left)                                      
                g.FillRectangle(PadBrush, new Rectangle(35, 74, 19, 19));
            if (right)                                     
                g.FillRectangle(PadBrush, new Rectangle(72, 74, 19, 19));
        }
    }
}