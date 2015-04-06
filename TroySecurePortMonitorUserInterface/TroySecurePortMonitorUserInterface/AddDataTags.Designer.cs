namespace TroySecurePortMonitorUserInterface
{
    partial class AddDataTags
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddDataTags));
            this.label1 = new System.Windows.Forms.Label();
            this.txtLeadingTag = new System.Windows.Forms.TextBox();
            this.txtTrailing = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chkIncludeLeading = new System.Windows.Forms.CheckBox();
            this.txtTrailingString = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtLeadingString = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblPjlNote = new System.Windows.Forms.Label();
            this.chkUseLF = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.chkOnePerPage = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Leading Tag:";
            // 
            // txtLeadingTag
            // 
            this.txtLeadingTag.Location = new System.Drawing.Point(103, 32);
            this.txtLeadingTag.Name = "txtLeadingTag";
            this.txtLeadingTag.Size = new System.Drawing.Size(170, 20);
            this.txtLeadingTag.TabIndex = 1;
            // 
            // txtTrailing
            // 
            this.txtTrailing.Location = new System.Drawing.Point(103, 88);
            this.txtTrailing.Name = "txtTrailing";
            this.txtTrailing.Size = new System.Drawing.Size(170, 20);
            this.txtTrailing.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Trailing Tag:";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(146, 402);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(238, 402);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // chkIncludeLeading
            // 
            this.chkIncludeLeading.AutoSize = true;
            this.chkIncludeLeading.Location = new System.Drawing.Point(29, 205);
            this.chkIncludeLeading.Name = "chkIncludeLeading";
            this.chkIncludeLeading.Size = new System.Drawing.Size(192, 17);
            this.chkIncludeLeading.TabIndex = 6;
            this.chkIncludeLeading.Text = "Include Leading Tag In TROYmark";
            this.chkIncludeLeading.UseVisualStyleBackColor = true;
            // 
            // txtTrailingString
            // 
            this.txtTrailingString.Location = new System.Drawing.Point(89, 61);
            this.txtTrailingString.Name = "txtTrailingString";
            this.txtTrailingString.Size = new System.Drawing.Size(170, 20);
            this.txtTrailingString.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Trailing String:";
            // 
            // txtLeadingString
            // 
            this.txtLeadingString.Location = new System.Drawing.Point(91, 20);
            this.txtLeadingString.Name = "txtLeadingString";
            this.txtLeadingString.Size = new System.Drawing.Size(170, 20);
            this.txtLeadingString.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(78, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Leading String:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtLeadingString);
            this.groupBox1.Controls.Add(this.txtTrailingString);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(12, 242);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(280, 100);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Optional Output Text for TROYmark...";
            // 
            // lblPjlNote
            // 
            this.lblPjlNote.AutoSize = true;
            this.lblPjlNote.Location = new System.Drawing.Point(96, 55);
            this.lblPjlNote.Name = "lblPjlNote";
            this.lblPjlNote.Size = new System.Drawing.Size(189, 13);
            this.lblPjlNote.TabIndex = 12;
            this.lblPjlNote.Text = "Note: Include trailing \'=\' for PJL strings.";
            this.lblPjlNote.Visible = false;
            // 
            // chkUseLF
            // 
            this.chkUseLF.AutoSize = true;
            this.chkUseLF.Location = new System.Drawing.Point(85, 114);
            this.chkUseLF.Name = "chkUseLF";
            this.chkUseLF.Size = new System.Drawing.Size(209, 17);
            this.chkUseLF.TabIndex = 13;
            this.chkUseLF.Text = "Use Line Feed for Trailing (PJL Format)";
            this.chkUseLF.UseVisualStyleBackColor = true;
            this.chkUseLF.Visible = false;
            this.chkUseLF.CheckedChanged += new System.EventHandler(this.chkUseLF_CheckedChanged);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(26, 143);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(259, 40);
            this.label5.TabIndex = 14;
            this.label5.Text = "Note: Use /e for Escape, /r for Carriage Return and /n for Line Feed.";
            // 
            // chkOnePerPage
            // 
            this.chkOnePerPage.AutoSize = true;
            this.chkOnePerPage.Location = new System.Drawing.Point(24, 358);
            this.chkOnePerPage.Name = "chkOnePerPage";
            this.chkOnePerPage.Size = new System.Drawing.Size(122, 17);
            this.chkOnePerPage.TabIndex = 15;
            this.chkOnePerPage.Text = "Find Once Per Page";
            this.chkOnePerPage.UseVisualStyleBackColor = true;
            // 
            // AddDataTags
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(337, 437);
            this.Controls.Add(this.chkOnePerPage);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.chkUseLF);
            this.Controls.Add(this.lblPjlNote);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.chkIncludeLeading);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtTrailing);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtLeadingTag);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AddDataTags";
            this.Text = "Add Data Tags";
            this.Load += new System.EventHandler(this.AddDataTags_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtLeadingTag;
        private System.Windows.Forms.TextBox txtTrailing;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chkIncludeLeading;
        private System.Windows.Forms.TextBox txtTrailingString;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtLeadingString;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblPjlNote;
        private System.Windows.Forms.CheckBox chkUseLF;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chkOnePerPage;
    }
}