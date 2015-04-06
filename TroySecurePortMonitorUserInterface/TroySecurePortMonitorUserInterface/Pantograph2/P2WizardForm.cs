#define Print

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Drawing.Printing;
using System.Threading;
using System.Xml.Serialization;
using PantographPclBuilder;

namespace TroySecurePortMonitorUserInterface.Pantograph2
{
    public partial class P2WizardForm : Form
    {
        public enum P2WizardState
        {
            Start,
            InitialPrint,
            ValidBlend,
            FailReason,
            FGAdjust,
            PrintFullPanto,
            IsValidPanto,
            Complete,
            Advanced,
            Unknown
        }

        public enum Step
        {
            Next,
            Back,
            Yes,
            No,
            Cancel,
            Save
        }

        List<P2WizardState> enableNextWhen = new List<P2WizardState>() { P2WizardState.Start, P2WizardState.InitialPrint, P2WizardState.FailReason, P2WizardState.FGAdjust, P2WizardState.PrintFullPanto, P2WizardState.Advanced };
        List<P2WizardState> enableBackWhen = new List<P2WizardState>() { P2WizardState.PrintFullPanto };
        List<P2WizardState> enableYesNoWhen = new List<P2WizardState>() { P2WizardState.ValidBlend, P2WizardState.IsValidPanto };
        List<P2WizardState> enableCancelWhen = new List<P2WizardState>() { P2WizardState.FGAdjust, P2WizardState.Complete };
        List<P2WizardState> enableSaveWhen = new List<P2WizardState>() { P2WizardState.Complete };

        public P2WizardState state = P2WizardState.Start;
        List<TabPage> pages = new List<TabPage>();
        int _density = 0;
        int Density
        {
            get { return _density; }
            set { _density = value; lblDarkness.Text = string.Format("(hidden in final) Density = {0}", Density); } 
        }

        int _page = 0;
        int Page
        {
            get { return _page; }
            set { _page = value; lblPage.Text = string.Format("(hidden in final) Page # = {0}", Page); }
        }

        #region Properties
        public string PrinterName { get; set; }
        public string CustomName { get; set; }
        public int IntfPattern { get; set; }
        public string IntfFontPath { get; set; }

        /// <summary>
        /// Data for foreground pattern
        /// </summary>
        public PatternDefinition foreground { get; set; }
        /// <summary>
        /// Data for background pattern
        /// </summary>
        public PatternDefinition background { get; set; }

        /// <summary>
        /// Collection of dots based on current filtering critera
        /// </summary>
        IEnumerable<Dot> Cartesian = null;
        /// <summary>
        /// Collection of dots near a selected cell (currently not used)
        /// </summary>
        IEnumerable<Dot> SubCartesian = null;
        /// <summary>
        /// Switch LINQ query between Cartesian and SubCartesian
        /// </summary>
        bool useCartesian = true;

        Dot CurrentFG = null;

        #endregion

        public P2WizardForm()
        {
            InitializeComponent();
            foreground = new PatternDefinition();
            background = new PatternDefinition();
        }

        private void P2Wizard_Load(object sender, EventArgs e)
        {
            tabStart.Tag = P2WizardState.Start;
            tabInitialPrint.Tag = P2WizardState.InitialPrint;
            tabValidBlend.Tag = P2WizardState.ValidBlend;
            tabFailReason.Tag = P2WizardState.FailReason;
            tabFGAdjust.Tag = P2WizardState.FGAdjust;
            tabPrintFullPanto.Tag = P2WizardState.PrintFullPanto;
            tabValidPanto.Tag = P2WizardState.IsValidPanto;
            tabComplete.Tag = P2WizardState.Complete;
            tabAdvanced.Tag = P2WizardState.Advanced;

            foreach (TabPage t in pages)
            {
                if ((P2WizardState)t.Tag == null)
                    t.Tag = P2WizardState.Unknown;
            }

            foreach (TabPage t in tabPages.TabPages)
            {
                pages.Add(t);
            }

            Density = 2;
            Page = 0;
            nudDarknessBG.Maximum = 100 / Constants.DensityRange;

            HighlightCurrentStep();
        }

