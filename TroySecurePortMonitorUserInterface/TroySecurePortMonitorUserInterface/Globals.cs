using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Troy.PortMonitor.Core.XmlConfiguration;

namespace TroySecurePortMonitorUserInterface
{
    public static class Globals
    {
        public static bool Lite;
        public static string currentPortName;
        public static string currentBasePath;
        public static string currentConfigPath;
        public static string currentDataPath;
        public static string appDataPath;
        public static TroyFontGlyphMapList FontGlyphMapList;

        public static DataCaptureList dcaplist = new DataCaptureList();

        public static TroyCustomPatterns customPatterns = new TroyCustomPatterns();
        public static string[] CustomPatternName = new string[4];

        public static bool PantographChanged = false;

        public static CellSetup[,] CellSetups = new CellSetup[4, 10];

        public static void Initialize()
        {
            for (int cntr = 0; cntr < 4; cntr++)
            {
                for (int cntr2 = 0; cntr2 < 10; cntr2++)
                {
                    CellSetups[cntr, cntr2] = new CellSetup();
                    CellSetups[cntr, cntr2].CellText = "";
                    CellSetups[cntr, cntr2].PatternEnabled = false;
                }
            }
        }

        public class Densities
        {
            public int DensityPattern1 = 16;
            public int DensityPattern2 = 62;
            public int DensityPattern3 = 20;
            public int DensityPattern4 = 74;
            public int DensityPattern5 = 174;
            public int DensityPattern6 = 30;
            public int DensityPattern7 = 78;
            public int DensityPattern8 = 28;
            public int DensityPattern9 = 78;
            public int DarknessFactor = 1;
        }
        public static Dictionary<string, Densities> BaselineDensity = new Dictionary<string, Densities>();
    }
}
