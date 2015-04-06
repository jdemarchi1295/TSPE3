using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TroySecurePortMonitorUserInterface.Pantograph2
{
    /// <summary>
    /// Defines the 2 different dot patterns that can make up a pattern
    /// </summary>
    public class PatternDefinition
    {
        public DataDefinition pattern1 = new DataDefinition();
        public DataDefinition pattern2 = new DataDefinition();
    }

    /// <summary>
    /// Defines the bytes that make up a single dot pattern
    /// </summary>
    public class DataDefinition
    {
        /// <summary>
        /// One dimensional byte array to insert into PCL stream
        /// </summary>
        public byte[] data = null;
        /// <summary>
        /// The height of the rasterized image
        /// </summary>
        public int height = 0;
        /// <summary>
        /// The width of the rasterized image
        /// </summary>
        public int width = 0;
    }
}
