using BizHawk.Bizware.BizwareGL;
using BizHawk.Client.Common;
using BizHawk.Emulation.Common;
using NEAT;
using NEAT.Genetics;
using SonicPlugin;
using SonicPlugin.Sonic;
using SonicPlugin.Sonic.NN;
using SoNNic;
using SoNNic.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace BizHawk.Client.EmuHawk
{
    public partial class CustomMainForm : Form, IExternalToolForm
    {
        #region Declares

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
        private DateTime SubjectStartTime;

        public const double MaxFitness = 9676;
        private double BestFitness = 0;
        private DateTime StartTime;
        private DateTime StopTime;
        public TimeSpan TimePassed { get { return DateTime.Now - StartTime; } }
        private bool Running;
        private string AutoSavePath = null;

        private readonly string SettingsPath = AppDomain.CurrentDomain.BaseDirectory + "settings.cfg";
        private int Port = 2729;

        private bool BestFitnessAlert;

        private bool Listen;
        private TcpListener Listener;
        private List<SonicTcpClient> Clients = new List<SonicTcpClient>();

        private TelegramBotClient Bot;
        private List<long> ChatIDs = new List<long>();

        #endregion

        //TODO: save/load current state: possibly record fitness, time, more?

        #region WinForms/Initialization

        public CustomMainForm()
        {
            InitializeComponent();

            this.LoadSettings();
            this.portUpDown.Value = this.Port;
            this.CreateTelegramBot();
        }

        //private void includeReservedObjects_CheckedChanged(object sender, EventArgs e)
        //{
        //    this.includeReserved = includeReservedCheckBox.Checked;
        //}

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            _memoryDomains.MainMemory.PokeByte(0xFFFA, checkBox1.Checked ? (byte)1 : (byte)0);
        }

        private void UpdateGenomeLabels()
        {
            fitnessLabel.Text = "Fitness: " + CurrentSubject.Fitness.ToString("0") + " (" + ((CurrentSubject.Fitness / MaxFitness) * 100D).ToString("0.00") + "%)";
            totalTimeLabel.Text = "Time passed: " + TimePassed.ToReadableString();

            if (idleWatcher != null)
            {
                idleLabel.Text = "IdleWatcher: " + idleWatcher.IdleFrames + "/" + idleWatcher.Limit;
                idleBar.Value = idleWatcher.IdleFrames;
            }
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
                case (Keys.Control | Keys.Right):
                    NextSubject();
                    break;

                case (Keys.Control | Keys.Left):
                    PreviousSubject();
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void controllerButton_Click(object sender, EventArgs e)
        {
            if ((controller != null) && !controller.Visible)
                return;

            controller = new ControllerForm();
            controller.Show();
        }

        private void maxFitnessAlertLabel_LinkClicked(object sender, EventArgs e)
        {
            BestFitnessAlert = !BestFitnessAlert;
            maxFitnessAlertLabel.Image = BestFitnessAlert ? Resources.exclamation_red : Resources.exclamation_circle;
            toolTip1.SetToolTip(maxFitnessAlertLabel, (BestFitnessAlert ? "Disable" : "Enable") + " alert on fitness increase");
        }

        private void networkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (networkBox.Checked)
            {
                StartListener();
            }
            else
            {
                StopListener();
            }
        }

        #endregion

        #region BizHawk

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

        public Image Screenshot()
        {
            using (var bb = GlobalWin.DisplayManager.RenderOffscreen(Global.Emulator.VideoProvider(), false))
            {
                bb.DiscardAlpha();
                return bb.ToSysdrawingBitmap();
            }
        }

        #endregion

        #region Evolution

        private void startEvolutionButton_Click(object sender, EventArgs e)
        {
            if (EvoController == null)
                CreateEvolutionController();

            if (!this.Running)
            {
                if (EvoController.Population.Count < 2)
                    EvoController.Start(300, 2, 5, new NEAT.NeuralNetworks.ActivationFunctions.EvenSigmoid(5));

                this.StartEvolution();
            }
            else
            {
                this.StopEvolution();
            }
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
            {
                this.StartTime = DateTime.Now;
            }
            else if (this.StopTime != DateTime.MinValue)
            {
                //account for time not running
                TimeSpan timeSinceStop = DateTime.Now - this.StopTime;
                this.StartTime += timeSinceStop;
            }

            idleBar.Maximum = idleWatcher.Limit;
            startEvolutionButton.Text = "Stop Evolution";
            this.Running = true;
        }

        private void StopEvolution()
        {
            CurrentSubject = null;
            SubjectIndex = 0;
            StopTime = DateTime.Now;
            startEvolutionButton.Text = "Start Evolution";
            genomeLabel.Text = "-";
            fitnessLabel.Text = "-";
            this.Text = "SoNNic";
            this.Running = false;
        }

        public void CreateEvolutionController()
        {
            EvoController = new EvolutionController(-5, 5, 1, 1, 0.4, 1);
            EvoController.Population.Parameters.MatingOffspringProportion = 0.75; //0.2;
            EvoController.Population.Parameters.MutationOffspringProportion = 0.25; //0.8;
            EvoController.Population.Parameters.InitialConnectionProportion = 1;
            EvoController.Population.Parameters.PossibleMutations.FirstOrDefault(mi => mi.MutationType == Genome.MutationType.Weight).Probability = 0.90; //0.5;
            EvoController.Population.Parameters.PossibleMutations.FirstOrDefault(mi => mi.MutationType == Genome.MutationType.Node).Probability = 0.50; //0.03;
            EvoController.Population.Parameters.PossibleMutations.FirstOrDefault(mi => mi.MutationType == Genome.MutationType.Connection).Probability = 1.0; //0.5;
            EvoController.Population.Parameters.PossibleMutations.FirstOrDefault(mi => mi.MutationType == Genome.MutationType.AddInput).Probability = 1.0;
            EvoController.Population.Parameters.SensorSize = 32;
        }

        private void CreateSubjects()
        {
            var genomes = EvoController.Population.GetAll();

            Subjects = new LivingSonic[genomes.Length];
            for (int i = 0; i < Subjects.Length; i++)
            {
                genomes[i].Fitness = 0;
                Subjects[i] = new LivingSonic(genomes[i], ref Map.Drawer); //, 16, 200, 200
            }

            if (AutoSavePath != null)
            {
                try
                {
                    SaveGenomes(Subjects, EvoController.Generation, Path.Combine(AutoSavePath, "gen" + (EvoController.Generation + 1).ToString() + ".evo"));
                }
                catch { }
            }
        }

        private void NextSubject()
        {
            if (CurrentSubject != null)
            {
                CurrentSubject.TimePassed = DateTime.Now - SubjectStartTime;
                if (BestFitness < CurrentSubject.Fitness)
                {
                    BestFitness = CurrentSubject.Fitness;
                    OnNewBestFitness();
                }
            }

            ResetLevel();

            if (SubjectIndex < Subjects.Length)
            {
                CurrentSubject = Subjects[SubjectIndex++];
                SubjectStartTime = DateTime.Now;
            }
            else //Next generation
            {
                EvoController.Population.SortGenomesByFitness();
                double bestFitness = EvoController.Population.GetAll().OrderByDescending(g => g.Fitness).First().Fitness;

                BestFitness = bestFitness;

                if (AutoSavePath != null)
                {
                    try
                    {
                        SaveGenomes(Subjects, EvoController.Generation, Path.Combine(AutoSavePath, "gen" + (EvoController.Generation + 1).ToString() + ".evo"));
                    }
                    catch { }
                }

                EvoController.NextGeneration();
                CreateSubjects();
                SubjectIndex = 0;
                CurrentSubject = Subjects[SubjectIndex++];
            }

            maxFitnessLabel.Text = "Best fitness: " + BestFitness.ToString("0") + " (" + ((BestFitness / MaxFitness) * 100D).ToString("0.00") + "%)";
            currentGenLabel.Text = "Generation: " + (EvoController.Generation + 1);
            this.Text = "SoNNic // G" + (EvoController.Generation + 1) + ":" + SubjectIndex;
            genomeLabel.Text = "Genome " + SubjectIndex + "/" + Subjects.Length;

            BroadcastState();
        }

        public static void ResetLevel()
        {
            string path = PathManager.SaveStatePrefix(Global.Game) + ".QuickSave1.State";
            SavestateManager.LoadStateFile(path, Path.GetFileName(path));
        }

        private void PreviousSubject()
        {
            if (SubjectIndex > 0)
            {
                ResetLevel();
                CurrentSubject = Subjects[--SubjectIndex];
            }
        }

        private void OnNewBestFitness()
        {
            try
            {
                if (BestFitnessAlert)
                    FlashWindow(this.Handle, FlashMode.UntilForeground);

                if (this.Bot.IsReceiving)
                    BroadcastFitness();

                foreach (SonicTcpClient client in Clients)
                    SendState(client);
            }
            catch { }
        }

        #endregion

        #region Save/Load

        public void SaveGenomes(LivingSonic[] subjects, uint generation, string filename)
        {
            using (StreamWriter writer = File.CreateText(filename))
            {
                writer.WriteLine(generation);
                writer.WriteLine(InnovationGenerator.Current);

                if (StartTime != DateTime.MinValue)
                    writer.WriteLine(TimePassed.TotalSeconds);
                else
                    writer.WriteLine(0);

                for (int i = 0; i < subjects.Length; i++)
                {
                    writer.WriteLine(subjects[i].Fitness.ToString() + "/" + subjects[i].TimePassed.TotalSeconds.ToString() + "/" + subjects[i].Genome.ToString());
                }
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
            {
                string[] values = lines[i + 3].Split('/');
                genomes[i] = Genome.FromString(values[2]);
            }

            return genomes;
        }

        private void SaveSettings()
        {
            try
            {
                using (StreamWriter writer = File.CreateText(SettingsPath))
                {
                    writer.WriteLine(this.Port);

                    foreach (long id in ChatIDs)
                        writer.WriteLine(id);
                }
            }
            catch { }
        }

        private void LoadSettings()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    string[] lines = File.ReadAllLines(SettingsPath);

                    this.Port = int.Parse(lines[0]);

                    for (int i = 1; i < lines.Length; i++)
                        ChatIDs.Add(long.Parse(lines[i]));
                }
            }
            catch { }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (EvoController != null && EvoController.Population != null)
            {
                if (saveGenomeDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        SaveGenomes(Subjects, EvoController.Generation, saveGenomeDialog.FileName);
                        MessageBox.Show("Successfully saved current population!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch
                    {
                        MessageBox.Show("Could not save current population!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                catch
                {
                    MessageBox.Show("Could not load current population!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
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

        private void portUpDown_ValueChanged(object sender, EventArgs e)
        {
            this.Port = (int)portUpDown.Value;
            SaveSettings();
        }

        #endregion

        #region WinAPI

        public static bool FlashWindow(IntPtr hwnd, FlashMode mode)
        {
            FLASHWINFO fInfo = new FLASHWINFO();
            fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
            fInfo.hwnd = hwnd;
            fInfo.dwFlags = FLASHW_ALL | FLASHW_TIMERNOFG;
            fInfo.uCount = UInt32.MaxValue;
            fInfo.dwTimeout = 0;

            return FlashWindowEx(ref fInfo);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct FLASHWINFO
        {
            public UInt32 cbSize;
            public IntPtr hwnd;
            public UInt32 dwFlags;
            public UInt32 uCount;
            public UInt32 dwTimeout;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

        private const uint FLASHW_ALL = 3;
        private const uint FLASHW_TIMERNOFG = 12;

        public enum FlashMode
        {
            UntilForeground,
            UntilClosed
        }

        #endregion

        #region TCP

        private void StartListener()
        {
            StopListener();
            try
            {
                if (Listener == null)
                    Listener = new TcpListener(IPAddress.Any, 2729);

                Listener.Start();
                Listen = true;

                new Thread(delegate ()
                {
                    try
                    {
                        while (Listen)
                        {
                            if (!Listener.Pending())
                            {
                                Thread.Sleep(500);
                                continue;
                            }

                            SonicTcpClient client = new SonicTcpClient(Listener.AcceptTcpClient());
                            new Thread(delegate () { HandleClient(client); })
                            { IsBackground = true, Name = "Handler of " + ((IPEndPoint)client.Client.Client.RemoteEndPoint).ToString() }.Start();
                        }
                    }
                    catch { }
                })
                { IsBackground = true, Name = "ListenThread" }.Start();
            }
            catch { }
        }

        private void StopListener()
        {
            try
            {
                Listen = false;

                if (Listener != null)
                    Listener.Stop();
            }
            catch { }
        }

        private void HandleClient(SonicTcpClient client)
        {
            try
            {
                Clients.Add(client);
                SendState(client);
                client.Client.GetStream().ReadByte();
            }
            catch { }

            if (Clients.Contains(client))
            {
                try { client.Close(); } catch { }
                Clients.Remove(client);
            }
        }

        private void SendState(SonicTcpClient client)
        {
            client.Writer.Write(EvoController.Generation); //uint
            client.Writer.Write(BestFitness); //double
            client.Writer.Write(MaxFitness); //double
            client.Writer.Write(TimePassed.TotalSeconds); //double
            client.Writer.Write(SubjectIndex); //int
            client.Writer.Write(Subjects.Length); //int
            client.Writer.Flush();
        }

        private void BroadcastState()
        {
            for (int i = 0; i < Clients.Count; i++)
            {
                try
                {
                    SendState(Clients[i]);
                }
                catch
                {
                    try { Clients[i].Close(); } catch { }
                    Clients.RemoveAt(i--);
                }
            }
        }

        #endregion

        #region Telegram

        private void CreateTelegramBot()
        {
            this.Bot = new TelegramBotClient("255128125:AAES2Vg3tUdqJq0fWvjfXOHT-Q8i5Yccquo");
            this.Bot.OnMessage += Bot_OnMessage;
        }

        private void telegramBox_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (telegramBox.Checked)
                {
                    this.Bot.StartReceiving();
                }
                else
                {
                    this.Bot.StopReceiving();
                }
            }
            catch
            {
                MessageBox.Show("Error toggling Telegram functionality!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BroadcastFitness()
        {
            string msg = "New fitness record achieved!\nCurrent record is now " + BestFitness.ToString("0") + " (" + ((BestFitness / MaxFitness) * 100D).ToString("0.00") + "%)\n" +
                         "(Generation " + (EvoController.Generation + 1).ToString() + ", Genome " + SubjectIndex + "/" + Subjects.Length + ")";
            Telegram.Bot.Types.FileToSend fts = new Telegram.Bot.Types.FileToSend("screenshot.png", Screenshot().ToStream(ImageFormat.Png));

            for (int i = 0; i < ChatIDs.Count; i++)
            {
                try
                {
                    await Bot.SendPhotoAsync(ChatIDs[i], fts, msg);
                }
                catch (Telegram.Bot.Exceptions.ApiRequestException ex)
                {
                    if (ex.Message.Contains("blocked"))
                    {
                        ChatIDs.RemoveAt(i--);
                        SaveSettings();
                    }
                }
                catch { }
            }
        }

        private async void Bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var message = e.Message;

            if (message == null || message.Type != MessageType.TextMessage)
                return;
            else if (message.Text.StartsWith("/") && message.Text.Length < 2)
                return;

            string[] cmd = message.Text.Substring(1).Split(' ');

            switch (cmd[0])
            {
                case "status":
                    {
                        string status =
                            "Generation " + (EvoController.Generation + 1).ToString() + "\n" +
                            "Best fitness: " + "Best fitness: " + BestFitness.ToString("0") + " (" + ((BestFitness / MaxFitness) * 100D).ToString("0.00") + "%)\n" +
                            "Time passed: " + TimePassed.ToReadableString() + "\n" +
                            "Current genome: " + SubjectIndex + "/" + Subjects.Length;

                        await Bot.SendTextMessageAsync(message.Chat.Id, status);
                    }
                    break;

                case "sub":
                case "subscribe":
                    {
                        if (!ChatIDs.Contains(message.Chat.Id))
                        {
                            await Bot.SendTextMessageAsync(message.Chat.Id, "You are now subscribed to fitness record updates.");

                            ChatIDs.Add(message.Chat.Id);
                            SaveSettings();
                        }
                        else
                        {
                            await Bot.SendTextMessageAsync(message.Chat.Id, "You are already subscribed!");
                        }
                    }
                    break;

                case "unsub":
                case "unsubscribe":
                    {
                        if (ChatIDs.Contains(message.Chat.Id))
                        {
                            ChatIDs.Remove(message.Chat.Id);
                            SaveSettings();

                            await Bot.SendTextMessageAsync(message.Chat.Id, "You are now unsubscribed.");
                        }
                        else
                        {
                            await Bot.SendTextMessageAsync(message.Chat.Id, "You are not subscribed. (You thus cannot unsubscribe)");
                        }
                    }
                    break;

                case "start":
                    {
                        await Bot.SendTextMessageAsync(message.Chat.Id, "Hey there! I'm a bot created to tell you the current state of the SoNNic instance running.\n\nHere are all commands I can help you with:\n");

                        string help =
                            "/status - Displays current evolution status.\n" +
                            "/subscribe - Subscribe to receive a message when a new fitness record has been achieved.\n" +
                            "/sub - Shorthand for /subscribe.\n" +
                            "/unsubscribe - Unsubscribe from fitness record messages.\n" +
                            "/unsub - Shorthand for /unsubscribe.\n" +
                            "/help - Displays help.";

                        await Bot.SendTextMessageAsync(message.Chat.Id, help);
                    }
                    break;

                case "help":
                    {
                        string help =
                            "Here are all my supported commands:\n\n" + 
                            "/status - Displays current evolution status.\n" +
                            "/subscribe - Subscribe to receive a message when a new fitness record has been achieved.\n" +
                            "/sub - Shorthand for /subscribe.\n" +
                            "/unsubscribe - Unsubscribe from fitness record messages.\n" +
                            "/unsub - Shorthand for /unsubscribe.\n" +
                            "/help - Displays help.";

                        await Bot.SendTextMessageAsync(message.Chat.Id, help);
                    }
                    break;
            }
        }

        #endregion
    }
}