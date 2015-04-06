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
            this.btnEnableLicense = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
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
            // btnEnableLicense
            // 
            this.btnEnableLicense.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnEnableLicense.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnEnableLicense.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEnableLicense.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnEnableLicense.Location = new System.Drawing.Point(565, 483);
            this.btnEnableLicense.Name = "btnEnableLicense";
            this.btnEnableLicense.Size = new System.Drawing.Size(74, 35);
            this.btnEnableLicense.TabIndex = 4;
            this.btnEnableLicense.Text = "Enable License";
            this.btnEnableLicense.UseVisualStyleBackColor = false;
            this.btnEnableLicense.Click += new System.EventHandler(this.btnEnableLicense_Click);
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
            // SplashForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = global::TroySecurePortMonitorUserInterface.Properties.Resources.Software_Ripple_795x608;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(741, 530);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.btnEnableLicense);
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
        private System.Windows.Forms.Button btnEnableLicense;
        private System.Windows.Forms.Label lblTitle;

    }
}