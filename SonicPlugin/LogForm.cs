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
    public partial class LogForm : Form
    {
        public LogForm()
        {
            InitializeComponent();
        }
        public LogForm(string title)
            : this()
        {
            this.Text = title;
        }

        public void WriteLine(string line)
        {
            logBox.AppendText(line + "\n");
        }
    }
}
