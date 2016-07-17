using BizHawk.Client.Common;
using BizHawk.Emulation.Common;
using SonicPlugin;
using SonicPlugin.Sonic;
using System;
using System.Drawing;
using System.Windows.Forms;
using SonicPlugin.Sonic.NN;
using System.Diagnostics;
using System.IO;

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
        //private LogForm log;

        private bool includeReserved;

        private WorldInput CheckPointInput;
        private WorldInput[] CheckPoints;

        private Stopwatch stopwatch;

        private ControllerForm controller;

        public CustomMainForm()
        {
            InitializeComponent();
            //log = new LogForm("Log: objects");
            //log.Show();
        }

        public bool AskSaveChanges()
        {
            return true;
        }

        /// <summary>
        /// This method is called instead of regular <see cref="UpdateValues"/>
        /// when emulator is runnig in turbo mode
        /// </summary>
        public void FastUpdate()
        { }

        /// <summary>
        /// Restart is called the first time you call the form
        /// but also when you start playing a movie
        /// </summary>
        public void Restart()
        {
            if (Global.Game.Name.Contains("Sonic The Hedgehog"))
            {
                if (_watches == null)
                {
                    _watches = new WatchList(_memoryDomains, _emu.SystemId ?? string.Empty);
                    Watch myFirstWatch = Watch.GenerateWatch(_memoryDomains.MainMemory, 0xD008, WatchSize.Word, BizHawk.Client.Common.DisplayType.Unsigned, true);
                    Watch mySecondWatch = Watch.GenerateWatch(_memoryDomains.MainMemory, 0xD00C, WatchSize.Word, BizHawk.Client.Common.DisplayType.Unsigned, true);
                    _watches.Add(myFirstWatch);
                    _watches.Add(mySecondWatch);
                }
                else
                {
                    _watches.RefreshDomains(_memoryDomains);
                }
            }

            #region old
            /*
            if (Global.Game.Name != "Null")
            {
                MessageBox.Show(Global.Game.Name);
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
            */
            #endregion
        }

        /// <summary>
        /// Return true if you want the <see cref="UpdateValues"/> method
        /// to be called before rendering
        /// </summary>
        public bool UpdateBefore => false;

        /// <summary>
        /// This method is called when a frame is rendered
        /// You can comapre it the lua equivalent emu.frameadvance()
        /// </summary>
        public void UpdateValues()
        {
            if (Global.Game.Name.Contains("Sonic The Hedgehog"))
            {
                //we update our watches
                _watches.UpdateValues();
                label_Watch1.Text = "X: " + _watches[0].ValueString;
                label_Watch2.Text = "Y: " + _watches[1].ValueString;

                var objects = SonicObject.ReadObjects(_memoryDomains, includeReserved).ToArray();
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

                if ((controller != null) && controller.Visible)
                    controller.ParseButtons(Global.ActiveController.PressedButtons);
            }
        }

        private void mapButton_Click(object sender, EventArgs e)
        {
            SonicMap map = new SonicMap(_memoryDomains);
            this.Map = new MapForm(map);
            this.Map.Show();
            this.Map.Shown += Map_Shown;
        }

        void Map_Shown(object sender, EventArgs e)
        {
            if (Map.Drawer != null)
            {
                CheckPointInput = new WorldInput(ref Map.Drawer, 0, -20, LivingSonic.Width, LivingSonic.Height);

                Random rnd = new Random();

                int range = 200;

                CheckPoints = new WorldInput[50];
                for (int i = 0; i < CheckPoints.Length; i++)
                {
                    CheckPoints[i] = new WorldInput(ref Map.Drawer, rnd.Next(-range, range), rnd.Next(-range, range), 16, 16);
                }
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

        private void includeReservedObjects_CheckedChanged(object sender, EventArgs e)
        {
            this.includeReserved = includeReservedCheckBox.Checked;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            _memoryDomains.MainMemory.PokeByte(0xFFFA, checkBox1.Checked ? (byte)1 : (byte)0);
        }

        private void controllerButton_Click(object sender, EventArgs e)
        {
            if ((controller != null) && !controller.Visible)
                return;

            controller = new ControllerForm();
            controller.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ResetLevel();
        }

        public static void ResetLevel()
        {
            string path = PathManager.SaveStatePrefix(Global.Game) + ".QuickSave1.State";
            SavestateManager.LoadStateFile(path, Path.GetFileName(path));
        }

        private void startEvolutionButton_Click(object sender, EventArgs e)
        {

        }
    }
}
