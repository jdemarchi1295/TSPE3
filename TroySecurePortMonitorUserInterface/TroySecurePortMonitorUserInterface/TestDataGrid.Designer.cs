namespace TroySecurePortMonitorUserInterface
{
    partial class TestDataGrid
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.DataCapType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DataTagsCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RemoveStringsCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FontNamesCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RemoveData = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.PlainText = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DataCapType,
            this.DataTagsCount,
            this.RemoveStringsCount,
            this.FontNamesCount,
            this.RemoveData,
            this.PlainText});
            this.dataGridView1.Cursor = System.Windows.Forms.Cursors.Cross;
            this.dataGridView1.Location = new System.Drawing.Point(12, 12);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataGridView1.RowHeadersWidth = 20;
            this.dataGridView1.Size = new System.Drawing.Size(760, 235);
            this.dataGridView1.TabIndex = 0;
            // 
            // DataCapType
            // 
            this.DataCapType.HeaderText = "Data Capture Type";
            this.DataCapType.Name = "DataCapType";
            this.DataCapType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.DataCapType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.DataCapType.Width = 125;
            // 
            // DataTagsCount
            // 
            this.DataTagsCount.HeaderText = "Data Tags Count";
            this.DataTagsCount.Name = "DataTagsCount";
            this.DataTagsCount.Width = 105;
            // 
            // RemoveStringsCount
            // 
            this.RemoveStringsCount.HeaderText = "Remove Strings Count";
            this.RemoveStringsCount.Name = "RemoveStringsCount";
            this.RemoveStringsCount.Width = 125;
            // 
            // FontNamesCount
            // 
            this.FontNamesCount.HeaderText = "Font Names Count";
            this.FontNamesCount.Name = "FontNamesCount";
            this.FontNamesCount.Width = 115;
            // 
            // RemoveData
            // 
            this.RemoveData.HeaderText = "Remove Data";
            this.RemoveData.Name = "RemoveData";
            this.RemoveData.Width = 80;
            // 
            // PlainText
            // 
            this.PlainText.HeaderText = "Plain Text";
            this.PlainText.Name = "PlainText";
            this.PlainText.Width = 65;
            // 
            // TestDataGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 421);
            this.Controls.Add(this.dataGridView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "TestDataGrid";
            this.Text = "TestDataGrid";
            this.Load += new System.EventHandler(this.TestDataGrid_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn DataCapType;
        private System.Windows.Forms.DataGridViewTextBoxColumn DataTagsCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn RemoveStringsCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn FontNamesCount;
        private System.Windows.Forms.DataGridViewCheckBoxColumn RemoveData;
        private System.Windows.Forms.DataGridViewCheckBoxColumn PlainText;
    }
}