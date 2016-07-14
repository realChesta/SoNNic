using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SonicPlugin
{
    public partial class ControllerForm : Form
    {
        public bool A
        {
            get
            {
                return controllerBox.A;
            }
            set
            {
                controllerBox.A = value;
            }
        }
        public bool B
        {
            get
            {
                return controllerBox.B;
            }
            set
            {
                controllerBox.B = value;
            }
        }
        public bool C
        {
            get
            {
                return controllerBox.C;
            }
            set
            {
                controllerBox.C = value;
            }
        }

        public ControllerForm()
        {
            InitializeComponent();
        }

        private void ControllerForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Y:
                    controllerBox.A = true;
                    break;
                case Keys.X:
                    controllerBox.B = true;
                    break;
                case Keys.C:
                    controllerBox.C = true;
                    break;

                case Keys.Up:
                    controllerBox.PadUp = true;
                    break;
                case Keys.Down:
                    controllerBox.PadDown = true;
                    break;
                case Keys.Left:
                    controllerBox.PadLeft = true;
                    break;
                case Keys.Right:
                    controllerBox.PadRight = true;
                    break;
            }
        }

        private void ControllerForm_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Y:
                    controllerBox.A = false;
                    break;
                case Keys.X:
                    controllerBox.B = false;
                    break;
                case Keys.C:
                    controllerBox.C = false;
                    break;

                case Keys.Up:
                    controllerBox.PadUp = false;
                    break;
                case Keys.Down:
                    controllerBox.PadDown = false;
                    break;
                case Keys.Left:
                    controllerBox.PadLeft = false;
                    break;
                case Keys.Right:
                    controllerBox.PadRight = false;
                    break;
            }
        }
    }
}
