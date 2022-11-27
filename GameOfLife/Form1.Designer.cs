namespace GameOfLife
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tmrGenTick = new System.Windows.Forms.Timer(this.components);
            this.lblGenCount = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tmrGenTick
            // 
            this.tmrGenTick.Enabled = true;
            this.tmrGenTick.Interval = 1;
            this.tmrGenTick.Tick += new System.EventHandler(this.tmrGenTick_Tick);
            // 
            // lblGenCount
            // 
            this.lblGenCount.AutoSize = true;
            this.lblGenCount.Location = new System.Drawing.Point(3, 3);
            this.lblGenCount.Name = "lblGenCount";
            this.lblGenCount.Size = new System.Drawing.Size(74, 15);
            this.lblGenCount.TabIndex = 0;
            this.lblGenCount.Text = "lblGenCount";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1029, 1049);
            this.Controls.Add(this.lblGenCount);
            this.Name = "Form1";
            this.Text = "Left Mouse = Random, Right Mouse = Spawn, Space = Clear";
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Form1_KeyPress);
            this.Leave += new System.EventHandler(this.Form1_Leave);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer tmrGenTick;
        private Label lblGenCount;
    }
}