        #region WizardSteps

        private void cmdNext_Click(object sender, EventArgs e)
        {
            ProcessCurrentStep(Step.Next);
        }

        private void cmdBack_Click(object sender, EventArgs e)
        {
            ProcessCurrentStep(Step.Back);
        }

        private void cmdYes_Click(object sender, EventArgs e)
        {
            ProcessCurrentStep(Step.Yes);
        }

        private void cmdNo_Click(object sender, EventArgs e)
        {
            ProcessCurrentStep(Step.No);
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            ProcessCurrentStep(Step.Cancel);
        }

        private void cmdSave_Click(object sender, EventArgs e)
        {
            ProcessCurrentStep(Step.Save);
        }

        //should be StepChanged or setter on state property
        private void ProcessCurrentStep(Step step)
        {
            switch (state)
            {
                case P2WizardState.Start:
                    ProcessStart(step); break;
                case P2WizardState.InitialPrint:
                    ProcessInitialPrint(step); break;
                case P2WizardState.ValidBlend:
                    ProcessValidBlend(step); break;
                case P2WizardState.FailReason:
                    ProcessFailReason(step); break;
                case P2WizardState.FGAdjust:
                    ProcessFGAdjust(step); break;
                case P2WizardState.PrintFullPanto:
                    ProcessPrintFullPanto(step); break;
                case P2WizardState.IsValidPanto:
                    ProcessIsValidPanto(step); break;
                case P2WizardState.Complete:
                    ProcessComplete(step); return;
                case P2WizardState.Advanced:
                    ProcessAdvanced(step); break;
                default: break;
            }
            HighlightCurrentStep();
        }

        private void ProcessStart(Step step)
        {
            if (step != Step.Next)  //invalid
                return;
            state = P2WizardState.InitialPrint;
        }

        private void ProcessInitialPrint(Step step)
        {
            if (step != Step.Next)  //invalid
                return;
            Cursor.Current = Cursors.WaitCursor;
#if Print
            GenerateCellPage();
#endif
            Cursor.Current = Cursors.Default;
            state = P2WizardState.ValidBlend;
        }

        private void ProcessValidBlend(Step step)
        {
            if (step == Step.Next || step == Step.Back)  //invalid
                return;

            if (step == Step.Yes)
            {
                state = P2WizardState.PrintFullPanto;
                nudDarknessBG.Value = Density;
                nudPageNumberBG.Value = Page + 1;
            }
            if (step == Step.No)
                state = P2WizardState.FailReason;
        }

        private void ProcessFailReason(Step step)
        {
            if (step != Step.Next)  //invalid
                return;

            if (rdNoGoodPattern.Checked)
                Page++;
            if (rdTooDark.Checked)
            {
                if (Density == 0)
                {
                    MessageBox.Show("Cannot go any lighter");
                    return;
                }
                Density--;
                Page = 0;
            }
            if (rdTooLight.Checked)
            {
                if (Density == 10)
                {
                    MessageBox.Show("Cannot go any darker");
                    return;
                }
                Density++;
                Page = 0;
            }
            if (rdStripe.Checked)
            {
                state = P2WizardState.FGAdjust;
                nudDarknessFG.Value = Density;
                nudPageNumberFG.Value = Page + 1;
                ResetDensityChoices();
                return;
            }

            state = P2WizardState.InitialPrint;
            ResetDensityChoices();
        }

