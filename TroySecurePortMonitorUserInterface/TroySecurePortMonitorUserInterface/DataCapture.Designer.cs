namespace TroySecurePortMonitorUserInterface
{
    partial class DataCapture
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataCapture));
            this.chkRemoveData = new System.Windows.Forms.CheckBox();
            this.gbTags = new System.Windows.Forms.GroupBox();
            this.gbTagList = new System.Windows.Forms.GroupBox();
            this.dgvDataCap = new System.Windows.Forms.DataGridView();
            this.IndexId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LeadingTag = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TrailingTag = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IncludeLeading = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LeadingText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TrailingText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OnePerPage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnAddTag = new System.Windows.Forms.Button();
            this.btnRemoveTag = new System.Windows.Forms.Button();
            this.radUseTags = new System.Windows.Forms.RadioButton();
            this.radUseAllData = new System.Windows.Forms.RadioButton();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.radPjlHeader = new System.Windows.Forms.RadioButton();
            this.gbFonts = new System.Windows.Forms.GroupBox();
            this.lstFonts = new System.Windows.Forms.ListBox();
            this.btnAddFont = new System.Windows.Forms.Button();
            this.btnDelFont = new System.Windows.Forms.Button();
            this.radFont = new System.Windows.Forms.RadioButton();
            this.radTroyFont = new System.Windows.Forms.RadioButton();
            this.radPlainText = new System.Windows.Forms.RadioButton();
            this.gbRemoveString = new System.Windows.Forms.GroupBox();
            this.dgvRemoveList = new System.Windows.Forms.DataGridView();
            this.StringtoRemove = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radMpText = new System.Windows.Forms.RadioButton();
            this.radPassThrough = new System.Windows.Forms.RadioButton();
            this.radUseWithTroyMark = new System.Windows.Forms.RadioButton();
            this.radUseForPrinterName = new System.Windows.Forms.RadioButton();
            this.gbFontNames = new System.Windows.Forms.GroupBox();
            this.gbTags.SuspendLayout();
            this.gbTagList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDataCap)).BeginInit();
            this.gbFonts.SuspendLayout();
            this.gbRemoveString.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRemoveList)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.gbFontNames.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkRemoveData
            // 
            this.chkRemoveData.AutoSize = true;
            this.chkRemoveData.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkRemoveData.Location = new System.Drawing.Point(22, 503);
            this.chkRemoveData.Name = "chkRemoveData";
            this.chkRemoveData.Size = new System.Drawing.Size(425, 20);
            this.chkRemoveData.TabIndex = 2;
            this.chkRemoveData.Text = "Remove the Captured Data from Print Stream Before Printing";
            this.chkRemoveData.UseVisualStyleBackColor = true;
            // 
            // gbTags
            // 
            this.gbTags.Controls.Add(this.gbTagList);
            this.gbTags.Controls.Add(this.radUseTags);
            this.gbTags.Controls.Add(this.radUseAllData);
            this.gbTags.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbTags.Location = new System.Drawing.Point(22, 243);
            this.gbTags.Name = "gbTags";
            this.gbTags.Size = new System.Drawing.Size(739, 250);
            this.gbTags.TabIndex = 5;
            this.gbTags.TabStop = false;
            this.gbTags.Text = "How will the data be captured...";
            // 
            // gbTagList
            // 
            this.gbTagList.Controls.Add(this.dgvDataCap);
            this.gbTagList.Controls.Add(this.btnAddTag);
            this.gbTagList.Controls.Add(this.btnRemoveTag);
            this.gbTagList.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbTagList.Location = new System.Drawing.Point(252, 12);
            this.gbTagList.Name = "gbTagList";
            this.gbTagList.Size = new System.Drawing.Size(466, 225);
            this.gbTagList.TabIndex = 6;
            this.gbTagList.TabStop = false;
            // 
            // dgvDataCap
            // 
            this.dgvDataCap.AllowUserToAddRows = false;
            this.dgvDataCap.AllowUserToDeleteRows = false;
            this.dgvDataCap.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dgvDataCap.BackgroundColor = System.Drawing.Color.Gainsboro;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvDataCap.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvDataCap.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDataCap.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IndexId,
            this.LeadingTag,
            this.TrailingTag,
            this.IncludeLeading,
            this.LeadingText,
            this.TrailingText,
            this.OnePerPage});
            this.dgvDataCap.Location = new System.Drawing.Point(24, 21);
            this.dgvDataCap.MultiSelect = false;
            this.dgvDataCap.Name = "dgvDataCap";
            this.dgvDataCap.ReadOnly = true;
            this.dgvDataCap.RowHeadersWidth = 25;
            this.dgvDataCap.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvDataCap.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDataCap.Size = new System.Drawing.Size(427, 170);
            this.dgvDataCap.TabIndex = 3;
            // 
            // IndexId
            // 
            this.IndexId.HeaderText = "Index";
            this.IndexId.Name = "IndexId";
            this.IndexId.ReadOnly = true;
            this.IndexId.Visible = false;
            this.IndexId.Width = 90;
            // 
            // LeadingTag
            // 
            this.LeadingTag.HeaderText = "Leading Tag";
            this.LeadingTag.Name = "LeadingTag";
            this.LeadingTag.ReadOnly = true;
            this.LeadingTag.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.LeadingTag.Width = 95;
            // 
            // TrailingTag
            // 
            this.TrailingTag.HeaderText = "Trailing Tag";
            this.TrailingTag.Name = "TrailingTag";
            this.TrailingTag.ReadOnly = true;
            this.TrailingTag.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.TrailingTag.Width = 94;
            // 
            // IncludeLeading
            // 
            this.IncludeLeading.HeaderText = "Include Leading";
            this.IncludeLeading.Name = "IncludeLeading";
            this.IncludeLeading.ReadOnly = true;
            this.IncludeLeading.Width = 112;
            // 
            // LeadingText
            // 
            this.LeadingText.HeaderText = "Leading Text";
            this.LeadingText.Name = "LeadingText";
            this.LeadingText.ReadOnly = true;
            this.LeadingText.Width = 97;
            // 
            // TrailingText
            // 
            this.TrailingText.HeaderText = "Trailing Text";
            this.TrailingText.Name = "TrailingText";
            this.TrailingText.ReadOnly = true;
            this.TrailingText.Width = 96;
            // 
            // OnePerPage
            // 
            this.OnePerPage.HeaderText = "One Per Page";
            this.OnePerPage.Name = "OnePerPage";
            this.OnePerPage.ReadOnly = true;
            this.OnePerPage.Visible = false;
            // 
            // btnAddTag
            // 
            this.btnAddTag.Location = new System.Drawing.Point(33, 197);
            this.btnAddTag.Name = "btnAddTag";
            this.btnAddTag.Size = new System.Drawing.Size(57, 23);
            this.btnAddTag.TabIndex = 2;
            this.btnAddTag.Text = "Add";
            this.btnAddTag.UseVisualStyleBackColor = true;
            this.btnAddTag.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemoveTag
            // 
            this.btnRemoveTag.Location = new System.Drawing.Point(106, 197);
            this.btnRemoveTag.Name = "btnRemoveTag";
            this.btnRemoveTag.Size = new System.Drawing.Size(57, 23);
            this.btnRemoveTag.TabIndex = 1;
            this.btnRemoveTag.Text = "Del";
            this.btnRemoveTag.UseVisualStyleBackColor = true;
            this.btnRemoveTag.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // radUseTags
            // 
            this.radUseTags.AutoSize = true;
            this.radUseTags.Checked = true;
            this.radUseTags.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radUseTags.Location = new System.Drawing.Point(150, 21);
            this.radUseTags.Name = "radUseTags";
            this.radUseTags.Size = new System.Drawing.Size(79, 20);
            this.radUseTags.TabIndex = 5;
            this.radUseTags.TabStop = true;
            this.radUseTags.Text = "Use Tags";
            this.radUseTags.UseVisualStyleBackColor = true;
            this.radUseTags.CheckedChanged += new System.EventHandler(this.radUseTags_CheckedChanged);
            // 
            // radUseAllData
            // 
            this.radUseAllData.AutoSize = true;
            this.radUseAllData.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radUseAllData.Location = new System.Drawing.Point(17, 21);
            this.radUseAllData.Name = "radUseAllData";
            this.radUseAllData.Size = new System.Drawing.Size(95, 20);
            this.radUseAllData.TabIndex = 4;
            this.radUseAllData.TabStop = true;
            this.radUseAllData.Text = "Use All Data";
            this.radUseAllData.UseVisualStyleBackColor = true;
            this.radUseAllData.CheckedChanged += new System.EventHandler(this.radUseAllData_CheckedChanged);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(574, 539);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 6;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(686, 539);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // radPjlHeader
            // 
            this.radPjlHeader.AutoSize = true;
            this.radPjlHeader.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radPjlHeader.Location = new System.Drawing.Point(132, 32);
            this.radPjlHeader.Name = "radPjlHeader";
            this.radPjlHeader.Size = new System.Drawing.Size(159, 20);
            this.radPjlHeader.TabIndex = 11;
            this.radPjlHeader.Text = "PJL Header Information";
            this.radPjlHeader.UseVisualStyleBackColor = true;
            this.radPjlHeader.CheckedChanged += new System.EventHandler(this.radPjlHeader_CheckedChanged);
            // 
            // gbFonts
            // 
            this.gbFonts.Controls.Add(this.lstFonts);
            this.gbFonts.Controls.Add(this.btnAddFont);
            this.gbFonts.Controls.Add(this.btnDelFont);
            this.gbFonts.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbFonts.Location = new System.Drawing.Point(540, 80);
            this.gbFonts.Name = "gbFonts";
            this.gbFonts.Size = new System.Drawing.Size(221, 161);
            this.gbFonts.TabIndex = 10;
            this.gbFonts.TabStop = false;
            this.gbFonts.Text = "Fonts to use with Standard Fonts...";
            // 
            // lstFonts
            // 
            this.lstFonts.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstFonts.FormattingEnabled = true;
            this.lstFonts.Location = new System.Drawing.Point(18, 20);
            this.lstFonts.Name = "lstFonts";
            this.lstFonts.Size = new System.Drawing.Size(180, 82);
            this.lstFonts.TabIndex = 0;
            // 
            // btnAddFont
            // 
            this.btnAddFont.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddFont.Location = new System.Drawing.Point(18, 122);
            this.btnAddFont.Name = "btnAddFont";
            this.btnAddFont.Size = new System.Drawing.Size(57, 23);
            this.btnAddFont.TabIndex = 3;
            this.btnAddFont.Text = "Add";
            this.btnAddFont.UseVisualStyleBackColor = true;
            this.btnAddFont.Click += new System.EventHandler(this.btnAddFont_Click);
            // 
            // btnDelFont
            // 
            this.btnDelFont.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelFont.Location = new System.Drawing.Point(81, 122);
            this.btnDelFont.Name = "btnDelFont";
            this.btnDelFont.Size = new System.Drawing.Size(57, 23);
            this.btnDelFont.TabIndex = 4;
            this.btnDelFont.Text = "Del";
            this.btnDelFont.UseVisualStyleBackColor = true;
            this.btnDelFont.Click += new System.EventHandler(this.btnDelFont_Click);
            // 
            // radFont
            // 
            this.radFont.AutoSize = true;
            this.radFont.Checked = true;
            this.radFont.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radFont.Location = new System.Drawing.Point(330, 32);
            this.radFont.Name = "radFont";
            this.radFont.Size = new System.Drawing.Size(113, 20);
            this.radFont.TabIndex = 9;
            this.radFont.TabStop = true;
            this.radFont.Text = "Standard Fonts";
            this.radFont.UseVisualStyleBackColor = true;
            this.radFont.CheckedChanged += new System.EventHandler(this.radFont_CheckedChanged);
            // 
            // radTroyFont
            // 
            this.radTroyFont.AutoSize = true;
            this.radTroyFont.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radTroyFont.Location = new System.Drawing.Point(253, 538);
            this.radTroyFont.Name = "radTroyFont";
            this.radTroyFont.Size = new System.Drawing.Size(111, 20);
            this.radTroyFont.TabIndex = 8;
            this.radTroyFont.Text = "Troy Font Data";
            this.radTroyFont.UseVisualStyleBackColor = true;
            this.radTroyFont.Visible = false;
            this.radTroyFont.CheckedChanged += new System.EventHandler(this.radTroyFont_CheckedChanged);
            // 
            // radPlainText
            // 
            this.radPlainText.AutoSize = true;
            this.radPlainText.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radPlainText.Location = new System.Drawing.Point(11, 32);
            this.radPlainText.Name = "radPlainText";
            this.radPlainText.Size = new System.Drawing.Size(82, 20);
            this.radPlainText.TabIndex = 7;
            this.radPlainText.Text = "Plain Text";
            this.radPlainText.UseVisualStyleBackColor = true;
            this.radPlainText.CheckedChanged += new System.EventHandler(this.radPlainText_CheckedChanged);
            // 
            // gbRemoveString
            // 
            this.gbRemoveString.Controls.Add(this.dgvRemoveList);
            this.gbRemoveString.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbRemoveString.Location = new System.Drawing.Point(12, 530);
            this.gbRemoveString.Name = "gbRemoveString";
            this.gbRemoveString.Size = new System.Drawing.Size(229, 63);
            this.gbRemoveString.TabIndex = 10;
            this.gbRemoveString.TabStop = false;
            this.gbRemoveString.Text = "Remove any extra data included in the captured data...";
            this.gbRemoveString.Visible = false;
            // 
            // dgvRemoveList
            // 
            this.dgvRemoveList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRemoveList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.StringtoRemove});
            this.dgvRemoveList.Location = new System.Drawing.Point(13, 22);
            this.dgvRemoveList.Name = "dgvRemoveList";
            this.dgvRemoveList.Size = new System.Drawing.Size(189, 58);
            this.dgvRemoveList.TabIndex = 1;
            // 
            // StringtoRemove
            // 
            this.StringtoRemove.HeaderText = "String To Remove";
            this.StringtoRemove.Name = "StringtoRemove";
            this.StringtoRemove.Width = 400;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radMpText);
            this.groupBox1.Controls.Add(this.radPassThrough);
            this.groupBox1.Controls.Add(this.radUseWithTroyMark);
            this.groupBox1.Controls.Add(this.radUseForPrinterName);
            this.groupBox1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(22, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(669, 67);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Captured data will be used for which function...";
            // 
            // radMpText
            // 
            this.radMpText.AutoSize = true;
            this.radMpText.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radMpText.Location = new System.Drawing.Point(461, 29);
            this.radMpText.Name = "radMpText";
            this.radMpText.Size = new System.Drawing.Size(112, 20);
            this.radMpText.TabIndex = 3;
            this.radMpText.TabStop = true;
            this.radMpText.Text = "MicroPrint Text";
            this.radMpText.UseVisualStyleBackColor = true;
            this.radMpText.CheckedChanged += new System.EventHandler(this.radMpText_CheckedChanged);
            // 
            // radPassThrough
            // 
            this.radPassThrough.AutoSize = true;
            this.radPassThrough.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radPassThrough.Location = new System.Drawing.Point(330, 29);
            this.radPassThrough.Name = "radPassThrough";
            this.radPassThrough.Size = new System.Drawing.Size(104, 20);
            this.radPassThrough.TabIndex = 2;
            this.radPassThrough.TabStop = true;
            this.radPassThrough.Text = "Pass Through";
            this.radPassThrough.UseVisualStyleBackColor = true;
            this.radPassThrough.CheckedChanged += new System.EventHandler(this.radPassThrough_CheckedChanged);
            // 
            // radUseWithTroyMark
            // 
            this.radUseWithTroyMark.AutoSize = true;
            this.radUseWithTroyMark.Checked = true;
            this.radUseWithTroyMark.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radUseWithTroyMark.Location = new System.Drawing.Point(17, 29);
            this.radUseWithTroyMark.Name = "radUseWithTroyMark";
            this.radUseWithTroyMark.Size = new System.Drawing.Size(117, 20);
            this.radUseWithTroyMark.TabIndex = 1;
            this.radUseWithTroyMark.TabStop = true;
            this.radUseWithTroyMark.Text = "TROYmark Data";
            this.radUseWithTroyMark.UseVisualStyleBackColor = true;
            this.radUseWithTroyMark.CheckedChanged += new System.EventHandler(this.radUseWithTroyMark_CheckedChanged);
            // 
            // radUseForPrinterName
            // 
            this.radUseForPrinterName.AutoSize = true;
            this.radUseForPrinterName.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radUseForPrinterName.Location = new System.Drawing.Point(186, 29);
            this.radUseForPrinterName.Name = "radUseForPrinterName";
            this.radUseForPrinterName.Size = new System.Drawing.Size(92, 20);
            this.radUseForPrinterName.TabIndex = 0;
            this.radUseForPrinterName.TabStop = true;
            this.radUseForPrinterName.Text = "Printer Map";
            this.radUseForPrinterName.UseVisualStyleBackColor = true;
            this.radUseForPrinterName.CheckedChanged += new System.EventHandler(this.radUseForPrinterName_CheckedChanged);
            // 
            // gbFontNames
            // 
            this.gbFontNames.Controls.Add(this.radPlainText);
            this.gbFontNames.Controls.Add(this.radFont);
            this.gbFontNames.Controls.Add(this.radPjlHeader);
            this.gbFontNames.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbFontNames.Location = new System.Drawing.Point(22, 121);
            this.gbFontNames.Name = "gbFontNames";
            this.gbFontNames.Size = new System.Drawing.Size(490, 73);
            this.gbFontNames.TabIndex = 16;
            this.gbFontNames.TabStop = false;
            this.gbFontNames.Text = "The format of the data in the print job...";
            // 
            // DataCapture
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(786, 572);
            this.Controls.Add(this.gbFontNames);
            this.Controls.Add(this.radTroyFont);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gbFonts);
            this.Controls.Add(this.gbRemoveString);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.gbTags);
            this.Controls.Add(this.chkRemoveData);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DataCapture";
            this.Text = "Data Capture Setup";
            this.Load += new System.EventHandler(this.DataCapture_Load);
            this.gbTags.ResumeLayout(false);
            this.gbTags.PerformLayout();
            this.gbTagList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDataCap)).EndInit();
            this.gbFonts.ResumeLayout(false);
            this.gbRemoveString.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRemoveList)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.gbFontNames.ResumeLayout(false);
            this.gbFontNames.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkRemoveData;
        private System.Windows.Forms.GroupBox gbTags;
        private System.Windows.Forms.Button btnAddTag;
        private System.Windows.Forms.Button btnRemoveTag;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnDelFont;
        private System.Windows.Forms.Button btnAddFont;
        private System.Windows.Forms.ListBox lstFonts;
        private System.Windows.Forms.GroupBox gbRemoveString;
        private System.Windows.Forms.DataGridView dgvDataCap;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radUseForPrinterName;
        private System.Windows.Forms.RadioButton radPassThrough;
        private System.Windows.Forms.RadioButton radUseWithTroyMark;
        private System.Windows.Forms.RadioButton radUseTags;
        private System.Windows.Forms.RadioButton radUseAllData;
        private System.Windows.Forms.GroupBox gbFonts;
        private System.Windows.Forms.RadioButton radFont;
        private System.Windows.Forms.RadioButton radTroyFont;
        private System.Windows.Forms.RadioButton radPlainText;
        private System.Windows.Forms.RadioButton radPjlHeader;
        private System.Windows.Forms.GroupBox gbFontNames;
        private System.Windows.Forms.GroupBox gbTagList;
        private System.Windows.Forms.DataGridView dgvRemoveList;
        private System.Windows.Forms.DataGridViewTextBoxColumn StringtoRemove;
        private System.Windows.Forms.DataGridViewTextBoxColumn IndexId;
        private System.Windows.Forms.DataGridViewTextBoxColumn LeadingTag;
        private System.Windows.Forms.DataGridViewTextBoxColumn TrailingTag;
        private System.Windows.Forms.DataGridViewTextBoxColumn IncludeLeading;
        private System.Windows.Forms.DataGridViewTextBoxColumn LeadingText;
        private System.Windows.Forms.DataGridViewTextBoxColumn TrailingText;
        private System.Windows.Forms.DataGridViewTextBoxColumn OnePerPage;
        private System.Windows.Forms.RadioButton radMpText;
    }
}