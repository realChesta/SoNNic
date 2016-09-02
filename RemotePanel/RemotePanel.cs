using RemotePanel.Properties;
using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace RemotePanel
{
    public partial class RemotePanel : Form
    {
        private TcpClient Client;
        private bool BestFitnessAlert;
        private double BestFitness = -1;

        public RemotePanel()
        {
            InitializeComponent();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            try
            {
                var dest = GetDestination(destBox.Text);
                this.Connect(dest.Item1, dest.Item2);
            }
            catch { }
        }

        private void destBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                try
                {
                    var dest = GetDestination(destBox.Text);
                    this.Connect(dest.Item1, dest.Item2);
                }
                catch { }
            }
        }

        private Tuple<string, int> GetDestination(string toParse)
        {
            string[] splitted = toParse.Split(':');

            if (splitted.Length != 2)
                throw new FormatException("Invalid destination format!");

            return new Tuple<string, int>(splitted[0], int.Parse(splitted[1]));
        }

        private void Connect(string host, int port)
        {
            new Thread(delegate()
            {
                try
                {
                    this.InvokeEx(f =>
                    {
                        f.destBox.Enabled = false;
                        f.connectButton.Enabled = false;
                    });

                    this.Client = new TcpClient(host, port);
                    
                    HandleCommunication(Client);
                }
                catch { }

                this.InvokeEx(f =>
                {
                    f.destBox.Enabled = true;
                    f.connectButton.Enabled = true;
                    f.currentGenLabel.Text = "-";
                    f.maxFitnessLabel.Text = "-";
                    f.totalTimeLabel.Text = "-";
                    f.genomeLabel.Text = "-";
                    f.Text = "SoNNic remote Panel";
                });
            })
            { IsBackground = true, Name = "ConnectionThread" }.Start();
        }

        private void HandleCommunication(TcpClient client)
        {
            try
            {
                using (client)
                using (BinaryReader reader = new BinaryReader(client.GetStream()))
                {
                    while (Client.Connected)
                    {
                        uint gen = reader.ReadUInt32();
                        double bestFitness = reader.ReadDouble();
                        double maxFitness = reader.ReadDouble();
                        TimeSpan timePassed = TimeSpan.FromSeconds(reader.ReadDouble());
                        int subject = reader.ReadInt32();
                        int population = reader.ReadInt32();

                        this.InvokeEx(f =>
                        {
                            f.currentGenLabel.Text = "Generation " + (gen + 1).ToString();
                            f.maxFitnessLabel.Text = "Best fitness: " + bestFitness.ToString("0") + " (" + ((bestFitness / maxFitness) * 100D).ToString("0.00") + "%)";
                            f.totalTimeLabel.Text = "Time passed: " + timePassed.ToReadableString();
                            f.genomeLabel.Text = "Genome " + subject + "/" + population;
                            f.Text = "SoNNic // G" + (gen + 1).ToString() + ":" + subject;
                        });

                        if (BestFitnessAlert && (bestFitness > BestFitness))
                        {
                            BestFitness = bestFitness;
                            FlashWindow(this.Handle, FlashMode.UntilForeground);
                        }
                    }
                }
            }
            catch { }

            try
            { client.Close(); }
            catch { }
        }

        private void maxFitnessAlertLabel_Click(object sender, EventArgs e)
        {
            BestFitnessAlert = !BestFitnessAlert;
            maxFitnessAlertLabel.Image = BestFitnessAlert ? Resources.exclamation_red : Resources.exclamation_circle;
            toolTip1.SetToolTip(maxFitnessAlertLabel, (BestFitnessAlert ? "Disable" : "Enable") + " alert on fitness increase");
        }

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
    }
}
