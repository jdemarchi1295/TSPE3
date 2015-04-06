//#define PRINTFAST   //Use smaller grid for fast printing

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TroySecurePortMonitorUserInterface.Pantograph2
{
    /// <summary>
    /// Page generation constants
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Values to use to generate dots (xOver, xDown, yOver, yDown)
        /// </summary>
        public static List<int> OverDown = new List<int>() { 4, 5, 6, 7, 8, 9 };
        //public static List<int> OverDown = new List<int>() { 4, 8, 16, 32 };
        /// <summary>
        /// Sizes of dots to generate
        /// </summary>
        public static List<int> DotSizes = new List<int>() { 3, 4 };

        /// <summary>
        /// Values to use to 'nudge' dots from potential candidates (currently not used)
        /// </summary>
        public static List<int> OverDownSub = new List<int>() { -2, -1, 0, 1, 2 };

        /// <summary>
        /// Cell width in px
        /// </summary>
        public static int cellW = 175*2;
        /// <summary>
        /// Cell height in px
        /// </summary>
        public static int cellH = 275*2;
        /// <summary>
        /// Cell padding in px
        /// </summary>
        public static int padding = 5;
        /// <summary>
        /// DPI of printer
        /// </summary>
        public static int DPI = 600;

#if PRINTFAST
        public static int PageW = 2 * DPI;
        public static int PageH = 3 * DPI;
#else
        public static int PageW = 8 * DPI;
        public static int PageH = 10 * DPI;
#endif
        /// <summary>
        /// Used to determine midpoint height of cell
        /// </summary>
        public static int offY = (cellH - cellW) / 2;
        /// <summary>
        /// Offset from corner for diagonal slash in cell in px
        /// </summary>
        public static int offL = 20*2;

        /// <summary>
        /// X-position of top left cell in px
        /// </summary>
        public static int shiftX = 250;
        /// <summary>
        /// Y-position of top left cell in px
        /// </summary>
        public static int shiftY = 250;

        /// <summary>
        /// Number of columns per page based off other constants
        /// </summary>
        public static int NumColumns = (PageW - shiftX) / cellW;
        /// <summary>
        /// Number of rows per page based off other constants
        /// </summary>
        public static int NumRows = (PageH - shiftY) / cellH;
        /// <summary>
        /// Number of cells per page based off other constants
        /// </summary>
        public static int CellsPerPage = NumColumns * NumRows;

        public static int DensityRange = 10;
    }
}
