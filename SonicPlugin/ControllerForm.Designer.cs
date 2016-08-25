namespace SonicPlugin
{
    partial class ControllerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControllerForm));
            this.controllerBox = new SonicPlugin.ControllerBox();
            ((System.ComponentModel.ISupportInitialize)(this.controllerBox)).BeginInit();
            this.SuspendLayout();
            // 
            // controllerBox
            // 
            this.controllerBox.A = false;
            this.controllerBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.controllerBox.B = false;
            this.controllerBox.C = false;
            this.controllerBox.Image = ((System.Drawing.Image)(resources.GetObject("controllerBox.Image")));
            this.controllerBox.Location = new System.Drawing.Point(12, 12);
            this.controllerBox.Name = "controllerBox";
            this.controllerBox.PadDown = false;
            this.controllerBox.PadLeft = false;
            this.controllerBox.PadRight = false;
            this.controllerBox.PadUp = false;
            this.controllerBox.Size = new System.Drawing.Size(300, 191);
            this.controllerBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.controllerBox.TabIndex = 0;
            this.controllerBox.TabStop = false;
            // 
            // ControllerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 213);
            this.Controls.Add(this.controllerBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ControllerForm";
            this.Text = "Controller";
            ((System.ComponentModel.ISupportInitialize)(this.controllerBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ControllerBox controllerBox;
    }
}