        private void ProcessFGAdjust(Step step)
        {
            if (step != Step.Cancel && step != Step.Next)   //invalid
                return;

            if (step == Step.Cancel)
            {
                state = P2WizardState.FailReason;
                return;
            }

            GenerateCartesian((int)nudPageNumberFG.Value, (int)nudDarknessFG.Value);
            Dot d = GetDot((int)nudPageNumberFG.Value, (int)nudDarknessFG.Value, txtCellFG.Text, "FG");
            if (d == null)
            {
                MessageBox.Show("Invalid foreground pattern parameters, please re-enter");
                return;
            }
            CurrentFG = d;
            state = P2WizardState.InitialPrint;
        }

        private void ProcessPrintFullPanto(Step step)
        {
            if (step != Step.Next && step != Step.Back)  //invalid
                return;

            if (step == Step.Back)
            {
                state = P2WizardState.FailReason;
                return;
            }
#if !Print
            state = P2WizardState.IsValidPanto;
            return;
#endif

            Dot bg = GetDot((int)nudPageNumberBG.Value, (int)nudDarknessBG.Value, txtCellBG.Text, "BG");
            if (bg == null)
            {
                MessageBox.Show("You have entered an invalid page number or cell index.  Please correct and try again");
                return;
            }
            if(CurrentFG == null)
                CurrentFG = Cartesian.First();
            byte[] data = bg.GetBytes(true);
            byte[] dataFg = CurrentFG.GetBytes(true);

            //MessageBox.Show("TODO - Print pantograph based on entered cell");
            
            CustomConfiguration cc = new CustomConfiguration();

            //Using interference pattern?
            if (!string.IsNullOrEmpty(IntfFontPath))
            {
                //Set the interference enabled bit and chessboard bit
                cc.PantographConfiguration = "257";
            }
            else
            {
                //Set only the chessboard bit
                cc.PantographConfiguration = "1";
                IntfFontPath = Application.StartupPath;  //TBD Temporary until I fix the panto builder.  Does not like a blank string.
            }
            cc.PageOrientation = PageOrientationType.poPortrait;
            cc.PageType = PageType.ptLetter;
            cc.UseDefaultInclusionForPaperSize = true;
            cc.InterferencePatternId = 20;
            cc.CellList.Clear();

            //Create a cell with the text COPY and id = 10
            PantographCellDescriptorType pcdt = new PantographCellDescriptorType();
            pcdt.msg = "COPY";
            pcdt.pidx = 10;
            cc.CellList.Add(pcdt);

            //Set the custom pattern
            cc.CustomBackgroundPatternData = data;
            //TBD use real foreground
            cc.CustomForegroundPatternData = dataFg;

            //Create the full page pantograph
            string filename = "PantographProfile1Page1.pcl";

            var xser = new XmlSerializer(typeof(CustomConfiguration));
            string cfilename = IntfFontPath + "TroyPantographConfiguration.xml";
            var writer = new StreamWriter(cfilename);
            xser.Serialize(writer, cc);

            var bp = new BuildPantographWrap.Wrapper();
            int rc = bp.ManagedCreatePantograph(IntfFontPath);

            //Print the pantograph file
            PrinterLib.PrintToSpooler.SendFileToPrinter(PrinterName, filename, "Test");

            //TBD Temporary kluge to make the other temporary kluge work properly
            if (cc.PantographConfiguration == "1")
            {
                IntfFontPath = "";
            }

            state = P2WizardState.IsValidPanto;
        }

        private void ProcessIsValidPanto(Step step)
        {
            if (step == Step.Next || step == Step.Back)  //invalid
                return;

            if (step == Step.Yes)
                state = P2WizardState.Complete;
            if (step == Step.No)
                state = P2WizardState.PrintFullPanto;
        }

        private void ProcessComplete(Step step)
        {
            if (step != Step.Cancel && step != Step.Save)  //invalid
                return;

            if (step == Step.Save)
            {
                if (string.IsNullOrEmpty(txtCustomPtnName.Text))
                {
                    MessageBox.Show("You must enter a name for the pattern.");
                    return;
                }
                SavePatterns();
            }
            this.Close();
        }

        private void ProcessAdvanced(Step step)
        {
            state = P2WizardState.Start;
        }

