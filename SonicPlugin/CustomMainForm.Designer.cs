using SonicPlugin;
namespace BizHawk.Client.EmuHawk
{
    partial class CustomMainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomMainForm));
            this.label_Watch1 = new System.Windows.Forms.Label();
            this.label_Watch2 = new System.Windows.Forms.Label();
            this.label_Objects = new System.Windows.Forms.Label();
            this.includeReservedCheckBox = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.controllerButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.genomeLabel = new System.Windows.Forms.Label();
            this.fitnessLabel = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.totalTimeLabel = new System.Windows.Forms.Label();
            this.maxFitnessLabel = new System.Windows.Forms.Label();
            this.currentGenLabel = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.startEvolutionButton = new System.Windows.Forms.Button();
            this.mapButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.loadButton = new System.Windows.Forms.Button();
            this.saveGenomeDialog = new System.Windows.Forms.SaveFileDialog();
            this.openGenomesDialog = new System.Windows.Forms.OpenFileDialog();
            this.autoSaveBox = new System.Windows.Forms.CheckBox();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label_Watch1
            // 
            this.label_Watch1.AutoEllipsis = true;
            this.label_Watch1.Location = new System.Drawing.Point(6, 16);
            this.label_Watch1.Name = "label_Watch1";
            this.label_Watch1.Size = new System.Drawing.Size(104, 14);
            this.label_Watch1.TabIndex = 0;
            this.label_Watch1.Text = "label1";
            // 
            // label_Watch2
            // 
            this.label_Watch2.AutoEllipsis = true;
            this.label_Watch2.Location = new System.Drawing.Point(6, 34);
            this.label_Watch2.Name = "label_Watch2";
            this.label_Watch2.Size = new System.Drawing.Size(104, 16);
            this.label_Watch2.TabIndex = 1;
            this.label_Watch2.Text = "label1";
            // 
            // label_Objects
            // 
            this.label_Objects.AutoSize = true;
            this.label_Objects.Location = new System.Drawing.Point(6, 51);
            this.label_Objects.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_Objects.Name = "label_Objects";
            this.label_Objects.Size = new System.Drawing.Size(35, 13);
            this.label_Objects.TabIndex = 3;
            this.label_Objects.Text = "label1";
            // 
            // includeReservedCheckBox
            // 
            this.includeReservedCheckBox.AutoSize = true;
            this.includeReservedCheckBox.Location = new System.Drawing.Point(6, 66);
            this.includeReservedCheckBox.Name = "includeReservedCheckBox";
            this.includeReservedCheckBox.Size = new System.Drawing.Size(107, 30);
            this.includeReservedCheckBox.TabIndex = 6;
            this.includeReservedCheckBox.Text = "include reserved \r\nObjects";
            this.includeReservedCheckBox.UseVisualStyleBackColor = true;
            this.includeReservedCheckBox.CheckedChanged += new System.EventHandler(this.includeReservedObjects_CheckedChanged);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(6, 102);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(85, 17);
            this.checkBox1.TabIndex = 7;
            this.checkBox1.Text = "debug mode";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // controllerButton
            // 
            this.controllerButton.Image = ((System.Drawing.Image)(resources.GetObject("controllerButton.Image")));
            this.controllerButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.controllerButton.Location = new System.Drawing.Point(12, 143);
            this.controllerButton.Name = "controllerButton";
            this.controllerButton.Size = new System.Drawing.Size(116, 23);
            this.controllerButton.TabIndex = 8;
            this.controllerButton.Text = "   Show Controller";
            this.controllerButton.UseVisualStyleBackColor = true;
            this.controllerButton.Click += new System.EventHandler(this.controllerButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.genomeLabel);
            this.groupBox1.Controls.Add(this.fitnessLabel);
            this.groupBox1.Location = new System.Drawing.Point(134, 82);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(208, 55);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Current Genome";
            // 
            // genomeLabel
            // 
            this.genomeLabel.AutoEllipsis = true;
            this.genomeLabel.Location = new System.Drawing.Point(6, 16);
            this.genomeLabel.Name = "genomeLabel";
            this.genomeLabel.Size = new System.Drawing.Size(188, 14);
            this.genomeLabel.TabIndex = 13;
            this.genomeLabel.Text = "-";
            // 
            // fitnessLabel
            // 
            this.fitnessLabel.AutoEllipsis = true;
            this.fitnessLabel.Location = new System.Drawing.Point(6, 33);
            this.fitnessLabel.Name = "fitnessLabel";
            this.fitnessLabel.Size = new System.Drawing.Size(188, 14);
            this.fitnessLabel.TabIndex = 12;
            this.fitnessLabel.Text = "-";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.totalTimeLabel);
            this.groupBox2.Controls.Add(this.maxFitnessLabel);
            this.groupBox2.Controls.Add(this.currentGenLabel);
            this.groupBox2.Location = new System.Drawing.Point(134, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(208, 64);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Evolution";
            // 
            // totalTimeLabel
            // 
            this.totalTimeLabel.AutoEllipsis = true;
            this.totalTimeLabel.Location = new System.Drawing.Point(6, 46);
            this.totalTimeLabel.Name = "totalTimeLabel";
            this.totalTimeLabel.Size = new System.Drawing.Size(188, 14);
            this.totalTimeLabel.TabIndex = 13;
            this.totalTimeLabel.Text = "-";
            // 
            // maxFitnessLabel
            // 
            this.maxFitnessLabel.AutoEllipsis = true;
            this.maxFitnessLabel.Location = new System.Drawing.Point(6, 31);
            this.maxFitnessLabel.Name = "maxFitnessLabel";
            this.maxFitnessLabel.Size = new System.Drawing.Size(188, 14);
            this.maxFitnessLabel.TabIndex = 12;
            this.maxFitnessLabel.Text = "-";
            // 
            // currentGenLabel
            // 
            this.currentGenLabel.AutoEllipsis = true;
            this.currentGenLabel.Location = new System.Drawing.Point(6, 16);
            this.currentGenLabel.Name = "currentGenLabel";
            this.currentGenLabel.Size = new System.Drawing.Size(188, 14);
            this.currentGenLabel.TabIndex = 11;
            this.currentGenLabel.Text = "-";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label_Watch1);
            this.groupBox3.Controls.Add(this.label_Watch2);
            this.groupBox3.Controls.Add(this.label_Objects);
            this.groupBox3.Controls.Add(this.includeReservedCheckBox);
            this.groupBox3.Controls.Add(this.checkBox1);
            this.groupBox3.Location = new System.Drawing.Point(12, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(116, 125);
            this.groupBox3.TabIndex = 13;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Debug";
            // 
            // startEvolutionButton
            // 
            this.startEvolutionButton.Location = new System.Drawing.Point(134, 143);
            this.startEvolutionButton.Name = "startEvolutionButton";
            this.startEvolutionButton.Size = new System.Drawing.Size(208, 23);
            this.startEvolutionButton.TabIndex = 14;
            this.startEvolutionButton.Text = "Start Evolution";
            this.startEvolutionButton.UseVisualStyleBackColor = true;
            this.startEvolutionButton.Click += new System.EventHandler(this.startEvolutionButton_Click);
            // 
            // mapButton
            // 
            this.mapButton.Image = ((System.Drawing.Image)(resources.GetObject("mapButton.Image")));
            this.mapButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mapButton.Location = new System.Drawing.Point(12, 172);
            this.mapButton.Name = "mapButton";
            this.mapButton.Size = new System.Drawing.Size(116, 23);
            this.mapButton.TabIndex = 15;
            this.mapButton.Text = "Map";
            this.mapButton.UseVisualStyleBackColor = true;
            this.mapButton.Click += new System.EventHandler(this.mapButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Image = ((System.Drawing.Image)(resources.GetObject("saveButton.Image")));
            this.saveButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.saveButton.Location = new System.Drawing.Point(165, 172);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(85, 23);
            this.saveButton.TabIndex = 16;
            this.saveButton.Text = "   Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // loadButton
            // 
            this.loadButton.Image = ((System.Drawing.Image)(resources.GetObject("loadButton.Image")));
            this.loadButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.loadButton.Location = new System.Drawing.Point(256, 172);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(86, 23);
            this.loadButton.TabIndex = 17;
            this.loadButton.Text = "  Load";
            this.loadButton.UseVisualStyleBackColor = true;
            this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
            // 
            // saveGenomeDialog
            // 
            this.saveGenomeDialog.DefaultExt = "evo";
            this.saveGenomeDialog.Filter = "Save Files (*.evo)|*.evo";
            this.saveGenomeDialog.Title = "Save current population";
            // 
            // openGenomesDialog
            // 
            this.openGenomesDialog.DefaultExt = "evo";
            this.openGenomesDialog.Filter = "Save Files (*.evo)|*.evo";
            this.openGenomesDialog.Title = "Load saved population";
            // 
            // autoSaveBox
            // 
            this.autoSaveBox.Appearance = System.Windows.Forms.Appearance.Button;
            this.autoSaveBox.AutoCheck = false;
            this.autoSaveBox.AutoEllipsis = true;
            this.autoSaveBox.Image = ((System.Drawing.Image)(resources.GetObject("autoSaveBox.Image")));
            this.autoSaveBox.Location = new System.Drawing.Point(134, 172);
            this.autoSaveBox.Name = "autoSaveBox";
            this.autoSaveBox.Size = new System.Drawing.Size(25, 23);
            this.autoSaveBox.TabIndex = 19;
            this.autoSaveBox.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.autoSaveBox, "Auto-save each generation");
            this.autoSaveBox.UseVisualStyleBackColor = true;
            this.autoSaveBox.Click += new System.EventHandler(this.autoSaveBox_Click);
            // 
            // folderBrowserDialog
            // 
            this.folderBrowserDialog.Description = "Select a folder to save each generation to:";
            // 
            // CustomMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(354, 204);
            this.Controls.Add(this.autoSaveBox);
            this.Controls.Add(this.loadButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.mapButton);
            this.Controls.Add(this.startEvolutionButton);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.controllerButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CustomMainForm";
            this.Text = "SoNNic";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label_Watch1;
        private System.Windows.Forms.Label label_Watch2;
        private System.Windows.Forms.Label label_Objects;
        private System.Windows.Forms.CheckBox includeReservedCheckBox;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button controllerButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label currentGenLabel;
        private System.Windows.Forms.Label maxFitnessLabel;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button startEvolutionButton;
        private System.Windows.Forms.Label totalTimeLabel;
        private System.Windows.Forms.Label fitnessLabel;
        private System.Windows.Forms.Label genomeLabel;
        private System.Windows.Forms.Button mapButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button loadButton;
        private System.Windows.Forms.SaveFileDialog saveGenomeDialog;
        private System.Windows.Forms.OpenFileDialog openGenomesDialog;
        private System.Windows.Forms.CheckBox autoSaveBox;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}