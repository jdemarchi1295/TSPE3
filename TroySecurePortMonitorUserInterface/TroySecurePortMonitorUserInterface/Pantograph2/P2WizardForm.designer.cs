namespace TroySecurePortMonitorUserInterface.Pantograph2
{
    partial class P2WizardForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(P2WizardForm));
            this.flwRight = new System.Windows.Forms.FlowLayoutPanel();
            this.cmdNext = new System.Windows.Forms.Button();
            this.cmdYes = new System.Windows.Forms.Button();
            this.cmdSave = new System.Windows.Forms.Button();
            this.cmdBack = new System.Windows.Forms.Button();
            this.pnlTitle = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.flwLeft = new System.Windows.Forms.FlowLayoutPanel();
            this.cmdNo = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.tabPages = new TroySecurePortMonitorUserInterface.Pantograph2.CustomTab();
            this.tabStart = new System.Windows.Forms.TabPage();
            this.cmdAdvanced = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tabInitialPrint = new System.Windows.Forms.TabPage();
            this.lblPage = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblDarkness = new System.Windows.Forms.Label();
            this.tabValidBlend = new System.Windows.Forms.TabPage();
            this.label7 = new System.Windows.Forms.Label();
            this.tabFailReason = new System.Windows.Forms.TabPage();
            this.label8 = new System.Windows.Forms.Label();
            this.grpDensity = new System.Windows.Forms.GroupBox();
            this.rdStripe = new System.Windows.Forms.RadioButton();
            this.rdTooLight = new System.Windows.Forms.RadioButton();
            this.rdTooDark = new System.Windows.Forms.RadioButton();
            this.rdNoGoodPattern = new System.Windows.Forms.RadioButton();
            this.tabFGAdjust = new System.Windows.Forms.TabPage();
            this.nudPageNumberFG = new System.Windows.Forms.NumericUpDown();
            this.nudDarknessFG = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.txtCellFG = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.tabPrintFullPanto = new System.Windows.Forms.TabPage();
            this.nudPageNumberBG = new System.Windows.Forms.NumericUpDown();
            this.nudDarknessBG = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.lblCell = new System.Windows.Forms.Label();
            this.txtCellBG = new System.Windows.Forms.TextBox();
            this.lblPageNum = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tabValidPanto = new System.Windows.Forms.TabPage();
            this.label9 = new System.Windows.Forms.Label();
            this.tabComplete = new System.Windows.Forms.TabPage();
            this.label4 = new System.Windows.Forms.Label();
            this.txtCustomPtnName = new System.Windows.Forms.TextBox();
            this.tabAdvanced = new System.Windows.Forms.TabPage();
            this.label6 = new System.Windows.Forms.Label();
            this.flwRight.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.panel1.SuspendLayout();
            this.flwLeft.SuspendLayout();
            this.tabPages.SuspendLayout();
            this.tabStart.SuspendLayout();
            this.tabInitialPrint.SuspendLayout();
            this.tabValidBlend.SuspendLayout();
            this.tabFailReason.SuspendLayout();
            this.grpDensity.SuspendLayout();
            this.tabFGAdjust.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPageNumberFG)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDarknessFG)).BeginInit();
            this.tabPrintFullPanto.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPageNumberBG)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDarknessBG)).BeginInit();
            this.tabValidPanto.SuspendLayout();
            this.tabComplete.SuspendLayout();
            this.tabAdvanced.SuspendLayout();
            this.SuspendLayout();
            // 
            // flwRight
            // 
            this.flwRight.Controls.Add(this.cmdNext);
            this.flwRight.Controls.Add(this.cmdYes);
            this.flwRight.Controls.Add(this.cmdSave);
            this.flwRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.flwRight.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flwRight.Location = new System.Drawing.Point(355, 0);
            this.flwRight.Name = "flwRight";
            this.flwRight.Size = new System.Drawing.Size(109, 42);
            this.flwRight.TabIndex = 0;
            // 
            // cmdNext
            // 
            this.cmdNext.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdNext.Location = new System.Drawing.Point(21, 3);
            this.cmdNext.Name = "cmdNext";
            this.cmdNext.Size = new System.Drawing.Size(85, 35);
            this.cmdNext.TabIndex = 0;
            this.cmdNext.Text = "Next";
            this.cmdNext.UseVisualStyleBackColor = true;
            this.cmdNext.Click += new System.EventHandler(this.cmdNext_Click);
            // 
            // cmdYes
            // 
            this.cmdYes.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdYes.Location = new System.Drawing.Point(21, 44);
            this.cmdYes.Name = "cmdYes";
            this.cmdYes.Size = new System.Drawing.Size(85, 35);
            this.cmdYes.TabIndex = 1;
            this.cmdYes.Text = "Yes";
            this.cmdYes.UseVisualStyleBackColor = true;
            this.cmdYes.Click += new System.EventHandler(this.cmdYes_Click);
            // 
            // cmdSave
            // 
            this.cmdSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdSave.Location = new System.Drawing.Point(21, 85);
            this.cmdSave.Name = "cmdSave";
            this.cmdSave.Size = new System.Drawing.Size(85, 35);
            this.cmdSave.TabIndex = 2;
            this.cmdSave.Text = "Save";
            this.cmdSave.UseVisualStyleBackColor = true;
            this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
            // 
            // cmdBack
            // 
            this.cmdBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdBack.Location = new System.Drawing.Point(3, 3);
            this.cmdBack.Name = "cmdBack";
            this.cmdBack.Size = new System.Drawing.Size(85, 35);
            this.cmdBack.TabIndex = 1;
            this.cmdBack.Text = "Back";
            this.cmdBack.UseVisualStyleBackColor = true;
            this.cmdBack.Click += new System.EventHandler(this.cmdBack_Click);
            // 
            // pnlTitle
            // 
            this.pnlTitle.BackColor = System.Drawing.Color.Transparent;
            this.pnlTitle.Controls.Add(this.lblTitle);
            this.pnlTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTitle.Location = new System.Drawing.Point(0, 0);
            this.pnlTitle.Name = "pnlTitle";
            this.pnlTitle.Size = new System.Drawing.Size(464, 25);
            this.pnlTitle.TabIndex = 2;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(5, 5);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(86, 20);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Step Title";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.flwLeft);
            this.panel1.Controls.Add(this.flwRight);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 252);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(464, 42);
            this.panel1.TabIndex = 3;
            // 
            // flwLeft
            // 
            this.flwLeft.Controls.Add(this.cmdBack);
            this.flwLeft.Controls.Add(this.cmdNo);
            this.flwLeft.Controls.Add(this.cmdCancel);
            this.flwLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.flwLeft.Location = new System.Drawing.Point(0, 0);
            this.flwLeft.Name = "flwLeft";
            this.flwLeft.Size = new System.Drawing.Size(108, 42);
            this.flwLeft.TabIndex = 2;
            // 
            // cmdNo
            // 
            this.cmdNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdNo.Location = new System.Drawing.Point(3, 44);
            this.cmdNo.Name = "cmdNo";
            this.cmdNo.Size = new System.Drawing.Size(85, 35);
            this.cmdNo.TabIndex = 2;
            this.cmdNo.Text = "No";
            this.cmdNo.UseVisualStyleBackColor = true;
            this.cmdNo.Click += new System.EventHandler(this.cmdNo_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdCancel.Location = new System.Drawing.Point(3, 85);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(85, 35);
            this.cmdCancel.TabIndex = 3;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // tabPages
            // 
            this.tabPages.Controls.Add(this.tabStart);
            this.tabPages.Controls.Add(this.tabInitialPrint);
            this.tabPages.Controls.Add(this.tabValidBlend);
            this.tabPages.Controls.Add(this.tabFailReason);
            this.tabPages.Controls.Add(this.tabFGAdjust);
            this.tabPages.Controls.Add(this.tabPrintFullPanto);
            this.tabPages.Controls.Add(this.tabValidPanto);
            this.tabPages.Controls.Add(this.tabComplete);
            this.tabPages.Controls.Add(this.tabAdvanced);
            this.tabPages.Location = new System.Drawing.Point(3, 35);
            this.tabPages.Name = "tabPages";
            this.tabPages.SelectedIndex = 0;
            this.tabPages.Size = new System.Drawing.Size(458, 211);
            this.tabPages.TabIndex = 1;
            // 
            // tabStart
            // 
            this.tabStart.Controls.Add(this.cmdAdvanced);
            this.tabStart.Controls.Add(this.label1);
            this.tabStart.Location = new System.Drawing.Point(4, 22);
            this.tabStart.Name = "tabStart";
            this.tabStart.Padding = new System.Windows.Forms.Padding(3);
            this.tabStart.Size = new System.Drawing.Size(450, 185);
            this.tabStart.TabIndex = 0;
            this.tabStart.Text = "Start";
            this.tabStart.UseVisualStyleBackColor = true;
            // 
            // cmdAdvanced
            // 
            this.cmdAdvanced.Location = new System.Drawing.Point(322, 147);
            this.cmdAdvanced.Name = "cmdAdvanced";
            this.cmdAdvanced.Size = new System.Drawing.Size(108, 23);
            this.cmdAdvanced.TabIndex = 2;
            this.cmdAdvanced.Text = "Advanced Settings";
            this.cmdAdvanced.UseVisualStyleBackColor = true;
            this.cmdAdvanced.Visible = false;
            this.cmdAdvanced.Click += new System.EventHandler(this.cmdAdvanced_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(20, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(410, 124);
            this.label1.TabIndex = 1;
            this.label1.Text = resources.GetString("label1.Text");
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // tabInitialPrint
            // 
            this.tabInitialPrint.Controls.Add(this.lblPage);
            this.tabInitialPrint.Controls.Add(this.label2);
            this.tabInitialPrint.Controls.Add(this.lblDarkness);
            this.tabInitialPrint.Location = new System.Drawing.Point(4, 22);
            this.tabInitialPrint.Name = "tabInitialPrint";
            this.tabInitialPrint.Padding = new System.Windows.Forms.Padding(3);
            this.tabInitialPrint.Size = new System.Drawing.Size(450, 185);
            this.tabInitialPrint.TabIndex = 1;
            this.tabInitialPrint.Text = "Initial Print";
            this.tabInitialPrint.UseVisualStyleBackColor = true;
            // 
            // lblPage
            // 
            this.lblPage.AutoSize = true;
            this.lblPage.Location = new System.Drawing.Point(308, 159);
            this.lblPage.Name = "lblPage";
            this.lblPage.Size = new System.Drawing.Size(35, 13);
            this.lblPage.TabIndex = 8;
            this.lblPage.Text = "label1";
            this.lblPage.Visible = false;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(20, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(410, 111);
            this.label2.TabIndex = 7;
            this.label2.Text = "Click Next to generate a page of samples.  This may take a minute or two.\r\n\r\nAfte" +
    "r the page is printed, run a copy of the printed page to be used in the next ste" +
    "p.";
            // 
            // lblDarkness
            // 
            this.lblDarkness.AutoSize = true;
            this.lblDarkness.Location = new System.Drawing.Point(308, 146);
            this.lblDarkness.Name = "lblDarkness";
            this.lblDarkness.Size = new System.Drawing.Size(35, 13);
            this.lblDarkness.TabIndex = 2;
            this.lblDarkness.Text = "label1";
            this.lblDarkness.Visible = false;
            // 
            // tabValidBlend
            // 
            this.tabValidBlend.Controls.Add(this.label7);
            this.tabValidBlend.Location = new System.Drawing.Point(4, 22);
            this.tabValidBlend.Name = "tabValidBlend";
            this.tabValidBlend.Size = new System.Drawing.Size(450, 185);
            this.tabValidBlend.TabIndex = 8;
            this.tabValidBlend.Text = "Valid Blend";
            this.tabValidBlend.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(20, 20);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(410, 69);
            this.label7.TabIndex = 8;
            this.label7.Text = "Are there any cells on this page that blend well on the original and show the str" +
    "ipe on the copy?";
            // 
            // tabFailReason
            // 
            this.tabFailReason.Controls.Add(this.label8);
            this.tabFailReason.Controls.Add(this.grpDensity);
            this.tabFailReason.Location = new System.Drawing.Point(4, 22);
            this.tabFailReason.Name = "tabFailReason";
            this.tabFailReason.Size = new System.Drawing.Size(450, 185);
            this.tabFailReason.TabIndex = 9;
            this.tabFailReason.Text = "Fail Reason";
            this.tabFailReason.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(20, 20);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(410, 45);
            this.label8.TabIndex = 8;
            this.label8.Text = "Please select the problem with the print and click Next.";
            // 
            // grpDensity
            // 
            this.grpDensity.Controls.Add(this.rdStripe);
            this.grpDensity.Controls.Add(this.rdTooLight);
            this.grpDensity.Controls.Add(this.rdTooDark);
            this.grpDensity.Controls.Add(this.rdNoGoodPattern);
            this.grpDensity.Location = new System.Drawing.Point(23, 68);
            this.grpDensity.Name = "grpDensity";
            this.grpDensity.Size = new System.Drawing.Size(200, 110);
            this.grpDensity.TabIndex = 6;
            this.grpDensity.TabStop = false;
            this.grpDensity.Text = "Result";
            // 
            // rdStripe
            // 
            this.rdStripe.AutoSize = true;
            this.rdStripe.Location = new System.Drawing.Point(15, 88);
            this.rdStripe.Name = "rdStripe";
            this.rdStripe.Size = new System.Drawing.Size(138, 17);
            this.rdStripe.TabIndex = 9;
            this.rdStripe.TabStop = true;
            this.rdStripe.Text = "Stripe is too dark or light";
            this.rdStripe.UseVisualStyleBackColor = true;
            // 
            // rdTooLight
            // 
            this.rdTooLight.AutoSize = true;
            this.rdTooLight.Location = new System.Drawing.Point(15, 65);
            this.rdTooLight.Name = "rdTooLight";
            this.rdTooLight.Size = new System.Drawing.Size(164, 17);
            this.rdTooLight.TabIndex = 2;
            this.rdTooLight.TabStop = true;
            this.rdTooLight.Text = "All cells or copies are too light";
            this.rdTooLight.UseVisualStyleBackColor = true;
            // 
            // rdTooDark
            // 
            this.rdTooDark.AutoSize = true;
            this.rdTooDark.Location = new System.Drawing.Point(15, 42);
            this.rdTooDark.Name = "rdTooDark";
            this.rdTooDark.Size = new System.Drawing.Size(166, 17);
            this.rdTooDark.TabIndex = 1;
            this.rdTooDark.TabStop = true;
            this.rdTooDark.Text = "All cells or copies are too dark";
            this.rdTooDark.UseVisualStyleBackColor = true;
            // 
            // rdNoGoodPattern
            // 
            this.rdNoGoodPattern.AutoSize = true;
            this.rdNoGoodPattern.Location = new System.Drawing.Point(15, 19);
            this.rdNoGoodPattern.Name = "rdNoGoodPattern";
            this.rdNoGoodPattern.Size = new System.Drawing.Size(142, 17);
            this.rdNoGoodPattern.TabIndex = 0;
            this.rdNoGoodPattern.TabStop = true;
            this.rdNoGoodPattern.Text = "Cannot find good pattern";
            this.rdNoGoodPattern.UseVisualStyleBackColor = true;
            // 
            // tabFGAdjust
            // 
            this.tabFGAdjust.Controls.Add(this.nudPageNumberFG);
            this.tabFGAdjust.Controls.Add(this.nudDarknessFG);
            this.tabFGAdjust.Controls.Add(this.label10);
            this.tabFGAdjust.Controls.Add(this.label11);
            this.tabFGAdjust.Controls.Add(this.txtCellFG);
            this.tabFGAdjust.Controls.Add(this.label12);
            this.tabFGAdjust.Controls.Add(this.label13);
            this.tabFGAdjust.Location = new System.Drawing.Point(4, 22);
            this.tabFGAdjust.Name = "tabFGAdjust";
            this.tabFGAdjust.Size = new System.Drawing.Size(450, 185);
            this.tabFGAdjust.TabIndex = 11;
            this.tabFGAdjust.Text = "Foreground Adjust";
            this.tabFGAdjust.UseVisualStyleBackColor = true;
            // 
            // nudPageNumberFG
            // 
            this.nudPageNumberFG.Location = new System.Drawing.Point(52, 133);
            this.nudPageNumberFG.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nudPageNumberFG.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudPageNumberFG.Name = "nudPageNumberFG";
            this.nudPageNumberFG.Size = new System.Drawing.Size(50, 20);
            this.nudPageNumberFG.TabIndex = 18;
            this.nudPageNumberFG.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // nudDarknessFG
            // 
            this.nudDarknessFG.Location = new System.Drawing.Point(123, 133);
            this.nudDarknessFG.Name = "nudDarknessFG";
            this.nudDarknessFG.Size = new System.Drawing.Size(50, 20);
            this.nudDarknessFG.TabIndex = 17;
            this.nudDarknessFG.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(120, 104);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(52, 13);
            this.label10.TabIndex = 16;
            this.label10.Text = "Darkness";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(191, 104);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(24, 13);
            this.label11.TabIndex = 15;
            this.label11.Text = "Cell";
            // 
            // txtCellFG
            // 
            this.txtCellFG.Location = new System.Drawing.Point(194, 133);
            this.txtCellFG.Name = "txtCellFG";
            this.txtCellFG.Size = new System.Drawing.Size(50, 20);
            this.txtCellFG.TabIndex = 14;
            this.txtCellFG.Text = "A1";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(49, 104);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(32, 13);
            this.label12.TabIndex = 13;
            this.label12.Text = "Page";
            // 
            // label13
            // 
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(20, 20);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(410, 71);
            this.label13.TabIndex = 12;
            this.label13.Text = "Ignore the stripe on the page.  Select a pattern where the copy gets darker than " +
    "the original.  Enter the appropriate page, darkness and cell values to use that " +
    "pattern as the new stripe pattern.";
            // 
            // tabPrintFullPanto
            // 
            this.tabPrintFullPanto.Controls.Add(this.nudPageNumberBG);
            this.tabPrintFullPanto.Controls.Add(this.nudDarknessBG);
            this.tabPrintFullPanto.Controls.Add(this.label5);
            this.tabPrintFullPanto.Controls.Add(this.lblCell);
            this.tabPrintFullPanto.Controls.Add(this.txtCellBG);
            this.tabPrintFullPanto.Controls.Add(this.lblPageNum);
            this.tabPrintFullPanto.Controls.Add(this.label3);
            this.tabPrintFullPanto.Location = new System.Drawing.Point(4, 22);
            this.tabPrintFullPanto.Name = "tabPrintFullPanto";
            this.tabPrintFullPanto.Padding = new System.Windows.Forms.Padding(3);
            this.tabPrintFullPanto.Size = new System.Drawing.Size(450, 185);
            this.tabPrintFullPanto.TabIndex = 3;
            this.tabPrintFullPanto.Text = "Print Full Panto";
            this.tabPrintFullPanto.UseVisualStyleBackColor = true;
            // 
            // nudPageNumberBG
            // 
            this.nudPageNumberBG.Location = new System.Drawing.Point(52, 133);
            this.nudPageNumberBG.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nudPageNumberBG.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudPageNumberBG.Name = "nudPageNumberBG";
            this.nudPageNumberBG.Size = new System.Drawing.Size(50, 20);
            this.nudPageNumberBG.TabIndex = 11;
            this.nudPageNumberBG.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // nudDarknessBG
            // 
            this.nudDarknessBG.Location = new System.Drawing.Point(123, 133);
            this.nudDarknessBG.Name = "nudDarknessBG";
            this.nudDarknessBG.Size = new System.Drawing.Size(50, 20);
            this.nudDarknessBG.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(120, 104);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Darkness";
            // 
            // lblCell
            // 
            this.lblCell.AutoSize = true;
            this.lblCell.Location = new System.Drawing.Point(191, 104);
            this.lblCell.Name = "lblCell";
            this.lblCell.Size = new System.Drawing.Size(24, 13);
            this.lblCell.TabIndex = 8;
            this.lblCell.Text = "Cell";
            // 
            // txtCellBG
            // 
            this.txtCellBG.Location = new System.Drawing.Point(194, 133);
            this.txtCellBG.Name = "txtCellBG";
            this.txtCellBG.Size = new System.Drawing.Size(50, 20);
            this.txtCellBG.TabIndex = 7;
            this.txtCellBG.Text = "A1";
            // 
            // lblPageNum
            // 
            this.lblPageNum.AutoSize = true;
            this.lblPageNum.Location = new System.Drawing.Point(49, 104);
            this.lblPageNum.Name = "lblPageNum";
            this.lblPageNum.Size = new System.Drawing.Size(32, 13);
            this.lblPageNum.TabIndex = 5;
            this.lblPageNum.Text = "Page";
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(20, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(410, 71);
            this.label3.TabIndex = 4;
            this.label3.Text = "Select a cell to print a full page, single pattern pantograph with.\r\n\r\nClick Next" +
    " to print the page using your current foreground pattern and the entered backgro" +
    "und pattern.";
            // 
            // tabValidPanto
            // 
            this.tabValidPanto.Controls.Add(this.label9);
            this.tabValidPanto.Location = new System.Drawing.Point(4, 22);
            this.tabValidPanto.Name = "tabValidPanto";
            this.tabValidPanto.Size = new System.Drawing.Size(450, 185);
            this.tabValidPanto.TabIndex = 10;
            this.tabValidPanto.Text = "Valid Panto";
            this.tabValidPanto.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(20, 20);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(410, 48);
            this.label9.TabIndex = 5;
            this.label9.Text = "Are you satisfied with the full page printout?  If No, you will return to the pre" +
    "vious screen to select another cell.";
            // 
            // tabComplete
            // 
            this.tabComplete.Controls.Add(this.label4);
            this.tabComplete.Controls.Add(this.txtCustomPtnName);
            this.tabComplete.Location = new System.Drawing.Point(4, 22);
            this.tabComplete.Name = "tabComplete";
            this.tabComplete.Size = new System.Drawing.Size(450, 185);
            this.tabComplete.TabIndex = 5;
            this.tabComplete.Text = "Complete";
            this.tabComplete.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(20, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(410, 56);
            this.label4.TabIndex = 5;
            this.label4.Text = "Please enter a name for the configuration (typically the printer or destination) " +
    "and click Save to save and exit.  Click Cancel to exit without saving.";
            // 
            // txtCustomPtnName
            // 
            this.txtCustomPtnName.Location = new System.Drawing.Point(54, 91);
            this.txtCustomPtnName.Name = "txtCustomPtnName";
            this.txtCustomPtnName.Size = new System.Drawing.Size(264, 20);
            this.txtCustomPtnName.TabIndex = 4;
            // 
            // tabAdvanced
            // 
            this.tabAdvanced.Controls.Add(this.label6);
            this.tabAdvanced.Location = new System.Drawing.Point(4, 22);
            this.tabAdvanced.Name = "tabAdvanced";
            this.tabAdvanced.Padding = new System.Windows.Forms.Padding(3);
            this.tabAdvanced.Size = new System.Drawing.Size(450, 185);
            this.tabAdvanced.TabIndex = 7;
            this.tabAdvanced.Text = "Advanced Settings";
            this.tabAdvanced.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(20, 20);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(410, 41);
            this.label6.TabIndex = 2;
            this.label6.Text = "Advanced settings will eventually go here.";
            // 
            // P2WizardForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 294);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pnlTitle);
            this.Controls.Add(this.tabPages);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "P2WizardForm";
            this.Text = "Pantograph Studio";
            this.Load += new System.EventHandler(this.P2Wizard_Load);
            this.flwRight.ResumeLayout(false);
            this.pnlTitle.ResumeLayout(false);
            this.pnlTitle.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.flwLeft.ResumeLayout(false);
            this.tabPages.ResumeLayout(false);
            this.tabStart.ResumeLayout(false);
            this.tabInitialPrint.ResumeLayout(false);
            this.tabInitialPrint.PerformLayout();
            this.tabValidBlend.ResumeLayout(false);
            this.tabFailReason.ResumeLayout(false);
            this.grpDensity.ResumeLayout(false);
            this.grpDensity.PerformLayout();
            this.tabFGAdjust.ResumeLayout(false);
            this.tabFGAdjust.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPageNumberFG)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDarknessFG)).EndInit();
            this.tabPrintFullPanto.ResumeLayout(false);
            this.tabPrintFullPanto.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPageNumberBG)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDarknessBG)).EndInit();
            this.tabValidPanto.ResumeLayout(false);
            this.tabComplete.ResumeLayout(false);
            this.tabComplete.PerformLayout();
            this.tabAdvanced.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flwRight;
        private System.Windows.Forms.Button cmdNext;
        private System.Windows.Forms.Button cmdBack;
        private CustomTab tabPages;
        private System.Windows.Forms.TabPage tabStart;
        private System.Windows.Forms.TabPage tabInitialPrint;
        private System.Windows.Forms.Panel pnlTitle;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblDarkness;
        private System.Windows.Forms.TabPage tabPrintFullPanto;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.FlowLayoutPanel flwLeft;
        private System.Windows.Forms.TabPage tabComplete;
        private System.Windows.Forms.TextBox txtCustomPtnName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblCell;
        private System.Windows.Forms.TextBox txtCellBG;
        private System.Windows.Forms.Label lblPageNum;
        private System.Windows.Forms.Button cmdYes;
        private System.Windows.Forms.Button cmdNo;
        private System.Windows.Forms.TabPage tabAdvanced;
        private System.Windows.Forms.TabPage tabValidBlend;
        private System.Windows.Forms.TabPage tabFailReason;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TabPage tabValidPanto;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox grpDensity;
        private System.Windows.Forms.RadioButton rdTooLight;
        private System.Windows.Forms.RadioButton rdTooDark;
        private System.Windows.Forms.RadioButton rdNoGoodPattern;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button cmdAdvanced;
        private System.Windows.Forms.RadioButton rdStripe;
        private System.Windows.Forms.Label lblPage;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdSave;
        private System.Windows.Forms.NumericUpDown nudDarknessBG;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown nudPageNumberBG;
        private System.Windows.Forms.TabPage tabFGAdjust;
        private System.Windows.Forms.NumericUpDown nudPageNumberFG;
        private System.Windows.Forms.NumericUpDown nudDarknessFG;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtCellFG;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
    }
}

