namespace RemotePanel
{
    partial class RemotePanel
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RemotePanel));
            this.destBox = new System.Windows.Forms.TextBox();
            this.connectButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.totalTimeLabel = new System.Windows.Forms.Label();
            this.maxFitnessLabel = new System.Windows.Forms.Label();
            this.currentGenLabel = new System.Windows.Forms.Label();
            this.maxFitnessAlertLabel = new System.Windows.Forms.LinkLabel();
            this.genomeLabel = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // destBox
            // 
            this.destBox.Location = new System.Drawing.Point(12, 12);
            this.destBox.Name = "destBox";
            this.destBox.Size = new System.Drawing.Size(192, 20);
            this.destBox.TabIndex = 1;
            this.destBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.destBox_KeyPress);
            // 
            // connectButton
            // 
            this.connectButton.Image = ((System.Drawing.Image)(resources.GetObject("connectButton.Image")));
            this.connectButton.Location = new System.Drawing.Point(210, 11);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(25, 22);
            this.connectButton.TabIndex = 3;
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.genomeLabel);
            this.groupBox2.Controls.Add(this.maxFitnessAlertLabel);
            this.groupBox2.Controls.Add(this.totalTimeLabel);
            this.groupBox2.Controls.Add(this.maxFitnessLabel);
            this.groupBox2.Controls.Add(this.currentGenLabel);
            this.groupBox2.Location = new System.Drawing.Point(12, 39);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(223, 83);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Status";
            // 
            // totalTimeLabel
            // 
            this.totalTimeLabel.AutoEllipsis = true;
            this.totalTimeLabel.Location = new System.Drawing.Point(6, 46);
            this.totalTimeLabel.Name = "totalTimeLabel";
            this.totalTimeLabel.Size = new System.Drawing.Size(211, 14);
            this.totalTimeLabel.TabIndex = 13;
            this.totalTimeLabel.Text = "-";
            // 
            // maxFitnessLabel
            // 
            this.maxFitnessLabel.AutoEllipsis = true;
            this.maxFitnessLabel.Location = new System.Drawing.Point(6, 31);
            this.maxFitnessLabel.Name = "maxFitnessLabel";
            this.maxFitnessLabel.Size = new System.Drawing.Size(211, 14);
            this.maxFitnessLabel.TabIndex = 12;
            this.maxFitnessLabel.Text = "-";
            // 
            // currentGenLabel
            // 
            this.currentGenLabel.AutoEllipsis = true;
            this.currentGenLabel.Location = new System.Drawing.Point(6, 16);
            this.currentGenLabel.Name = "currentGenLabel";
            this.currentGenLabel.Size = new System.Drawing.Size(211, 14);
            this.currentGenLabel.TabIndex = 11;
            this.currentGenLabel.Text = "-";
            // 
            // maxFitnessAlertLabel
            // 
            this.maxFitnessAlertLabel.Image = global::RemotePanel.Properties.Resources.exclamation_circle;
            this.maxFitnessAlertLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.maxFitnessAlertLabel.LinkColor = System.Drawing.SystemColors.ControlText;
            this.maxFitnessAlertLabel.Location = new System.Drawing.Point(202, 30);
            this.maxFitnessAlertLabel.Name = "maxFitnessAlertLabel";
            this.maxFitnessAlertLabel.Size = new System.Drawing.Size(16, 16);
            this.maxFitnessAlertLabel.TabIndex = 14;
            this.toolTip1.SetToolTip(this.maxFitnessAlertLabel, "Enable alert on fitness increase");
            this.maxFitnessAlertLabel.Click += new System.EventHandler(this.maxFitnessAlertLabel_Click);
            // 
            // genomeLabel
            // 
            this.genomeLabel.AutoEllipsis = true;
            this.genomeLabel.Location = new System.Drawing.Point(6, 62);
            this.genomeLabel.Name = "genomeLabel";
            this.genomeLabel.Size = new System.Drawing.Size(211, 14);
            this.genomeLabel.TabIndex = 15;
            this.genomeLabel.Text = "-";
            // 
            // RemotePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(247, 133);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.destBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "RemotePanel";
            this.Text = "SoNNic remote Panel";
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox destBox;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label totalTimeLabel;
        private System.Windows.Forms.Label maxFitnessLabel;
        private System.Windows.Forms.Label currentGenLabel;
        private System.Windows.Forms.LinkLabel maxFitnessAlertLabel;
        private System.Windows.Forms.Label genomeLabel;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

