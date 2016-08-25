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
        private ControllerForm controller;

        private EvolutionController EvoController;
        private LivingSonic[] Subjects;
        private int SubjectIndex = 0;
        private LivingSonic CurrentSubject;
        private IdleWatcher idleWatcher;

        public const double MaxFitness = 9676;
        private DateTime StartTime;
        public TimeSpan TimePassed { get { return DateTime.Now - StartTime; } }
        private bool Running;
        private string AutoSavePath = null;

        //TODO: save/load current state

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

                        if (idleWatcher.Next(CurrentSubject.Fitness) || CurrentSubject.Fitness < 70)
                        {
                            //CurrentSubject.Fitness = 0;
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
            ResetLevel();
            if (SubjectIndex < Subjects.Length)
            {
                CurrentSubject = Subjects[SubjectIndex++];
            }
            else //Next generation
            {
                EvoController.Population.SortGenomesByFitness();
                double bestFitness = EvoController.Population.GetAll().OrderByDescending(g => g.Fitness).First().Fitness;
                maxFitnessLabel.Text = "Best fitness: " + bestFitness.ToString("0") +  " (" + ((bestFitness / MaxFitness) * 100D).ToString("0.00") + "%)";

                EvoController.NextGeneration();
                CreateSubjects();
                SubjectIndex = 0;
            }

            currentGenLabel.Text = "Generation: " + (EvoController.Generation + 1);
            genomeLabel.Text = "Genome " + SubjectIndex + "/" + Subjects.Length;
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
                CreateEvolutionController();

            if (!this.Running)
            {
                if (EvoController.Population.Count < 2)
                    EvoController.Start(150, 2, 5, new NEAT.NeuralNetworks.ActivationFunctions.EvenSigmoid(5));

                this.StartEvolution();
            }
            else
            {
                this.StopEvolution();
            }
        }

        private void CreateSubjects()
        {
            var genomes = EvoController.Population.GetAll();

            if (AutoSavePath != null)
            {
                try
                {
                    SaveGenomes(genomes, EvoController.Generation, Path.Combine(AutoSavePath, "gen" + EvoController.Generation + ".evo"));
                }
                catch { }
            }

            Subjects = new LivingSonic[genomes.Length];
            for (int i = 0; i < Subjects.Length; i++)
            {
                genomes[i].Fitness = 0;
                Subjects[i] = new LivingSonic(genomes[i], ref Map.Drawer); //, 16, 200, 200
            }
        }

        private void UpdateGenomeLabels()
        {
            fitnessLabel.Text = "Fitness: " + CurrentSubject.Fitness.ToString("0");
            totalTimeLabel.Text = "Time passed: " + TimePassed.ToReadableString();
        }

        public void SaveGenomes(Genome[] genomes, uint generation, string filename)
        {
            using (StreamWriter writer = File.CreateText(filename))
            {
                writer.WriteLine(generation);
                writer.WriteLine(InnovationGenerator.Current);

                if (StartTime != DateTime.MinValue)
                    writer.WriteLine(TimePassed.TotalSeconds);
                else
                    writer.WriteLine(0);

                for (int i = 0; i < genomes.Length; i++)
                    writer.WriteLine(genomes[i].ToString());
            }
        }

        public Genome[] LoadGenomes(string filename, ref uint generation, ref double secondsPassed)
        {
            string[] lines = File.ReadAllLines(filename);
            Genome[] genomes = new Genome[lines.Length - 3];

            generation = uint.Parse(lines[0]);
            InnovationGenerator.Current = ulong.Parse(lines[1]);
            secondsPassed = double.Parse(lines[2]);

            for (int i = 0; i < genomes.Length; i++)
                genomes[i] = Genome.FromString(lines[i + 3]);

            return genomes;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (EvoController != null && EvoController.Population != null)
            {
                if (saveGenomeDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        SaveGenomes(EvoController.Population.GetAll(), EvoController.Generation, saveGenomeDialog.FileName);
                        MessageBox.Show("Successfully saved current population!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show((EvoController == null).ToString());
                        MessageBox.Show((EvoController.Population == null).ToString());
                        MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //MessageBox.Show("Could not save current population!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            if (openGenomesDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (EvoController == null)
                        CreateEvolutionController();

                    if (this.Running)
                        this.StopEvolution();

                    double seconds = 0;
                    EvoController.Start(LoadGenomes(openGenomesDialog.FileName, ref EvoController.Generation, ref seconds), false);

                    if (seconds != 0)
                        this.StartTime = DateTime.Now - TimeSpan.FromSeconds(seconds);
                    else
                        this.StartTime = DateTime.MinValue;

                    SubjectIndex = 0;
                    MessageBox.Show("Successfully loaded current population!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //MessageBox.Show("Could not load current population!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void StopEvolution()
        {
            CurrentSubject = null;
            SubjectIndex = 0;
            startEvolutionButton.Text = "Start Evolution";
            genomeLabel.Text = "-";
            fitnessLabel.Text = "-";
            this.Running = false;
        }

        private void StartEvolution()
        {
            idleWatcher = new IdleWatcher(60); //5
            if ((this.Map == null) || !this.Map.Visible)
            {
                this.Map = new MapForm(new SonicMap(_memoryDomains));
                this.Map.Show();
                this.Map.Shown += Map_Shown;
            }

            CreateSubjects();
            NextSubject();

            if (this.StartTime == DateTime.MinValue)
                this.StartTime = DateTime.Now;

            startEvolutionButton.Text = "Stop Evolution";
            this.Running = true;
        }

        public void CreateEvolutionController()
        {
            EvoController = new EvolutionController(-5, 5, 1, 1, 0.4, 3);
            EvoController.Population.Parameters.MatingOffspringProportion = 0.75; //0.2;
            EvoController.Population.Parameters.MutationOffspringProportion = 0.25; //0.8;
            EvoController.Population.Parameters.InitialConnectionProportion = 1;
            EvoController.Population.Parameters.PossibleMutations.FirstOrDefault(mi => mi.MutationType == Genome.MutationType.Weight).Probability = 0.90; //0.5;
            EvoController.Population.Parameters.PossibleMutations.FirstOrDefault(mi => mi.MutationType == Genome.MutationType.Node).Probability = 0.50; //0.03;
            EvoController.Population.Parameters.PossibleMutations.FirstOrDefault(mi => mi.MutationType == Genome.MutationType.Connection).Probability = 1.0; //0.5;
            EvoController.Population.Parameters.PossibleMutations.FirstOrDefault(mi => mi.MutationType == Genome.MutationType.AddInput).Probability = 1.0;
            EvoController.Population.Parameters.SensorSize = 20;
        }

        private void autoSaveBox_Click(object sender, EventArgs e)
        {
            if (!autoSaveBox.Checked)
            {
                folderBrowserDialog.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    AutoSavePath = folderBrowserDialog.SelectedPath;
                    autoSaveBox.Checked = true;
                }
            }
            else
            {
                AutoSavePath = null;
                autoSaveBox.Checked = false;
            }
        }
    }
}