        #endregion

        private void HighlightCurrentStep()
        {
            tabPages.TabPages.Clear();

            foreach (TabPage t in pages)
            {
                if((P2WizardState)t.Tag == state)
                {
                    tabPages.TabPages.Add(t);
                }
            }

            cmdBack.Visible = enableBackWhen.Contains(state);
            cmdNext.Visible = enableNextWhen.Contains(state);
            cmdYes.Visible = enableYesNoWhen.Contains(state);
            cmdNo.Visible = enableYesNoWhen.Contains(state);
            cmdCancel.Visible = enableCancelWhen.Contains(state);
            cmdSave.Visible = enableSaveWhen.Contains(state);

            switch (state)
            {
                case P2WizardState.Start:
                    lblTitle.Text = "Start"; break;
                case P2WizardState.InitialPrint:
                    lblTitle.Text = "Print A Page Of Samples"; break;
                case P2WizardState.ValidBlend:  
                    lblTitle.Text = "Is There A Valid Blend?"; break;
                case P2WizardState.FailReason:
                    lblTitle.Text = "What Is Wrong With The Blend?"; break;
                case P2WizardState.FGAdjust:
                    lblTitle.Text = "Foreground Adjust"; break;
                case P2WizardState.PrintFullPanto:
                    lblTitle.Text = "Select Cell To Print Full Page"; break;
                case P2WizardState.IsValidPanto:
                    lblTitle.Text = "Is Full Printout Valid?"; break;
                case P2WizardState.Complete:
                    lblTitle.Text = "Complete"; break;
                case P2WizardState.Advanced:
                    lblTitle.Text = "Advanced Settings"; break;
                default:
                    lblTitle.Text = "Invalid State"; break;
            }
        }

        private void ResetDensityChoices()
        {
            rdNoGoodPattern.Checked = false;
            rdTooDark.Checked = false;
            rdTooLight.Checked = false;
            rdStripe.Checked = false;
        }

        private void cmdAdvanced_Click(object sender, EventArgs e)
        {
            state = P2WizardState.Advanced;
            HighlightCurrentStep();
        }

        #region CellGeneration

        private int MinDensity(int density)
        {
            return density * Constants.DensityRange;
        }

        private int MaxDensity(int density)
        {
            return (density + 1) * Constants.DensityRange;
        }

        /// <summary>
        /// Take the currently selected patterns and set the return byte arrays
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SavePatterns()
        {
            Dot bg = GetDot((int)nudPageNumberBG.Value, (int)nudDarknessBG.Value, txtCellBG.Text, "BG");
            if (bg != null)
            {
                if (background == null)
                    background = new PatternDefinition();
                background.pattern1.data = bg.GetBytes(true);
                background.pattern1.height = bg.Height;
                background.pattern1.width = bg.Width;
            }

            if (CurrentFG != null)
            {
                if (foreground == null)
                    foreground = new PatternDefinition();
                foreground.pattern1.data = CurrentFG.GetBytes(true);
                foreground.pattern1.height = CurrentFG.Height;
                foreground.pattern1.width = CurrentFG.Width;
            }

            CustomName = txtCustomPtnName.Text;
        }

        private Dot GetDot(int page, int density, string cell, string tag)
        {
            int cellIdx = ConvertCellIdx(cell);
            if (cellIdx == -1)
            {
                //MessageBox.Show(string.Format("Invalid {0} Cell", tag));
                return null;
            }

            //int idx = Constants.CellsPerPage * (page - 1) + cellIdx;

            GenerateCartesian(page, density);

            if (Cartesian.Count() <= cellIdx)
            {
                //MessageBox.Show(string.Format("Invalid {0} parameters, please retry", tag));
                return null;
            }
            Dot d = Cartesian.ElementAt(cellIdx);
            d.RenderAndFill();

            return d;
        }

