namespace TroySecurePortMonitorUserInterface
{
    partial class InterferencePatterns
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
            this.pbIntPatterns = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.numIntPattern = new System.Windows.Forms.NumericUpDown();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pbIntPatterns)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIntPattern)).BeginInit();
            this.SuspendLayout();
            // 
            // pbIntPatterns
            // 
            this.pbIntPatterns.Image = global::TroySecurePortMonitorUserInterface.Properties.Resources.IntPatterns;
            this.pbIntPatterns.Location = new System.Drawing.Point(2, 4);
            this.pbIntPatterns.Name = "pbIntPatterns";
            this.pbIntPatterns.Size = new System.Drawing.Size(821, 502);
            this.pbIntPatterns.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbIntPatterns.TabIndex = 0;
            this.pbIntPatterns.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(431, 526);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select a Pattern:";
            // 
            // numIntPattern
            // 
            this.numIntPattern.Location = new System.Drawing.Point(532, 524);
            this.numIntPattern.Name = "numIntPattern";
            this.numIntPattern.Size = new System.Drawing.Size(47, 20);
            this.numIntPattern.TabIndex = 2;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(616, 518);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(82, 29);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(717, 518);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(82, 29);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // InterferencePatterns
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(825, 570);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.numIntPattern);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pbIntPatterns);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "InterferencePatterns";
            this.Text = "InterferencePatterns";
            this.Load += new System.EventHandler(this.InterferencePatterns_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbIntPatterns)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIntPattern)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbIntPatterns;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numIntPattern;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}