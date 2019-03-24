using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Flow
{
    class Settings
    {
        public static int BlockSize { get; set; }
        public static int GridSize { get; set; }
        public static int Speed { get; set; }
        public static int NumColors { get; set; }
        public static bool UseFlows { get; set; }

        public enum SortModes { Ascending, Descending } ;
        public static SortModes sortMode { get; set; }

        public Settings()
        {
            GridSize = 10;
            BlockSize = 50;
            Speed = 30;
            sortMode = SortModes.Ascending;
            UseFlows = true;
        }
    }
}
