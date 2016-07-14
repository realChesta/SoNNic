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
            this.label_Watch1 = new System.Windows.Forms.Label();
            this.label_Watch2 = new System.Windows.Forms.Label();
            this.label_Objects = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.includeReservedCheckBox = new System.Windows.Forms.CheckBox();
            this.listView1 = new SonicPlugin.ListViewNF();
            this.typeHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.objXHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.objYHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.miscHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label_Watch1
            // 
            this.label_Watch1.AutoEllipsis = true;
            this.label_Watch1.Location = new System.Drawing.Point(12, 9);
            this.label_Watch1.Name = "label_Watch1";
            this.label_Watch1.Size = new System.Drawing.Size(116, 14);
            this.label_Watch1.TabIndex = 0;
            this.label_Watch1.Text = "label1";
            // 
            // label_Watch2
            // 
            this.label_Watch2.AutoEllipsis = true;
            this.label_Watch2.Location = new System.Drawing.Point(12, 27);
            this.label_Watch2.Name = "label_Watch2";
            this.label_Watch2.Size = new System.Drawing.Size(116, 16);
            this.label_Watch2.TabIndex = 1;
            this.label_Watch2.Text = "label1";
            // 
            // label_Objects
            // 
            this.label_Objects.AutoSize = true;
            this.label_Objects.Location = new System.Drawing.Point(12, 44);
            this.label_Objects.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_Objects.Name = "label_Objects";
            this.label_Objects.Size = new System.Drawing.Size(35, 13);
            this.label_Objects.TabIndex = 3;
            this.label_Objects.Text = "label1";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 75);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(116, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Calc && Draw Map";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 103);
            this.button2.Margin = new System.Windows.Forms.Padding(2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(50, 24);
            this.button2.TabIndex = 5;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // includeReservedCheckBox
            // 
            this.includeReservedCheckBox.AutoSize = true;
            this.includeReservedCheckBox.Location = new System.Drawing.Point(12, 132);
            this.includeReservedCheckBox.Name = "includeReservedCheckBox";
            this.includeReservedCheckBox.Size = new System.Drawing.Size(107, 30);
            this.includeReservedCheckBox.TabIndex = 6;
            this.includeReservedCheckBox.Text = "include reserved \r\nObjects";
            this.includeReservedCheckBox.UseVisualStyleBackColor = true;
            this.includeReservedCheckBox.CheckedChanged += new System.EventHandler(this.includeReservedObjects_CheckedChanged);
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.typeHeader,
            this.objXHeader,
            this.objYHeader,
            this.miscHeader});
            this.listView1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.listView1.Location = new System.Drawing.Point(134, 12);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(421, 305);
            this.listView1.TabIndex = 2;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // typeHeader
            // 
            this.typeHeader.Text = "Object Type";
            this.typeHeader.Width = 149;
            // 
            // objXHeader
            // 
            this.objXHeader.Text = "Hitbox X";
            this.objXHeader.Width = 78;
            // 
            // objYHeader
            // 
            this.objYHeader.Text = "Hitbox Y";
            // 
            // miscHeader
            // 
            this.miscHeader.Text = "Subtype (high nibble)";
            this.miscHeader.Width = 118;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(12, 168);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(85, 17);
            this.checkBox1.TabIndex = 7;
            this.checkBox1.Text = "debug mode";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(39, 213);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 8;
            this.button3.Text = "controller";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // CustomMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(567, 331);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.includeReservedCheckBox);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label_Objects);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.label_Watch2);
            this.Controls.Add(this.label_Watch1);
            this.Name = "CustomMainForm";
            this.Text = "MainForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_Watch1;
        private System.Windows.Forms.Label label_Watch2;
        private ListViewNF listView1;
        private System.Windows.Forms.ColumnHeader typeHeader;
        private System.Windows.Forms.ColumnHeader objXHeader;
        private System.Windows.Forms.Label label_Objects;
        private System.Windows.Forms.ColumnHeader objYHeader;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox includeReservedCheckBox;
        private System.Windows.Forms.ColumnHeader miscHeader;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button button3;
    }
}