        /// <summary>
        /// Convert cell designation (A1, C24, etc) to a single int to
        /// index into the Cartesian collection
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private int ConvertCellIdx(string cell)
        {
            if (string.IsNullOrEmpty(cell))
                return -1;
            cell = cell.ToUpper();
            int idx = 0;
            string alpha = "";
            string numeric = "";
            foreach (char c in cell)
            {
                if (char.IsLetter(c))
                    alpha += c;
                else if (char.IsNumber(c))
                    numeric += c;
                else
                    return -1;
            }
            if (alpha.Length != 1)
                return -1;
            int a = (char)alpha[0] - 'A';
            int n = 0;
            if (!int.TryParse(numeric, out n))
                return -1;
            n--;

            return a + n * Constants.NumColumns;
        }

        /// <summary>
        /// Set the printer defaults and generate a page of cells
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenerateCellPage()
        {
            useCartesian = true;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            GenerateCartesian(Page, Density);
            if (Cartesian.Count() == 0)
            {
                //if (pages == 0)
                //    lblStatus.Text = "Your filtering criteria have returned zero results.  Please adjust and re-try.";
                //else
                //    lblStatus.Text = "Your filtering criteria has not generated any further pages.  Please adjust and re-try";
                return;
            }

            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(pd_PrintPage);

            pd.PrinterSettings.DefaultPageSettings.PrinterResolution.X = Constants.DPI;
            pd.PrinterSettings.DefaultPageSettings.PrinterResolution.Y = Constants.DPI;
            pd.PrinterSettings.PrinterName = PrinterName;

            Debug.Print(string.Format("Count: {0}, Columns: {1}, Rows: {2}", Cartesian.Count(), Constants.NumColumns, Constants.NumRows));

            pd.Print();
            pd.Dispose();

            sw.Stop();
            Debug.Print(string.Format("Print took {0} sec.", sw.Elapsed.TotalSeconds));

            //pages++;
        }

        /// <summary>
        /// Generate the collection of cells based on the current filtering criteria
        /// </summary>
        private void GenerateCartesian(int page, int density)
        {
            Cartesian = (from sz in Constants.DotSizes
                         from xd1 in Constants.OverDown
                         from xo1 in Constants.OverDown
                         from yd1 in Constants.OverDown
                         from yo1 in Constants.OverDown
                         select new Dot() { Height = sz, Width = sz, tolerance = .2f, xStepDown = xd1, xStepOver = xo1, yStepDown = yd1, yStepOver = yo1 }).ToList();

            //Parallel.ForEach(Cartesian, x => x.RenderAndFill());
            foreach (Dot d in Cartesian)
                d.RenderAndFill();

            var bytes = Cartesian.Select(x => x.GetBytesAsString(true));
            Cartesian.ElementAt(0).GetBytesAsString(true);
            float low = MinDensity(density) / 100f;
            float high = MaxDensity(density) / 100f;

            Cartesian = Cartesian.Where(x => low <= x.RFgridDensity && x.RFgridDensity <= high)/*.OrderBy(x => x.RFgridDensity)*/.Skip(Page * Constants.CellsPerPage).Take(Constants.CellsPerPage);
            //.GetRange(pages * Constants.CellsPerPage, Constants.CellsPerPage);
        }

