using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TroySecurePortMonitorUserInterface.Pantograph2
{
    /// <summary>
    /// Class to generate a repeating pattern of dots with adjustable offsets in
    /// X and Y directions, sizes and tolerances of dots, and rendering methods
    /// </summary>
    public class Dot
    {
        #region Properties
        private int _width;
        /// <summary>
        /// Width of dot, currently always same as height, but would allow for elliptical dots
        /// </summary>
        public int Width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
                ResizeGrid();
            }
        }

        private int _height;
        /// <summary>
        /// Height of dot, currently always same as width, but would allow for elliptical dots
        /// </summary>
        public int Height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
                ResizeGrid();
            }
        }

        /// <summary>
        /// Grid that the dot (circle, ellipse) is rendered to
        /// </summary>
        public bool[,] dotGrid = null;
        /// <summary>
        /// Rendered/Filled grid
        /// </summary>
        public bool[,] RFgrid = null;
        /// <summary>
        /// Computed density of the RFgrid
        /// </summary>
        public float RFgridDensity = 0;

        /// <summary>
        /// Tolerance when computing the radius of the circle, makes the
        /// dot rounder or sharper
        /// </summary>
        public float tolerance { get; set; }

        /// <summary>
        /// Dot grid adjustment parameter
        /// </summary>
        public int xStepOver { get; set; }
        /// <summary>
        /// Dot grid adjustment parameter
        /// </summary>
        public int xStepDown { get; set; }
        /// <summary>
        /// Dot grid adjustment parameter
        /// </summary>
        public int yStepOver { get; set; }
        /// <summary>
        /// Dot grid adjustment parameter
        /// </summary>
        public int yStepDown { get; set; }

        /// <summary>
        /// Whether the dot is enabled, used for 2-dot composite patterns
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Flag for the DrawCell method to highlight the selected cell for the
        /// 2nd pass tweaking (not currently used)
        /// </summary>
        public bool DrawThick { get; set; }
        #endregion

        public Dot()
        {
            Width = 4;
            Height = 4;
            tolerance = .2f;
            xStepOver = 10;
            xStepDown = 10;
            yStepOver = 10;
            yStepDown = 10;
            Enabled = true;
            DrawThick = false;
        }

        /// <summary>
        /// Set grid array based on size parameters
        /// </summary>
        private void ResizeGrid()
        {
            dotGrid = new bool[Height, Width];
        }

        /// <summary>
        /// Determine the minimum size of the grid
        /// </summary>
        /// <returns></returns>
        public int MinSize()
        {
            int sz = Math.Abs(NZ(xStepOver) * NZ(yStepDown) - NZ(xStepDown) * NZ(yStepOver));
            if (sz <= Height)
                sz = Math.Max(Math.Max(xStepOver, xStepDown), Math.Max(yStepOver, yStepDown)); //1;
            return sz;
        }

        /// <summary>
        /// Return not zero (minimum one)
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private int NZ(int i)
        {
            return (i == 0 ? 1 : i);
        }

        /// <summary>
        /// Draw the dot to the grid
        /// </summary>
        /// <param name="target"></param>
        public void RenderToGrid(bool[,] target)
        {
            if (!Enabled)
                return;
            if (dotGrid.GetLength(0) > target.GetLength(0))
                return;
            if (dotGrid.GetLength(1) > target.GetLength(1))
                return;
            //int max = -1 * 4 * MinSize();
            //if (max > -8)
            //    max = -8;
            int row = (xStepDown == 0 ? 0 : -1 * yStepDown);
            while (row * yStepDown < target.GetLength(0) * 4)
            //while (row < xStepOver)
            {
                //int col = (yStepOver == 0 ? 0 : -1 * (NZ(xStepOver) * NZ(yStepDown)));
                int col = (yStepOver == 0 ? 0 : -1 * xStepOver);
                while (col * xStepOver < target.GetLength(1) * 4)
                {
                    RenderAt(target, dotGrid, row * yStepDown + col * xStepDown, col * xStepOver + row * yStepOver);
                    if (xStepOver == 0)
                        break;

                    col++;
                }
                if (yStepDown == 0)
                    break;

                row++;
            }
        }

        /// <summary>
        /// Draw the target to the grid at the specified location
        /// </summary>
        /// <param name="target"></param>
        /// <param name="source"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private void RenderAt(bool[,] target, bool[,] source, int row, int col)
        {
            if (!Enabled)
                return;
            int maxRow = target.GetLength(0);
            int maxCol = target.GetLength(1);
            for (int r = 0; r < source.GetLength(0); r++)
            {
                for (int c = 0; c < source.GetLength(1); c++)
                {
                    if (r + row < maxRow && c + col < maxCol)
                        if (r + row >= 0 && c + col >= 0)
                            if (source[r, c])
                                target[r + row, c + col] = source[r, c];
                }
            }
        }

        /// <summary>
        /// Clear the pattern grid
        /// </summary>
        private void ClearPattern()
        {
            if (dotGrid.GetLength(0) == 0)
                return;
            if (dotGrid.GetLength(1) == 0)
                return;

            for (int r = 0; r < dotGrid.GetLength(0); r++)
            {
                for (int c = 0; c < dotGrid.GetLength(1); c++)
                {
                    dotGrid[r, c] = false;
                }
            }
        }

        /// <summary>
        /// Render the circle based on the tolerance
        /// </summary>
        public void FillCircle()
        {
            if (dotGrid.GetLength(0) != dotGrid.GetLength(1))
                return;
            ClearPattern();
            float radius = dotGrid.GetLength(0) / 2.0f;
            int rad = dotGrid.GetLength(0) / 2;
            float radPlus = radius + radius * tolerance;
            float adjust = 0.0f;

            if (dotGrid.GetLength(0) % 2 == 0)
                adjust = .5f;

            for (int r = 0; r < dotGrid.GetLength(0); r++)
            {
                for (int c = 0; c < dotGrid.GetLength(1); c++)
                {
                    float rr = r - rad + adjust;
                    float cc = c - rad + adjust;
                    bool all4 = true;
                    for (int r2 = -1; r2 <= 1; r2 += 2)
                    {
                        for (int c2 = -1; c2 <= 1; c2 += 2)
                        {
                            float pyth = (float)(Math.Pow(rr + r2 * .5f, 2) + Math.Pow(cc + c2 * .5f, 2));
                            if ((float)Math.Sqrt(pyth) > radPlus)
                                all4 = false;
                        }
                    }
                    if (all4)
                        dotGrid[r, c] = true;
                }
            }
        }

        /// <summary>
        /// Render the dot to the grid in such a way that tiling the pattern
        /// will result is a smoothly repeating bitmap
        /// </summary>
        public void RenderAndFill()
        {
            FillCircle();
            int ms = MinSize();
            RFgrid = new bool[ms, ms];
            RenderToGrid(RFgrid);
            RFgridDensity = GetDensity(RFgrid);
        }

        /// <summary>
        /// Return the density (0.0-1.0) of the grid
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        float GetDensity(bool[,] grid)
        {
            int totalPx = grid.GetLength(0) * grid.GetLength(1);
            int totalSet = 0;

            for (int r = 0; r < grid.GetLength(0); r++)
                for (int c = 0; c < grid.GetLength(1); c++)
                    if (grid[r, c])
                        totalSet++;

            return (float)totalSet / (float)totalPx;
        }

        /// <summary>
        /// Convert the grid to a byte array
        /// </summary>
        /// <param name="includeMeta">Include the bytes that specify the PCL attributes of the rasterized images</param>
        /// <returns></returns>
        public byte[] GetBytes(bool includeMeta = false)
        {
            RenderAndFill();
            bool[,] grid8 = MultplyGrid(RFgrid);
            return GetBytes(grid8, includeMeta);
        }
        
        /// <summary>
        /// Convert the grid to a byte array
        /// </summary>
        /// <param name="includeMeta">Include the bytes that specify the PCL attributes of the rasterized images</param>
        /// <returns></returns>
        public byte[] GetBytes(bool[,] grid, bool includeMeta = false)
        {
            RenderAndFill();
            int numPx = grid.GetLength(0) * grid.GetLength(1);
            numPx = numPx / 8 + (numPx % 8 == 0 ? 0 : 1);

            byte[] res = new byte[numPx + (includeMeta ? 12 : 0)];
            int idx = 0;
            int bIdx = 0;
            byte tmp = 0;

            if (includeMeta)
            {
                res[0] = 0x14;     //format
                res[1] = 0;     //continuation
                res[2] = 1;     //pixel encoding
                res[3] = 0;     //Reserved
                res[4] = (byte)(grid.GetLength(0) / 256);     //height
                res[5] = (byte)(grid.GetLength(0) % 256);     //height
                res[6] = (byte)(grid.GetLength(1) / 256);     //width
                res[7] = (byte)(grid.GetLength(1) % 256);     //width
                res[8] = (byte)(600 / 256);     //x-resolution
                res[9] = (byte)(600 % 256);     //x-resolution
                res[10] = (byte)(600 / 256);     //y-resolution
                res[11] = (byte)(600 % 256);     //y-resolution

                idx = 12;
            }

            for (int r = 0; r < grid.GetLength(0); r++)
            {
                for (int c = 0; c < grid.GetLength(1); c++)
                {
                    tmp = (byte)((tmp << 1) + (grid[r, c] ? 1 : 0));  //inverted, used to be 1:0 due to pantograph creator outputs
                    bIdx++;
                    if (bIdx == 8)
                    {
                        res[idx] = tmp;
                        tmp = 0;
                        idx++;
                        bIdx = 0;
                    }
                }
            }
            if (bIdx != 8 && bIdx != 0) //clean up residual bytes
            {
                while (bIdx != 8)
                {
                    bIdx++;
                    tmp = (byte)(tmp << 1);
                }
                res[idx] = tmp;
                tmp = 0;
                idx++;
                bIdx = 0;
            }
            return res;
        }

        public bool[,] MultplyGrid(bool[,] grid)
        {
            int multiplier = 1;
            if (grid.GetLength(0) != grid.GetLength(1))
                return null;
            while ((multiplier * grid.GetLength(0)) % 8 != 0)
                multiplier *= 2;
            if (multiplier == 1)
                return grid;

            bool[,] target = new bool[grid.GetLength(0) * multiplier, grid.GetLength(0) * multiplier];

            for (int r = 0; r < multiplier; r++)
            {
                for (int c = 0; c < multiplier; c++)
                {
                    RenderAt(target, grid, r * grid.GetLength(0), c * grid.GetLength(1));
                }
            }
            return target;
        }

        //Alternate implementation, pads edges
        //public byte[] GetBytes(bool includeMeta = false)
        //{
        //    RenderAndFill();
        //    int nc = RFgrid.GetLength(1) / 8 + (RFgrid.GetLength(1) % 8 == 0 ? 0 : 1); //how many bytes are needed per row
        //    byte[] res = new byte[nc * RFgrid.GetLength(0) + (includeMeta ? 12 : 0)];
        //    int idx = 0;
        //    int bIdx = 0;
        //    byte tmp = 0;

        //    if (includeMeta)
        //    {
        //        res[0] = 0x14;     //format
        //        res[1] = 0;     //continuation
        //        res[2] = 1;     //pixel encoding
        //        res[3] = 0;     //Reserved
        //        res[4] = (byte)(RFgrid.GetLength(0) / 256);     //height
        //        res[5] = (byte)(RFgrid.GetLength(0) % 256);     //height
        //        res[6] = (byte)(RFgrid.GetLength(1) / 256);     //width
        //        res[7] = (byte)(RFgrid.GetLength(1) % 256);     //width
        //        res[8] = (byte)(600 / 256);     //x-resolution
        //        res[9] = (byte)(600 % 256);     //x-resolution
        //        res[10] = (byte)(600 / 256);     //y-resolution
        //        res[11] = (byte)(600 % 256);     //y-resolution

        //        idx = 12;
        //    }

        //    for (int r = 0; r < RFgrid.GetLength(0); r++)
        //    {
        //        for (int c = 0; c < RFgrid.GetLength(1); c++)
        //        {
        //            tmp = (byte)((tmp << 1) + (RFgrid[r, c] ? 0 : 1));  //inverted, used to be 1:0 due to pantograph creator outputs
        //            bIdx++;
        //            if (bIdx == 8)
        //            {
        //                res[idx] = tmp;
        //                tmp = 0;
        //                idx++;
        //                bIdx = 0;
        //            }
        //        }
        //        if (bIdx != 8 && bIdx != 0) //clean up residual bytes
        //        {
        //            while (bIdx != 8)
        //            {
        //                bIdx++;
        //                tmp = (byte)(tmp << 1);
        //            }
        //            res[idx] = tmp;
        //            tmp = 0;
        //            idx++;
        //            bIdx = 0;
        //        }
        //    }
        //    return res;
        //}

        /// <summary>
        /// Returns bytes as a string to be pasted into source code
        /// </summary>
        /// <param name="includeMeta"></param>
        /// <returns></returns>
        public string GetBytesAsString(bool includeMeta = false)
        {
            byte[] bytes = GetBytes(includeMeta);
            StringBuilder sb = new StringBuilder();
            sb.Append("new byte[] {");
            bool isFirst = true;
            foreach (byte b in bytes)
            {
                if (!isFirst)
                    sb.Append(",");
                sb.Append("0x");
                sb.AppendFormat("{0:X2}", b);
                isFirst = false;
            }
            sb.Append("}");
            return sb.ToString();
        }
    }
}
