namespace TroySecurePortMonitorUserInterface
{
    partial class DataCaptureListForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataCaptureListForm));
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.dgvDataCap = new System.Windows.Forms.DataGridView();
            this.IndexId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DataCapType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Format = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RemoveData = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FontNameCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TagCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RemoveStringCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDataCap)).BeginInit();
            this.SuspendLayout();
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(34, 262);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 1;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(126, 261);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Text = "Del";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(221, 261);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 3;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(500, 291);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(394, 291);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 8;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // dgvDataCap
            // 
            this.dgvDataCap.AllowUserToAddRows = false;
            this.dgvDataCap.AllowUserToDeleteRows = false;
            this.dgvDataCap.AllowUserToResizeColumns = false;
            this.dgvDataCap.AllowUserToResizeRows = false;
            this.dgvDataCap.BackgroundColor = System.Drawing.Color.Gainsboro;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvDataCap.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvDataCap.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvDataCap.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IndexId,
            this.DataCapType,
            this.Format,
            this.RemoveData,
            this.FontNameCount,
            this.TagCount,
            this.RemoveStringCount});
            this.dgvDataCap.Location = new System.Drawing.Point(21, 12);
            this.dgvDataCap.MultiSelect = false;
            this.dgvDataCap.Name = "dgvDataCap";
            this.dgvDataCap.RowHeadersWidth = 25;
            this.dgvDataCap.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDataCap.Size = new System.Drawing.Size(545, 225);
            this.dgvDataCap.TabIndex = 10;
            // 
            // IndexId
            // 
            this.IndexId.HeaderText = "Index";
            this.IndexId.Name = "IndexId";
            this.IndexId.Visible = false;
            this.IndexId.Width = 50;
            // 
            // DataCapType
            // 
            this.DataCapType.HeaderText = "Type";
            this.DataCapType.Name = "DataCapType";
            this.DataCapType.Width = 210;
            // 
            // Format
            // 
            this.Format.HeaderText = "Format";
            this.Format.Name = "Format";
            this.Format.ReadOnly = true;
            this.Format.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Format.Width = 210;
            // 
            // RemoveData
            // 
            this.RemoveData.HeaderText = "Remove Data";
            this.RemoveData.Name = "RemoveData";
            this.RemoveData.Width = 98;
            // 
            // FontNameCount
            // 
            this.FontNameCount.HeaderText = "Font Name Count";
            this.FontNameCount.Name = "FontNameCount";
            this.FontNameCount.Visible = false;
            this.FontNameCount.Width = 115;
            // 
            // TagCount
            // 
            this.TagCount.HeaderText = "Tag Count";
            this.TagCount.Name = "TagCount";
            this.TagCount.Visible = false;
            this.TagCount.Width = 82;
            // 
            // RemoveStringCount
            // 
            this.RemoveStringCount.HeaderText = "Remove String Count";
            this.RemoveStringCount.Name = "RemoveStringCount";
            this.RemoveStringCount.Visible = false;
            this.RemoveStringCount.Width = 133;
            // 
            // DataCaptureListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(588, 334);
            this.Controls.Add(this.dgvDataCap);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnAdd);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DataCaptureListForm";
            this.Text = "Data Capture List";
            this.Load += new System.EventHandler(this.DataCaptureListForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDataCap)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.DataGridView dgvDataCap;
        private System.Windows.Forms.DataGridViewTextBoxColumn IndexId;
        private System.Windows.Forms.DataGridViewTextBoxColumn DataCapType;
        private System.Windows.Forms.DataGridViewTextBoxColumn Format;
        private System.Windows.Forms.DataGridViewTextBoxColumn RemoveData;
        private System.Windows.Forms.DataGridViewTextBoxColumn FontNameCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn TagCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn RemoveStringCount;
    }
}