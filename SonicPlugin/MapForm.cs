using SonicPlugin.Sonic;
using SonicPlugin.Sonic.Map;
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
    public partial class MapForm : Form
    {
        public SonicMap Map { get; private set; }
        public MapDrawer Drawer;

        public MapForm(SonicMap map)
        {
            InitializeComponent();

            this.Map = map;
            this.Drawer = new MapDrawer(((CanvasControl)elementHost1.Child).mainCanvas, this.Map);
        }

        private void MapForm_Shown(object sender, EventArgs e)
        {
            Drawer.DrawMapFast();
        }
    }
}
