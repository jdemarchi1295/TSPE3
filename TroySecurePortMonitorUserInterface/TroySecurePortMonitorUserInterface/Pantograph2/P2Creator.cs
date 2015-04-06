using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Diagnostics;
using System.IO;
//using System.Threading.Tasks;

namespace TroySecurePortMonitorUserInterface.Pantograph2
{
    public partial class P2Creator : Form
    {
        #region Properties
        public string PrinterName { get; set; }
        public string CustomName { get; set; }
        
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

        int idx = 0;
        int pages = 0;
        #endregion

        public P2Creator()
        {
            InitializeComponent();
            foreground = new PatternDefinition();
            background = new PatternDefinition();
        }

        private void SimpleCreator_Load(object sender, EventArgs e)
        {
            trkMin.Value = 20;
            trkMax.Value = 40;
            UpdateRange();
            lblStatus.Text = "Loaded";
        }

        /// <summary>
        /// Update the range label when the sliders are adjusted
        /// </summary>
        private void UpdateRange()
        {
            lblRange.Text = string.Format("{0}-{1}", trkMin.Value, trkMax.Value);
            pages = 0;
        }

        #region GUI
        private void cmdReset_Click(object sender, EventArgs e)
        {
            pages = 0;
            lblStatus.Text = "Page reset";
        }

        private void trkMin_Scroll(object sender, EventArgs e)
        {
            if (trkMin.Value > trkMax.Value)
                trkMin.Value = trkMax.Value;
            UpdateRange();
        }

        private void trkMax_Scroll(object sender, EventArgs e)
        {
            if (trkMax.Value < trkMin.Value)
                trkMax.Value = trkMin.Value;
            UpdateRange();
        }
        #endregion

        /// <summary>
        /// Take the currently selected patterns and set the return byte arrays
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdSavePatterns_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "Saving...";
            Dot bg = GetDot(txtPageNumberBG.Text, txtCellBG.Text, "BG");
            if (bg != null)
            {
                if (background == null)
                    background = new PatternDefinition();
                background.pattern1.data = bg.GetBytes(true);
                background.pattern1.height = bg.Height;
                background.pattern1.width = bg.Width;
            }

            Dot fg = GetDot(txtPageNumberFG.Text, txtCellFG.Text, "FG");
            if (fg != null)
            {
                if (foreground == null)
                    foreground = new PatternDefinition();
                foreground.pattern1.data = fg.GetBytes(true);
                foreground.pattern1.height = fg.Height;
                foreground.pattern1.width = fg.Width;
            }

            CustomName = txtCustomPtnName.Text;

            if (lblStatus.Text == "Saving...")
                this.Close();
        }

