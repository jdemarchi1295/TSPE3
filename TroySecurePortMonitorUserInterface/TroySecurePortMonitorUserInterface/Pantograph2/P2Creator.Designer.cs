namespace TroySecurePortMonitorUserInterface.Pantograph2
            //THIS IS A TEST
{
    partial class P2Creator
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
            this.cmdGenerate = new System.Windows.Forms.Button();
            this.lblRange = new System.Windows.Forms.Label();
            this.trkMin = new System.Windows.Forms.TrackBar();
            this.trkMax = new System.Windows.Forms.TrackBar();
            this.cmdReset = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.grpStep1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.grpStep2 = new System.Windows.Forms.GroupBox();
            this.txtCustomPtnName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtCellFG = new System.Windows.Forms.TextBox();
            this.txtPageNumberFG = new System.Windows.Forms.TextBox();
            this.cmdSavePatterns = new System.Windows.Forms.Button();
            this.cmdGenSubRange = new System.Windows.Forms.Button();
            this.lblCell = new System.Windows.Forms.Label();
            this.txtCellBG = new System.Windows.Forms.TextBox();
            this.lblPageNum = new System.Windows.Forms.Label();
            this.txtPageNumberBG = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.trkMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkMax)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.grpStep1.SuspendLayout();
            this.grpStep2.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmdGenerate
            // 
            this.cmdGenerate.Location = new System.Drawing.Point(304, 53);
            this.cmdGenerate.Name = "cmdGenerate";
            this.cmdGenerate.Size = new System.Drawing.Size(110, 56);
            this.cmdGenerate.TabIndex = 1;
            this.cmdGenerate.Text = "Generate Next Page";
            this.cmdGenerate.UseVisualStyleBackColor = true;
            this.cmdGenerate.Click += new System.EventHandler(this.cmdGenerate_Click);
            // 
            // lblRange
            // 
            this.lblRange.AutoSize = true;
            this.lblRange.Location = new System.Drawing.Point(30, 110);
            this.lblRange.Name = "lblRange";
            this.lblRange.Size = new System.Drawing.Size(35, 13);
            this.lblRange.TabIndex = 4;
            this.lblRange.Text = "label1";
            // 
            // trkMin
            // 
            this.trkMin.Location = new System.Drawing.Point(22, 53);
            this.trkMin.Maximum = 100;
            this.trkMin.Name = "trkMin";
            this.trkMin.Size = new System.Drawing.Size(254, 45);
            this.trkMin.TabIndex = 5;
            this.trkMin.TabStop = false;
            this.trkMin.TickFrequency = 10;
            this.trkMin.Scroll += new System.EventHandler(this.trkMin_Scroll);
            // 
            // trkMax
            // 
            this.trkMax.Location = new System.Drawing.Point(22, 78);
            this.trkMax.Maximum = 100;
            this.trkMax.Name = "trkMax";
            this.trkMax.Size = new System.Drawing.Size(254, 45);
            this.trkMax.TabIndex = 6;
            this.trkMax.TabStop = false;
            this.trkMax.TickFrequency = 10;
            this.trkMax.Value = 100;
            this.trkMax.Scroll += new System.EventHandler(this.trkMax_Scroll);
            // 
            // cmdReset
            // 
            this.cmdReset.Location = new System.Drawing.Point(395, 19);
            this.cmdReset.Name = "cmdReset";
            this.cmdReset.Size = new System.Drawing.Size(44, 22);
            this.cmdReset.TabIndex = 7;
            this.cmdReset.Text = "Reset";
            this.cmdReset.UseVisualStyleBackColor = true;
            this.cmdReset.Visible = false;
            this.cmdReset.Click += new System.EventHandler(this.cmdReset_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 424);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(512, 22);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(39, 17);
            this.lblStatus.Text = "Status";
            // 
            // grpStep1
            // 
            this.grpStep1.Controls.Add(this.label1);
            this.grpStep1.Controls.Add(this.cmdGenerate);
            this.grpStep1.Controls.Add(this.cmdReset);
            this.grpStep1.Controls.Add(this.lblRange);
            this.grpStep1.Controls.Add(this.trkMax);
            this.grpStep1.Controls.Add(this.trkMin);
            this.grpStep1.Location = new System.Drawing.Point(12, 12);
            this.grpStep1.Name = "grpStep1";
            this.grpStep1.Size = new System.Drawing.Size(445, 147);
            this.grpStep1.TabIndex = 9;
            this.grpStep1.TabStop = false;
            this.grpStep1.Text = "Step 1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Density %";
            // 
            // grpStep2
            // 
            this.grpStep2.Controls.Add(this.txtCustomPtnName);
            this.grpStep2.Controls.Add(this.label4);
            this.grpStep2.Controls.Add(this.label3);
            this.grpStep2.Controls.Add(this.label2);
            this.grpStep2.Controls.Add(this.txtCellFG);
            this.grpStep2.Controls.Add(this.txtPageNumberFG);
            this.grpStep2.Controls.Add(this.cmdSavePatterns);
            this.grpStep2.Controls.Add(this.cmdGenSubRange);
            this.grpStep2.Controls.Add(this.lblCell);
            this.grpStep2.Controls.Add(this.txtCellBG);
            this.grpStep2.Controls.Add(this.lblPageNum);
            this.grpStep2.Controls.Add(this.txtPageNumberBG);
            this.grpStep2.Location = new System.Drawing.Point(12, 181);
            this.grpStep2.Name = "grpStep2";
            this.grpStep2.Size = new System.Drawing.Size(445, 220);
            this.grpStep2.TabIndex = 10;
            this.grpStep2.TabStop = false;
            this.grpStep2.Text = "Step 2";
            // 
            // txtCustomPtnName
            // 
            this.txtCustomPtnName.Location = new System.Drawing.Point(133, 133);
            this.txtCustomPtnName.Name = "txtCustomPtnName";
            this.txtCustomPtnName.Size = new System.Drawing.Size(171, 20);
            this.txtCustomPtnName.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 136);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(113, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Custom Pattern Name:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 101);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Foreground";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Background";
            // 
            // txtCellFG
            // 
            this.txtCellFG.Location = new System.Drawing.Point(143, 98);
            this.txtCellFG.Name = "txtCellFG";
            this.txtCellFG.Size = new System.Drawing.Size(46, 20);
            this.txtCellFG.TabIndex = 5;
            this.txtCellFG.Text = "B2";
            // 
            // txtPageNumberFG
            // 
            this.txtPageNumberFG.Location = new System.Drawing.Point(81, 98);
            this.txtPageNumberFG.Name = "txtPageNumberFG";
            this.txtPageNumberFG.Size = new System.Drawing.Size(46, 20);
            this.txtPageNumberFG.TabIndex = 4;
            this.txtPageNumberFG.Text = "1";
            // 
            // cmdSavePatterns
            // 
            this.cmdSavePatterns.Location = new System.Drawing.Point(329, 93);
            this.cmdSavePatterns.Name = "cmdSavePatterns";
            this.cmdSavePatterns.Size = new System.Drawing.Size(110, 56);
            this.cmdSavePatterns.TabIndex = 7;
            this.cmdSavePatterns.Text = "Save Patterns";
            this.cmdSavePatterns.UseVisualStyleBackColor = true;
            this.cmdSavePatterns.Click += new System.EventHandler(this.cmdSavePatterns_Click);
            // 
            // cmdGenSubRange
            // 
            this.cmdGenSubRange.Location = new System.Drawing.Point(356, 19);
            this.cmdGenSubRange.Name = "cmdGenSubRange";
            this.cmdGenSubRange.Size = new System.Drawing.Size(83, 22);
            this.cmdGenSubRange.TabIndex = 5;
            this.cmdGenSubRange.Text = "Generate Low";
            this.cmdGenSubRange.UseVisualStyleBackColor = true;
            this.cmdGenSubRange.Visible = false;
            this.cmdGenSubRange.Click += new System.EventHandler(this.cmdGenSubRange_Click);
            // 
            // lblCell
            // 
            this.lblCell.AutoSize = true;
            this.lblCell.Location = new System.Drawing.Point(140, 43);
            this.lblCell.Name = "lblCell";
            this.lblCell.Size = new System.Drawing.Size(24, 13);
            this.lblCell.TabIndex = 4;
            this.lblCell.Text = "Cell";
            // 
            // txtCellBG
            // 
            this.txtCellBG.Location = new System.Drawing.Point(143, 72);
            this.txtCellBG.Name = "txtCellBG";
            this.txtCellBG.Size = new System.Drawing.Size(46, 20);
            this.txtCellBG.TabIndex = 3;
            this.txtCellBG.Text = "A1";
            // 
            // lblPageNum
            // 
            this.lblPageNum.AutoSize = true;
            this.lblPageNum.Location = new System.Drawing.Point(78, 43);
            this.lblPageNum.Name = "lblPageNum";
            this.lblPageNum.Size = new System.Drawing.Size(32, 13);
            this.lblPageNum.TabIndex = 2;
            this.lblPageNum.Text = "Page";
            // 
            // txtPageNumberBG
            // 
            this.txtPageNumberBG.Location = new System.Drawing.Point(81, 72);
            this.txtPageNumberBG.Name = "txtPageNumberBG";
            this.txtPageNumberBG.Size = new System.Drawing.Size(46, 20);
            this.txtPageNumberBG.TabIndex = 2;
            this.txtPageNumberBG.Text = "1";
            // 
            // P2Creator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(512, 446);
            this.Controls.Add(this.grpStep2);
            this.Controls.Add(this.grpStep1);
            this.Controls.Add(this.statusStrip1);
            this.Name = "P2Creator";
            this.Text = "Pantograph 2.0 Creator";
            this.Load += new System.EventHandler(this.SimpleCreator_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trkMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkMax)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.grpStep1.ResumeLayout(false);
            this.grpStep1.PerformLayout();
            this.grpStep2.ResumeLayout(false);
            this.grpStep2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdGenerate;
        private System.Windows.Forms.Label lblRange;
        private System.Windows.Forms.TrackBar trkMin;
        private System.Windows.Forms.TrackBar trkMax;
        private System.Windows.Forms.Button cmdReset;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.GroupBox grpStep1;
        private System.Windows.Forms.GroupBox grpStep2;
        private System.Windows.Forms.Button cmdGenSubRange;
        private System.Windows.Forms.Label lblCell;
        private System.Windows.Forms.TextBox txtCellBG;
        private System.Windows.Forms.Label lblPageNum;
        private System.Windows.Forms.TextBox txtPageNumberBG;
        private System.Windows.Forms.Button cmdSavePatterns;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtCellFG;
        private System.Windows.Forms.TextBox txtPageNumberFG;
        private System.Windows.Forms.TextBox txtCustomPtnName;
        private System.Windows.Forms.Label label4;
    }
}