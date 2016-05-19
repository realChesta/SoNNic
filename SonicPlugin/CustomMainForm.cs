using BizHawk.Client.Common;
using BizHawk.Emulation.Common;
using SonicPlugin;
using SonicPlugin.Sonic;
using System;
using System.Drawing;
using System.Windows.Forms;
using SonicPlugin.Sonic.NN;

namespace BizHawk.Client.EmuHawk
{
    public partial class CustomMainForm : Form, IExternalToolForm
    {
        [RequiredService]
        internal IMemoryDomains _memoryDomains { get; set; }
        [RequiredService]
        private IEmulator _emu { get; set; }

        private WatchList _watches;
        private MapForm Map;
        private LogForm log;

        private bool includeReserved = false;

        private WorldInput CheckPointInput;
        private WorldInput[] CheckPoints;

        public CustomMainForm()
        {
            InitializeComponent();
            log = new LogForm("Log: objects");
            log.Show();
        }

        public bool AskSaveChanges()
        {
            return true;
        }

        public void FastUpdate()
        { }

        public void Restart()
        {
            if (Global.Game.Name != "Null")
            {
                //first initialization of WatchList
                if (_watches == null)
                {
                    _watches = new WatchList(_memoryDomains, _emu.SystemId ?? string.Empty);

                    //Create some watch
                    Watch myFirstWatch = Watch.GenerateWatch(_memoryDomains.MainMemory, 0xD008, WatchSize.Word, BizHawk.Client.Common.DisplayType.Unsigned, true);
                    Watch mySecondWatch = Watch.GenerateWatch(_memoryDomains.MainMemory, 0xD00C, WatchSize.Word, BizHawk.Client.Common.DisplayType.Unsigned, true);
                    //Watch myThirdWatch = Watch.GenerateWatch(_memoryDomains.MainMemory, 0x60, WatchSize.DWord, BizHawk.Client.Common.DisplayType.Hex, true);

                    //MessageBox.Show("MemoryDomains:\n" + string.Join("\n", _memoryDomains.Select(m => m.Name)));

                    //add them into the list
                    _watches.Add(myFirstWatch);
                    _watches.Add(mySecondWatch);
                    //_watches.Add(myThirdWatch)
                }
                //refresh it
                else
                {
                    _watches.RefreshDomains(_memoryDomains);
                }
            }
        }

        public bool UpdateBefore
        {
            get { return false; }
        }

        public void UpdateValues()
        {
            if (Global.Game.Name != "Null")
            {
                //we update our watches
                _watches.UpdateValues();
                label_Watch1.Text = "X: " + _watches[0].ValueString;
                label_Watch2.Text = "Y: " + _watches[1].ValueString;
                //label_Watch1.Text = string.Format("X: {1}", _watches[0].AddressString, _watches[0].ValueString);
                //label_Watch2.Text = string.Format("Y: {1}", _watches[1].AddressString, _watches[1].ValueString);
                //label_Watch3.Text = string.Format("Third watch ({0}) current value: {1}", _watches[2].AddressString, _watches[2].ValueString);
                //try
                //{
                var objects = SonicObject.ReadObjects(_memoryDomains, includeReserved, log).ToArray();
                label_Objects.Text = "Objects: " + objects.Length;

                if (this.Map != null)
                {
                    this.Map.Drawer.DrawObjects(objects);
                    Point sonicPos = new Point(_watches[0].Value, _watches[1].Value);
                    this.Map.Drawer.DrawCheckPoint(CheckPointInput, sonicPos, objects);
                    this.Map.Drawer.CenterOn(sonicPos.X, sonicPos.Y);

                    for (int i = 0; i < CheckPoints.Length; i++)
                    {
                        this.Map.Drawer.DrawCheckPoint(CheckPoints[i], sonicPos, objects);
                    }
                }

                //listView1.Items.Clear();
                //foreach (SonicObject so in objects)
                //{
                //    listView1.Items.Add(new System.Windows.Forms.ListViewItem(new string[] { so.ObjectType.ToString(), so.NewHitbox_HorizontalRadius.ToString(), so.NewHitbox_VerticalRadius.ToString(), so.ObjectSubType.ToString() }));
                //}
                //}
                //catch { }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SonicMap map = new SonicMap(_memoryDomains);
            this.Map = new MapForm(map);
            this.Map.Show();
            this.Map.Shown += Map_Shown;
        }

        void Map_Shown(object sender, EventArgs e)
        {
            CheckPointInput = new WorldInput(ref Map.Drawer, 0, 0, 16, 16);

            Random rnd = new Random();

            int range = 200;

            CheckPoints = new WorldInput[50];
            for (int i = 0; i < CheckPoints.Length; i++)
            {
                CheckPoints[i] = new WorldInput(ref Map.Drawer, rnd.Next(-range, range), rnd.Next(-range, range), 16, 16);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case (Keys.Control | Keys.Left):
                    CheckPointInput.RelativePosition.X -= 5;
                    break;

                case (Keys.Control | Keys.Right):
                    CheckPointInput.RelativePosition.X += 5;
                    break;

                case (Keys.Control | Keys.Up):
                    CheckPointInput.RelativePosition.Y -= 5;
                    break;

                case (Keys.Control | Keys.Down):
                    CheckPointInput.RelativePosition.Y += 5;
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        private void includeReservedObjects_CheckedChanged(object sender, EventArgs e)
        {
            this.includeReserved = includeReservedCheckBox.Checked;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            _memoryDomains.MainMemory.PokeByte(0xFFFA, checkBox1.Checked ? (byte)1 : (byte)0);
        }
    }
}
