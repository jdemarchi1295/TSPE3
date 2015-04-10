namespace TroySecurePortMonitorUserInterface
{
    partial class SplashForm
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
            this.lblVersion = new System.Windows.Forms.Label();
            this.lblLicenseTitle = new System.Windows.Forms.Label();
            this.lblLicenseCount = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btn_alf = new System.Windows.Forms.Button();
            this.btn_lrf = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.BackColor = System.Drawing.Color.Transparent;
            this.lblVersion.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVersion.ForeColor = System.Drawing.SystemColors.Info;
            this.lblVersion.Location = new System.Drawing.Point(472, 9);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(81, 16);
            this.lblVersion.TabIndex = 0;
            this.lblVersion.Text = "Version 3.0";
            // 
            // lblLicenseTitle
            // 
            this.lblLicenseTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblLicenseTitle.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLicenseTitle.ForeColor = System.Drawing.SystemColors.Info;
            this.lblLicenseTitle.Location = new System.Drawing.Point(472, 35);
            this.lblLicenseTitle.Name = "lblLicenseTitle";
            this.lblLicenseTitle.Size = new System.Drawing.Size(140, 19);
            this.lblLicenseTitle.TabIndex = 1;
            this.lblLicenseTitle.Text = "License Status:";
            // 
            // lblLicenseCount
            // 
            this.lblLicenseCount.BackColor = System.Drawing.Color.Transparent;
            this.lblLicenseCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLicenseCount.ForeColor = System.Drawing.SystemColors.Info;
            this.lblLicenseCount.Location = new System.Drawing.Point(589, 35);
            this.lblLicenseCount.Name = "lblLicenseCount";
            this.lblLicenseCount.Size = new System.Drawing.Size(140, 19);
            this.lblLicenseCount.TabIndex = 2;
            this.lblLicenseCount.Text = "License";
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("Calibri", 36F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(20, 88);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(709, 61);
            this.lblTitle.TabIndex = 5;
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btn_alf
            // 
            this.btn_alf.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btn_alf.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btn_alf.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_alf.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btn_alf.Location = new System.Drawing.Point(644, 483);
            this.btn_alf.Name = "btn_alf";
            this.btn_alf.Size = new System.Drawing.Size(74, 35);
            this.btn_alf.TabIndex = 6;
            this.btn_alf.Text = "Activate License";
            this.btn_alf.UseVisualStyleBackColor = false;
            this.btn_alf.Click += new System.EventHandler(this.btn_alf_Click);
            // 
            // btn_lrf
            // 
            this.btn_lrf.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btn_lrf.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btn_lrf.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_lrf.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btn_lrf.Location = new System.Drawing.Point(538, 483);
            this.btn_lrf.Name = "btn_lrf";
            this.btn_lrf.Size = new System.Drawing.Size(74, 35);
            this.btn_lrf.TabIndex = 7;
            this.btn_lrf.Text = "Request License";
            this.btn_lrf.UseVisualStyleBackColor = false;
            this.btn_lrf.Click += new System.EventHandler(this.btn_lrf_Click);
            // 
            // SplashForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = global::TroySecurePortMonitorUserInterface.Properties.Resources.Software_Ripple_795x608;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(741, 530);
            this.Controls.Add(this.btn_lrf);
            this.Controls.Add(this.btn_alf);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblLicenseCount);
            this.Controls.Add(this.lblLicenseTitle);
            this.Controls.Add(this.lblVersion);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SplashForm";
            this.Load += new System.EventHandler(this.SplashForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label lblLicenseTitle;
        private System.Windows.Forms.Label lblLicenseCount;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btn_alf;
        private System.Windows.Forms.Button btn_lrf;

    }
}