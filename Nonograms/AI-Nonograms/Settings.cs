using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Nonograms
{
    class Settings
    {
        public static int BlockSize { get; set; }
        public static int GridWidth { get; set; }
        public static int GridHeight { get; set; }
        public static int Speed { get; set; }
        public static int GridSize { get; set; }

        public Settings()
        {
            GridWidth = GridHeight = 10;
            BlockSize = 30;
            Speed = 30;
        }
    }
}
