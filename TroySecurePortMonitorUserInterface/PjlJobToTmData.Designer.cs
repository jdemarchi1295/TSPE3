namespace TroySecurePortMonitorUserInterface
{
    partial class PjlJobToTmData
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PjlJobToTmData));
            this.dgvPjlToTmMap = new System.Windows.Forms.DataGridView();
            this.btnEditTextMap = new System.Windows.Forms.Button();
            this.btnDelTextMap = new System.Windows.Forms.Button();
            this.btnAddTextMap = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.MapText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PrintQueue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPjlToTmMap)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvPjlToTmMap
            // 
            this.dgvPjlToTmMap.AllowUserToAddRows = false;
            this.dgvPjlToTmMap.AllowUserToDeleteRows = false;
            this.dgvPjlToTmMap.BackgroundColor = System.Drawing.Color.Gainsboro;
            this.dgvPjlToTmMap.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPjlToTmMap.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.MapText,
            this.PrintQueue});
            this.dgvPjlToTmMap.Location = new System.Drawing.Point(29, 31);
            this.dgvPjlToTmMap.MultiSelect = false;
            this.dgvPjlToTmMap.Name = "dgvPjlToTmMap";
            this.dgvPjlToTmMap.RowHeadersWidth = 25;
            this.dgvPjlToTmMap.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvPjlToTmMap.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPjlToTmMap.Size = new System.Drawing.Size(528, 212);
            this.dgvPjlToTmMap.TabIndex = 11;
            // 
            // btnEditTextMap
            // 
            this.btnEditTextMap.Location = new System.Drawing.Point(158, 249);
            this.btnEditTextMap.Name = "btnEditTextMap";
            this.btnEditTextMap.Size = new System.Drawing.Size(59, 23);
            this.btnEditTextMap.TabIndex = 14;
            this.btnEditTextMap.Text = "Edit";
            this.btnEditTextMap.UseVisualStyleBackColor = true;
            this.btnEditTextMap.Click += new System.EventHandler(this.btnEditTextMap_Click);
            // 
            // btnDelTextMap
            // 
            this.btnDelTextMap.Location = new System.Drawing.Point(94, 249);
            this.btnDelTextMap.Name = "btnDelTextMap";
            this.btnDelTextMap.Size = new System.Drawing.Size(59, 23);
            this.btnDelTextMap.TabIndex = 13;
            this.btnDelTextMap.Text = "Del";
            this.btnDelTextMap.UseVisualStyleBackColor = true;
            this.btnDelTextMap.Click += new System.EventHandler(this.btnDelTextMap_Click);
            // 
            // btnAddTextMap
            // 
            this.btnAddTextMap.Location = new System.Drawing.Point(29, 249);
            this.btnAddTextMap.Name = "btnAddTextMap";
            this.btnAddTextMap.Size = new System.Drawing.Size(59, 23);
            this.btnAddTextMap.TabIndex = 12;
            this.btnAddTextMap.Text = "Add";
            this.btnAddTextMap.UseVisualStyleBackColor = true;
            this.btnAddTextMap.Click += new System.EventHandler(this.btnAddTextMap_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(483, 274);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 16;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(392, 274);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 15;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // MapText
            // 
            this.MapText.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.MapText.HeaderText = "PJL Job Name";
            this.MapText.Name = "MapText";
            this.MapText.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.MapText.Width = 250;
            // 
            // PrintQueue
            // 
            this.PrintQueue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.PrintQueue.HeaderText = "Troymark Text";
            this.PrintQueue.Name = "PrintQueue";
            this.PrintQueue.Width = 250;
            // 
            // PjlJobToTmData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(590, 311);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnEditTextMap);
            this.Controls.Add(this.btnDelTextMap);
            this.Controls.Add(this.btnAddTextMap);
            this.Controls.Add(this.dgvPjlToTmMap);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PjlJobToTmData";
            this.Text = "PJL Job Name to Troymark Data Map";
            this.Load += new System.EventHandler(this.PjlJobToTmData_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPjlToTmMap)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvPjlToTmMap;
        private System.Windows.Forms.Button btnEditTextMap;
        private System.Windows.Forms.Button btnDelTextMap;
        private System.Windows.Forms.Button btnAddTextMap;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.DataGridViewTextBoxColumn MapText;
        private System.Windows.Forms.DataGridViewTextBoxColumn PrintQueue;
    }
}