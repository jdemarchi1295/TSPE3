namespace TroySecurePortMonitorUserInterface
{
    partial class AddExclusion
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
            this.txtPgHeight = new System.Windows.Forms.TextBox();
            this.txtPgWidth = new System.Windows.Forms.TextBox();
            this.lblPgHeight = new System.Windows.Forms.Label();
            this.lblPgWidth = new System.Windows.Forms.Label();
            this.txtPgYAnchor = new System.Windows.Forms.TextBox();
            this.lblPgYAnchor = new System.Windows.Forms.Label();
            this.txtPgXAnchor = new System.Windows.Forms.TextBox();
            this.lblPgXAnchor = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtPgHeight
            // 
            this.txtPgHeight.Location = new System.Drawing.Point(464, 26);
            this.txtPgHeight.MaxLength = 6;
            this.txtPgHeight.Name = "txtPgHeight";
            this.txtPgHeight.Size = new System.Drawing.Size(45, 20);
            this.txtPgHeight.TabIndex = 15;
            // 
            // txtPgWidth
            // 
            this.txtPgWidth.Location = new System.Drawing.Point(337, 26);
            this.txtPgWidth.MaxLength = 6;
            this.txtPgWidth.Name = "txtPgWidth";
            this.txtPgWidth.Size = new System.Drawing.Size(45, 20);
            this.txtPgWidth.TabIndex = 14;
            // 
            // lblPgHeight
            // 
            this.lblPgHeight.AutoSize = true;
            this.lblPgHeight.Location = new System.Drawing.Point(419, 29);
            this.lblPgHeight.Name = "lblPgHeight";
            this.lblPgHeight.Size = new System.Drawing.Size(41, 13);
            this.lblPgHeight.TabIndex = 13;
            this.lblPgHeight.Text = "Height:";
            // 
            // lblPgWidth
            // 
            this.lblPgWidth.AutoSize = true;
            this.lblPgWidth.Location = new System.Drawing.Point(294, 29);
            this.lblPgWidth.Name = "lblPgWidth";
            this.lblPgWidth.Size = new System.Drawing.Size(38, 13);
            this.lblPgWidth.TabIndex = 12;
            this.lblPgWidth.Text = "Width:";
            // 
            // txtPgYAnchor
            // 
            this.txtPgYAnchor.Location = new System.Drawing.Point(211, 26);
            this.txtPgYAnchor.MaxLength = 6;
            this.txtPgYAnchor.Name = "txtPgYAnchor";
            this.txtPgYAnchor.Size = new System.Drawing.Size(45, 20);
            this.txtPgYAnchor.TabIndex = 11;
            // 
            // lblPgYAnchor
            // 
            this.lblPgYAnchor.AutoSize = true;
            this.lblPgYAnchor.Location = new System.Drawing.Point(153, 29);
            this.lblPgYAnchor.Name = "lblPgYAnchor";
            this.lblPgYAnchor.Size = new System.Drawing.Size(54, 13);
            this.lblPgYAnchor.TabIndex = 10;
            this.lblPgYAnchor.Text = "Y Anchor:";
            // 
            // txtPgXAnchor
            // 
            this.txtPgXAnchor.Location = new System.Drawing.Point(70, 26);
            this.txtPgXAnchor.MaxLength = 6;
            this.txtPgXAnchor.Name = "txtPgXAnchor";
            this.txtPgXAnchor.Size = new System.Drawing.Size(45, 20);
            this.txtPgXAnchor.TabIndex = 9;
            // 
            // lblPgXAnchor
            // 
            this.lblPgXAnchor.AutoSize = true;
            this.lblPgXAnchor.Location = new System.Drawing.Point(12, 29);
            this.lblPgXAnchor.Name = "lblPgXAnchor";
            this.lblPgXAnchor.Size = new System.Drawing.Size(54, 13);
            this.lblPgXAnchor.TabIndex = 8;
            this.lblPgXAnchor.Text = "X Anchor:";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(328, 69);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 16;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(422, 69);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 17;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "(Units 1/600 inch)";
            // 
            // AddExclusion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(524, 107);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtPgHeight);
            this.Controls.Add(this.txtPgWidth);
            this.Controls.Add(this.lblPgHeight);
            this.Controls.Add(this.lblPgWidth);
            this.Controls.Add(this.txtPgYAnchor);
            this.Controls.Add(this.lblPgYAnchor);
            this.Controls.Add(this.txtPgXAnchor);
            this.Controls.Add(this.lblPgXAnchor);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "AddExclusion";
            this.Text = "Add Exclusion Area";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtPgHeight;
        private System.Windows.Forms.TextBox txtPgWidth;
        private System.Windows.Forms.Label lblPgHeight;
        private System.Windows.Forms.Label lblPgWidth;
        private System.Windows.Forms.TextBox txtPgYAnchor;
        private System.Windows.Forms.Label lblPgYAnchor;
        private System.Windows.Forms.TextBox txtPgXAnchor;
        private System.Windows.Forms.Label lblPgXAnchor;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
    }
}