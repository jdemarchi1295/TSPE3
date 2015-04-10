namespace TroySecurePortMonitorUserInterface
{
    partial class WarningBoxConfig
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WarningBoxConfig));
            this.txtWbConfig = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnOKWB = new System.Windows.Forms.Button();
            this.btnCancelWB = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtWbConfig
            // 
            this.txtWbConfig.Location = new System.Drawing.Point(31, 58);
            this.txtWbConfig.Multiline = true;
            this.txtWbConfig.Name = "txtWbConfig";
            this.txtWbConfig.Size = new System.Drawing.Size(504, 183);
            this.txtWbConfig.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(163, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Warning Box Configuration String";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(43, 254);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(427, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "NOTE: use /s for STX, /t for TAB, /n for Line Feed, /r for Carriage Return and /d" +
                " for DEL";
            // 
            // btnOKWB
            // 
            this.btnOKWB.Location = new System.Drawing.Point(361, 301);
            this.btnOKWB.Name = "btnOKWB";
            this.btnOKWB.Size = new System.Drawing.Size(75, 23);
            this.btnOKWB.TabIndex = 3;
            this.btnOKWB.Text = "OK";
            this.btnOKWB.UseVisualStyleBackColor = true;
            this.btnOKWB.Click += new System.EventHandler(this.btnOKWB_Click);
            // 
            // btnCancelWB
            // 
            this.btnCancelWB.Location = new System.Drawing.Point(460, 301);
            this.btnCancelWB.Name = "btnCancelWB";
            this.btnCancelWB.Size = new System.Drawing.Size(75, 23);
            this.btnCancelWB.TabIndex = 4;
            this.btnCancelWB.Text = "Cancel";
            this.btnCancelWB.UseVisualStyleBackColor = true;
            this.btnCancelWB.Click += new System.EventHandler(this.btnCancelWB_Click);
            // 
            // WarningBoxConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(568, 345);
            this.Controls.Add(this.btnCancelWB);
            this.Controls.Add(this.btnOKWB);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtWbConfig);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "WarningBoxConfig";
            this.Text = "Warning Box Configuration";
            this.Load += new System.EventHandler(this.WarningBoxConfig_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtWbConfig;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnOKWB;
        private System.Windows.Forms.Button btnCancelWB;
    }
}