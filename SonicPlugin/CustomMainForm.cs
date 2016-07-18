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
using System.Linq;
using NEAT;
using NEAT.Genetics;

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
        private WorldInput CheckPointInput;
        private bool includeReserved;
        private Stopwatch stopwatch;
        private ControllerForm controller;

        private EvolutionController EvoController;
        private LivingSonic[] Subjects;
        private int SubjectIndex = 0;
        private LivingSonic CurrentSubject;
        private IdleWatcher idleWatcher;

        public const double MaxFitness = 9676;

        //TODO: handle death
        //TODO: save/load current state
        //TODO: anchor worldInput position in genomes

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
        { UpdateValues(); }

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

                SonicObject[] objects = SonicObject.ReadObjects(_memoryDomains, includeReserved).ToArray();
                label_Objects.Text = "Objects: " + objects.Length;

                if ((this.Map != null) && this.Map.Visible)
                {
                    this.Map.Drawer.DrawObjects(objects);
                    Point sonicPos = new Point(_watches[0].Value, _watches[1].Value);
                    this.Map.Drawer.DrawCheckPoint(CheckPointInput, sonicPos, objects);
                    this.Map.Drawer.CenterOn(sonicPos.X, sonicPos.Y);

                    if (CurrentSubject != null)
                    {
                        CurrentSubject.Fitness = sonicPos.X;
                        CurrentSubject.Step(sonicPos, objects);

                        if (idleWatcher.Next(CurrentSubject.Fitness))
                        {
                            CurrentSubject.Fitness = 0;
                            NextSubject();
                            return;
                        }
                        else if (CurrentSubject.Fitness >= MaxFitness)
                        {
                            //TODO: handle successful level
                            NextSubject();
                            return;
                        }
                        else
                        {
                            CurrentSubject.PressButtons();
                        }

                        CurrentSubject.DrawCheckPoints(sonicPos, objects);
                        UpdateGenomeLabels();
                    }

                }

                if ((controller != null) && controller.Visible)
                    controller.ParseButtons(Global.ActiveController.PressedButtons);
            }
        }

        private void NextSubject()
        {
            if (SubjectIndex < Subjects.Length)
            {
                CurrentSubject = Subjects[SubjectIndex++];
            }
            else //Next generation
            {
                EvoController.Population.SortGenomesByFitness();
                currentGenLabel.Text = "Best fitness (?): " + EvoController.Population.GetAll()[0].Fitness;

                EvoController.NextGeneration();
                CreateSubjects();

                currentGenLabel.Text = "Generation: " + EvoController.Generation;
                totalTimeLabel.Text = "Total elapsed time: " + stopwatch.Elapsed.ToString("HH:mm:ss");
            }

            ResetLevel();
        }

        private void mapButton_Click(object sender, EventArgs e)
        {
            if ((this.Map != null) && !this.Map.Visible)
                return;

            this.Map = new MapForm(new SonicMap(_memoryDomains));
            this.Map.Show();
            this.Map.Shown += Map_Shown;
        }

        private void Map_Shown(object sender, EventArgs e)
        {
            if (Map.Drawer != null)
            {
                CheckPointInput = new WorldInput(ref Map.Drawer, 0, -20, LivingSonic.Width, LivingSonic.Height);
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

        public static void ResetLevel()
        {
            string path = PathManager.SaveStatePrefix(Global.Game) + ".QuickSave1.State";
            SavestateManager.LoadStateFile(path, Path.GetFileName(path));
        }

        private void startEvolutionButton_Click(object sender, EventArgs e)
        {
            if (EvoController == null)
            {
                EvoController = new EvolutionController(-5, 5, 1, 1, 0.4, 3);
                EvoController.Population.Parameters.MatingOffspringProportion = 0.2;
                EvoController.Population.Parameters.MutationOffspringProportion = 0.8;
                EvoController.Population.Parameters.InitialConnectionProportion = 1;
                EvoController.Population.Parameters.PossibleMutations.FirstOrDefault(mi => mi.MutationType == Genome.MutationType.Weight).Probability = 0.5;
                EvoController.Population.Parameters.PossibleMutations.FirstOrDefault(mi => mi.MutationType == Genome.MutationType.Node).Probability = 0.03;
                EvoController.Population.Parameters.PossibleMutations.FirstOrDefault(mi => mi.MutationType == Genome.MutationType.Connection).Probability = 0.5;

                EvoController.Start(150, 2, 5, new NEAT.NeuralNetworks.ActivationFunctions.EvenSigmoid(5));

                idleWatcher = new IdleWatcher(5);

                if ((this.Map == null) || !this.Map.Visible)
                {
                    this.Map = new MapForm(new SonicMap(_memoryDomains));
                    this.Map.Show();
                    this.Map.Shown += Map_Shown;
                }

                CreateSubjects();

                NextSubject();

                stopwatch.Reset();
                stopwatch.Start();

                startEvolutionButton.Text = "Stop Evolution";
            }
            else
            {
                CurrentSubject = null;
                stopwatch.Stop();
                startEvolutionButton.Text = "Start Evolution";
            }
        }

        private void CreateSubjects()
        {
            var genomes = EvoController.Population.GetAll();

            Subjects = new LivingSonic[genomes.Length];
            for (int i = 0; i < Subjects.Length; i++)
            {
                genomes[i].Fitness = 0;
                Subjects[i] = new LivingSonic(genomes[i], ref Map.Drawer, 16, 200, 200);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mapButton_Click(null, null);
        }

        private void UpdateGenomeLabels()
        {
            fitnessLabel.Text = "Fitness: " + CurrentSubject.Fitness.ToString("0.0000");
        }
    }
}