        /// <summary>
        /// (not currently used)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdGenSubRange_Click(object sender, EventArgs e)
        {
            useCartesian = false;
            //TODO - get dot here
            int cellIdx = 0;
            //int.TryParse((int)nudPageNumberBG.Value, out cellIdx);
            //Dot baseDot = new Dot() { Height = 3, Width = 3, tolerance = .2f, xStepDown = };
            Dot baseDot = Cartesian.ElementAt(cellIdx);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(pd_PrintPage);

            pd.PrinterSettings.DefaultPageSettings.PrinterResolution.X = 600;
            pd.PrinterSettings.DefaultPageSettings.PrinterResolution.Y = 600;
            pd.PrinterSettings.PrinterName = PrinterName;

            SubCartesian = (from xd1 in Constants.OverDownSub
                            from xo1 in Constants.OverDownSub
                            from yd1 in Constants.OverDownSub
                            from yo1 in Constants.OverDownSub
                            select new Dot()
                            {
                                Height = baseDot.Height,
                                Width = baseDot.Width,
                                tolerance = baseDot.tolerance,
                                xStepDown = xd1 + baseDot.xStepDown,
                                xStepOver = xo1 + baseDot.xStepOver,
                                yStepDown = yd1 + baseDot.yStepDown,
                                yStepOver = yo1 + baseDot.yStepOver,
                                DrawThick = (xd1 == 0 && xo1 == 0 && yd1 == 0 && yo1 == 0)
                            }).ToList();

            //Parallel.ForEach(Cartesian, x => x.RenderAndFill());
            foreach (Dot d in SubCartesian)
                d.RenderAndFill();

            var bytes = SubCartesian.Select(x => x.GetBytesAsString(true));
            SubCartesian.ElementAt(0).GetBytesAsString(true);
            float low = MinDensity(Density) / 100f;
            float high = MaxDensity(Density) / 100f;

            //Do not filter
            //subCartesian = subCartesian.Where(x => low <= x.RFgridDensity && x.RFgridDensity <= high).ToList().GetRange(pages * Constants.CellsPerPage, Constants.CellsPerPage);

            Debug.Print(string.Format("Sub Count: {0}, Columns: {1}, Rows: {2}", SubCartesian.Count(), Constants.NumColumns, Constants.NumRows));

            pd.Print();
            pd.Dispose();

            sw.Stop();
            Debug.Print(string.Format("Sub Print took {0} sec.", sw.Elapsed.TotalSeconds));

            //pages++;
        }

        /// <summary>
        /// Render the list of cells to a page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            g.PageUnit = GraphicsUnit.Pixel;

            List<int> lc = Enumerable.Range(0, Constants.NumColumns).ToList();
            List<int> lr = Enumerable.Range(0, Constants.NumRows).ToList();

            Brush stripeBrush = null;

            if (CurrentFG == null)
                CurrentFG = Cartesian.First();

            //Dot fg = GetDot(txtPageNumberFG.Text, txtCellFG.Text, "FG");
            //if (fg == null)
            //{
            //    stripeBrush = new SolidBrush(Color.Gray);
            //}
            //else
            //{
            int ms = CurrentFG.MinSize();
            bool[,] fgGrid = new bool[ms, ms];
            CurrentFG.RenderToGrid(fgGrid);
            Bitmap fgb = ConvertToBMP(fgGrid, false);
            stripeBrush = new TextureBrush(fgb);
            //}

            var results = lr./*AsParallel().*/Select(
                r => lc./*AsParallel().*/Select(
                    c => (r * Constants.NumColumns + c >= (useCartesian ? Cartesian.Count() : SubCartesian.Count()) ?
                        new Bitmap(Constants.cellW, Constants.cellH) :
                        DrawCell((useCartesian ? Cartesian.ElementAt(r * Constants.NumColumns + c) :
                            SubCartesian.ElementAt(r * Constants.NumColumns + c)), stripeBrush, r * Constants.NumColumns + c))).ToList()).ToList();

            var results2 = results./*AsParallel().*/Select(x => StitchBMPsHoriz(x)).ToList();

            //var results3 = StitchBMPsVert(results2);
            int row = Constants.shiftY;
            int pg = 0;
            foreach (Bitmap b in results2)
            {
                b.SetResolution(Constants.DPI, Constants.DPI);
                g.DrawImage(b, new Rectangle(Constants.shiftX, row, b.Width, b.Height),
                                new Rectangle(0, 0, b.Width, b.Height), GraphicsUnit.Pixel);
                //g.DrawImageUnscaled(b, 0, row);
                row += b.Height;
                //if (row == 2)
                //    break;
                //WriteOutBMP("out" + pg.ToString(), b);
                pg++;
            }