        private Dot GetDot(string page, string cell, string tag)
        {
            int cellIdx = ConvertCellIdx(cell);
            if (cellIdx == -1)
            {
                lblStatus.Text = string.Format("Invalid {0} Cell", tag);
                return null;
            }

            int pageNum = 0;
            if (!int.TryParse(page, out pageNum))
            {
                lblStatus.Text = string.Format("Invalid {0} Page #", tag);
                return null;
            }

            int idx = Constants.CellsPerPage * (pageNum - 1) + cellIdx;

            if (Cartesian == null)
                GenerateCartesian();

            if (Cartesian.Count() <= idx)
            {
                lblStatus.Text = string.Format("Invalid {0} parameters, please retry", tag);
                return null;
            }
            Dot d = Cartesian.ElementAt(idx);
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
            if(string.IsNullOrEmpty(cell))
                return -1;
            cell = cell.ToUpper();
            int idx = 0;
            string alpha = "";
            string numeric = "";
            foreach(char c in cell)
            {
                if(char.IsLetter(c))
                    alpha += c;
                else if(char.IsNumber(c))
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
        private void cmdGenerate_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "Generating...";
            useCartesian = true;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            GenerateCartesian();
            if (Cartesian.Count() == 0)
            {
                if (pages == 0)
                    lblStatus.Text = "Your filtering criteria have returned zero results.  Please adjust and re-try.";
                else
                    lblStatus.Text = "Your filtering criteria has not generated any further pages.  Please adjust and re-try";
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

            pages++;
            lblStatus.Text = "Completed";
        }

        /// <summary>
        /// Generate the collection of cells based on the current filtering criteria
        /// </summary>
        private void GenerateCartesian()
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
            float low = trkMin.Value / 100f;
            float high = trkMax.Value / 100f;

            Cartesian = Cartesian.Where(x => low <= x.RFgridDensity && x.RFgridDensity <= high).Skip(pages * Constants.CellsPerPage).Take(Constants.CellsPerPage);
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
            int.TryParse(txtPageNumberBG.Text, out cellIdx);
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
                        select new Dot() { Height = baseDot.Height, Width = baseDot.Width, tolerance = baseDot.tolerance,
                                           xStepDown = xd1 + baseDot.xStepDown, xStepOver = xo1 + baseDot.xStepOver, 
                                           yStepDown = yd1 + baseDot.yStepDown, yStepOver = yo1 + baseDot.yStepOver,
                                           DrawThick = (xd1 == 0 && xo1 == 0 && yd1 == 0 && yo1 == 0)
                        }).ToList();

            //Parallel.ForEach(Cartesian, x => x.RenderAndFill());
            foreach (Dot d in SubCartesian)
                d.RenderAndFill();

            var bytes = SubCartesian.Select(x => x.GetBytesAsString(true));
            SubCartesian.ElementAt(0).GetBytesAsString(true);
            float low = trkMin.Value / 100f;
            float high = trkMax.Value / 100f;

            //Do not filter
            //subCartesian = subCartesian.Where(x => low <= x.RFgridDensity && x.RFgridDensity <= high).ToList().GetRange(pages * Constants.CellsPerPage, Constants.CellsPerPage);

            Debug.Print(string.Format("Sub Count: {0}, Columns: {1}, Rows: {2}", SubCartesian.Count(), Constants.NumColumns, Constants.NumRows));

            pd.Print();
            pd.Dispose();

            sw.Stop();
            Debug.Print(string.Format("Sub Print took {0} sec.", sw.Elapsed.TotalSeconds));

            pages++;
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

            Dot fg = GetDot(txtPageNumberFG.Text, txtCellFG.Text, "FG");
            if (fg == null)
            {
                stripeBrush = new SolidBrush(Color.Gray);
            }
            else
            {
                int ms = fg.MinSize();
                bool[,] fgGrid = new bool[ms, ms];
                fg.RenderToGrid(fgGrid);
                Bitmap fgb = ConvertToBMP(fgGrid, false);
                stripeBrush = new TextureBrush(fgb);
            }

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

            int yy = Constants.shiftY;
            int rowNum = 1;
            Font f = new Font("Arial", 10f, FontStyle.Regular);
            SolidBrush br = new SolidBrush(Color.Black);
            while (yy < Constants.PageH)
            {
                g.DrawString(rowNum.ToString(), f, br, new PointF(0, yy));
                yy += Constants.cellH;
                rowNum++;
            }
            
            int xx = Constants.shiftX;
            char colNum = 'A';
            while (xx < Constants.PageW)
            {
                g.DrawString(colNum.ToString(), f, br, new PointF(xx, 0));
                xx += Constants.cellW;
                colNum++;
            }

            g.DrawString(string.Format("Page {0}, range {1}-{2}", pages + 1, trkMin.Value, trkMax.Value), f, br, new PointF(50, Constants.PageH - 50));
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
                    tw.Write(grid[r,c] ? "1" : "0");
            tw.Flush();
            tw.Close();
            sw.Stop();
            Debug.Print(string.Format("Convert of {1} took {0} sec.", sw.Elapsed.TotalSeconds, fileName));
        }
    }
}
