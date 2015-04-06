namespace TroySecurePortMonitorUserInterface
{
    partial class PassThrough
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PassThrough));
            this.dgvPassThroughStrings = new System.Windows.Forms.DataGridView();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnDel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtAddText = new System.Windows.Forms.TextBox();
            this.PtString = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gbAllOrOne = new System.Windows.Forms.GroupBox();
            this.gbStrings = new System.Windows.Forms.GroupBox();
            this.radOneString = new System.Windows.Forms.RadioButton();
            this.radAllString = new System.Windows.Forms.RadioButton();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chkEnablePassThrough = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPassThroughStrings)).BeginInit();
            this.gbAllOrOne.SuspendLayout();
            this.gbStrings.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvPassThroughStrings
            // 
            this.dgvPassThroughStrings.AllowUserToAddRows = false;
            this.dgvPassThroughStrings.AllowUserToDeleteRows = false;
            this.dgvPassThroughStrings.AllowUserToResizeColumns = false;
            this.dgvPassThroughStrings.AllowUserToResizeRows = false;
            this.dgvPassThroughStrings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPassThroughStrings.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PtString});
            this.dgvPassThroughStrings.Location = new System.Drawing.Point(11, 19);
            this.dgvPassThroughStrings.MultiSelect = false;
            this.dgvPassThroughStrings.Name = "dgvPassThroughStrings";
            this.dgvPassThroughStrings.ReadOnly = true;
            this.dgvPassThroughStrings.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPassThroughStrings.Size = new System.Drawing.Size(396, 150);
            this.dgvPassThroughStrings.TabIndex = 0;
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(200, 185);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(65, 23);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnDel
            // 
            this.btnDel.Location = new System.Drawing.Point(293, 185);
            this.btnDel.Name = "btnDel";
            this.btnDel.Size = new System.Drawing.Size(65, 23);
            this.btnDel.TabIndex = 3;
            this.btnDel.Text = "Del";
            this.btnDel.UseVisualStyleBackColor = true;
            this.btnDel.Click += new System.EventHandler(this.btnDel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 187);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Add:";
            // 
            // txtAddText
            // 
            this.txtAddText.Location = new System.Drawing.Point(57, 187);
            this.txtAddText.Name = "txtAddText";
            this.txtAddText.Size = new System.Drawing.Size(137, 20);
            this.txtAddText.TabIndex = 5;
            // 
            // PtString
            // 
            this.PtString.HeaderText = "Pass Through Strings";
            this.PtString.Name = "PtString";
            this.PtString.ReadOnly = true;
            this.PtString.Width = 350;
            // 
            // gbAllOrOne
            // 
            this.gbAllOrOne.Controls.Add(this.radAllString);
            this.gbAllOrOne.Controls.Add(this.radOneString);
            this.gbAllOrOne.Location = new System.Drawing.Point(16, 291);
            this.gbAllOrOne.Name = "gbAllOrOne";
            this.gbAllOrOne.Size = new System.Drawing.Size(329, 51);
            this.gbAllOrOne.TabIndex = 6;
            this.gbAllOrOne.TabStop = false;
            this.gbAllOrOne.Text = "All Or One...";
            this.gbAllOrOne.Enter += new System.EventHandler(this.gbAllOrOne_Enter);
            // 
            // gbStrings
            // 
            this.gbStrings.Controls.Add(this.dgvPassThroughStrings);
            this.gbStrings.Controls.Add(this.txtAddText);
            this.gbStrings.Controls.Add(this.btnAdd);
            this.gbStrings.Controls.Add(this.label1);
            this.gbStrings.Controls.Add(this.btnDel);
            this.gbStrings.Location = new System.Drawing.Point(17, 46);
            this.gbStrings.Name = "gbStrings";
            this.gbStrings.Size = new System.Drawing.Size(422, 227);
            this.gbStrings.TabIndex = 7;
            this.gbStrings.TabStop = false;
            // 
            // radOneString
            // 
            this.radOneString.AutoSize = true;
            this.radOneString.Location = new System.Drawing.Point(173, 23);
            this.radOneString.Name = "radOneString";
            this.radOneString.Size = new System.Drawing.Size(120, 17);
            this.radOneString.TabIndex = 0;
            this.radOneString.Text = "Look For One String";
            this.radOneString.UseVisualStyleBackColor = true;
            // 
            // radAllString
            // 
            this.radAllString.AutoSize = true;
            this.radAllString.Checked = true;
            this.radAllString.Location = new System.Drawing.Point(17, 23);
            this.radAllString.Name = "radAllString";
            this.radAllString.Size = new System.Drawing.Size(116, 17);
            this.radAllString.TabIndex = 1;
            this.radAllString.TabStop = true;
            this.radAllString.Text = "Look For All Strings";
            this.radAllString.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(266, 373);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 8;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(359, 373);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // chkEnablePassThrough
            // 
            this.chkEnablePassThrough.AutoSize = true;
            this.chkEnablePassThrough.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkEnablePassThrough.Location = new System.Drawing.Point(12, 18);
            this.chkEnablePassThrough.Name = "chkEnablePassThrough";
            this.chkEnablePassThrough.Size = new System.Drawing.Size(193, 17);
            this.chkEnablePassThrough.TabIndex = 10;
            this.chkEnablePassThrough.Text = "Enable Pass Through Function";
            this.chkEnablePassThrough.UseVisualStyleBackColor = true;
            this.chkEnablePassThrough.CheckedChanged += new System.EventHandler(this.chkEnablePassThrough_CheckedChanged);
            // 
            // PassThrough
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(463, 418);
            this.Controls.Add(this.chkEnablePassThrough);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.gbStrings);
            this.Controls.Add(this.gbAllOrOne);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PassThrough";
            this.Text = "Pass Through Strings";
            this.Load += new System.EventHandler(this.PassThrough_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPassThroughStrings)).EndInit();
            this.gbAllOrOne.ResumeLayout(false);
            this.gbAllOrOne.PerformLayout();
            this.gbStrings.ResumeLayout(false);
            this.gbStrings.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvPassThroughStrings;
        private System.Windows.Forms.DataGridViewTextBoxColumn PtString;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnDel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtAddText;
        private System.Windows.Forms.GroupBox gbAllOrOne;
        private System.Windows.Forms.GroupBox gbStrings;
        private System.Windows.Forms.RadioButton radAllString;
        private System.Windows.Forms.RadioButton radOneString;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chkEnablePassThrough;
    }
}