            //int yy = Constants.shiftY;
            //int rowNum = 1;
            Font f = new Font("Arial", 10f, FontStyle.Regular);
            SolidBrush br = new SolidBrush(Color.Black);
            for (int y = 0; y < Constants.NumRows; y++)
            {
                g.DrawString((y + 1).ToString(), f, br, new PointF(0, y * Constants.cellH + Constants.shiftY));
            }

            //int xx = Constants.shiftX;
            char colNum = 'A';
            for (int x = 0; x < Constants.NumColumns; x++)
            {
                g.DrawString(colNum.ToString(), f, br, new PointF(x * Constants.cellW + Constants.shiftX, 0));
                colNum++;
            }
            //while (xx < Constants.PageW)
            //{
            //    g.DrawString(colNum.ToString(), f, br, new PointF(xx, 0));
            //    xx += Constants.cellW;
            //    colNum++;
            //}

            g.DrawString(string.Format("Page {0}, Darkness {1}", Page + 1, Density), f, br, new PointF(50, Constants.PageH - 50));
            e.HasMorePages = false;
        }

        /// <summary>
        /// Generate a bitmap based on a dot
        /// </summary>
        /// <param name="d"></param>
        /// <param name="idx"></param>
        /// <returns></returns>
        Bitmap DrawCell(Dot d, Brush stripeBrush, int idx = -1)
        {
            int curX = 0;
            int curY = 0;

            Bitmap res = new Bitmap(Constants.cellW, Constants.cellH);
            Graphics g = Graphics.FromImage(res);

            Pen p = new Pen(new SolidBrush(Color.Black), 2);

            d.FillCircle();
            int ms = d.MinSize();

            bool[,] dot1Grid = new bool[ms, ms];
            //render dots based on offset settings
            d.RenderToGrid(dot1Grid);

            //Debug.Print(string.Format("Density of {0} is {1}", idx, GetDensity(dot1Grid)));

            Bitmap d1b = ConvertToBMP(dot1Grid);
            //d1b.Save("d1b.bmp");

            //if (idx != -1 && idx < 5)
            //{
            //    WriteOutBMP(idx.ToString() + "b", d1b);
            //    WriteOutGrid(idx.ToString() + "g", dot1Grid);
            //}

            TextureBrush tb1 = new TextureBrush(d1b);
            Rectangle r = new Rectangle(curX, curY, Constants.cellW - Constants.padding, Constants.cellH - Constants.padding);
            g.FillRectangle(tb1, r);

            List<Point> pts = new List<Point>() {   // coordinates for diagonal stripe, assumes cell w < h
                new Point(curX, curY + Constants.offY + Constants.cellW - Constants.offL), 
                new Point(curX, curY + Constants.offY + Constants.cellW), 
                new Point(curX + Constants.offL, curY + Constants.offY + Constants.cellW), 
                new Point(curX + Constants.cellW - Constants.padding, curY + Constants.offY + Constants.offL), 
                new Point(curX + Constants.cellW - Constants.padding, curY + Constants.offY), 
                new Point(curX + Constants.cellW - Constants.offL, curY + Constants.offY) 
            };

            g.FillPolygon(stripeBrush, pts.ToArray());

            if (d.DrawThick)
                p = new Pen(new SolidBrush(Color.Black), 8);

            g.DrawRectangle(p, r);

            //if (idx != -1)
            //    res.Save(idx.ToString() + ".bmp");
            return res;
        }

        /// <summary>
        /// Convert a boolean grid to a bitmap
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        private Bitmap ConvertToBMP(bool[,] grid, bool drawTransparent = true)
        {
            Bitmap b = new Bitmap(grid.GetLength(0), grid.GetLength(1));
            for (int r = 0; r < grid.GetLength(0); r++)
            {
                for (int c = 0; c < grid.GetLength(1); c++)
                {
                    if (grid[r, c])
                    {
                        b.SetPixel(c, r, Color.Black);
                    }
                    else
                    {
                        b.SetPixel(c, r, (drawTransparent ? Color.Transparent : Color.White));
                    }
                }
            }
            return b;
        }

        /// <summary>
        /// Take a list of cell BMPs and combine them into a horizontal strip of cells
        /// </summary>
        /// <param name="bmps"></param>
        /// <returns></returns>
        private Bitmap StitchBMPsHoriz(List<Bitmap> bmps)
        {
            Bitmap res = new Bitmap(bmps.Count() * Constants.cellW, Constants.cellH);
            Graphics g = Graphics.FromImage(res);

            for (int i = 0; i < bmps.Count(); i++)
            {
                g.DrawImage(bmps[i], new Rectangle(i * Constants.cellW, 0, Constants.cellW, Constants.cellH),
                                new Rectangle(0, 0, Constants.cellW, Constants.cellH), GraphicsUnit.Pixel);
            }
            //if (idx != -1)
            //    res.Save((pages++).ToString() + ".bmp");
            return res;
        }

        //all BMPs should be same width, possible diff heights
        /// <summary>
        /// Take a list of horizontal strip BMPs and combine them vertically
        /// </summary>
        /// <param name="bmps"></param>
        /// <param name="idx"></param>
        /// <returns></returns>
        private List<Bitmap> StitchBMPsVert(List<Bitmap> bmps, int idx = -1)
        {
            List<Bitmap> res = new List<Bitmap>();

            int i = 0;
            while (i < bmps.Count)
            {
                if (i + 1 == bmps.Count)
                {
                    res.Add(bmps[i]);
                    break;
                }

                Bitmap b = new Bitmap(bmps[i].Width, bmps[i].Height + bmps[i + 1].Height);
                Graphics g = Graphics.FromImage(b);

                g.DrawImage(bmps[i], new Rectangle(0, 0, bmps[i].Width, bmps[i].Height),
                new Rectangle(0, 0, bmps[i].Width, bmps[i].Height), GraphicsUnit.Pixel);

                g.DrawImage(bmps[i + 1], new Rectangle(0, bmps[i].Height, bmps[i + 1].Width, bmps[i + 1].Height),
                new Rectangle(0, 0, bmps[i + 1].Width, bmps[i + 1].Height), GraphicsUnit.Pixel);

                res.Add(b);

                //if(idx != -1)
                //    b.Save((pages++).ToString() + ".bmp");
                i += 2;
            }
            return res;
        }

        /// <summary>
        /// Write out a bitmap as a series of 0's and 1's to test how long it would
        /// take to generate a PCL file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="b"></param>
        private void WriteOutBMP(string fileName, Bitmap b)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            TextWriter tw = new StreamWriter(string.Format("{0}.txt", fileName));
            for (int r = 0; r < b.Height; r++)
                for (int c = 0; c < b.Width; c++)
                    tw.Write((b.GetPixel(c, r) == Color.White) ? "1" : "0");
            tw.Flush();
            tw.Close();
            sw.Stop();
            Debug.Print(string.Format("Convert of {1} took {0} sec.", sw.Elapsed.TotalSeconds, fileName));
        }

        /// <summary>
        /// Write out a boolean array as a series of 0's and 1's to test how long it would
        /// take to generate a PCL file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="grid"></param>
        private void WriteOutGrid(string fileName, bool[,] grid)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            TextWriter tw = new StreamWriter(string.Format("{0}.txt", fileName));
            for (int r = 0; r < grid.GetLength(0); r++)
                for (int c = 0; c < grid.GetLength(1); c++)
                    tw.Write(grid[r, c] ? "1" : "0");
            tw.Flush();
            tw.Close();
            sw.Stop();
            Debug.Print(string.Format("Convert of {1} took {0} sec.", sw.Elapsed.TotalSeconds, fileName));
        }

        #endregion

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
