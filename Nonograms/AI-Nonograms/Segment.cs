using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Nonograms
{
    class Segment
    {
        public List<int> StartDomain = new List<int>();
        public int Size;
        public Segment(int size)
        {
            Size = size;
        }
        public Segment(int size,List<int> startDomain)
        {
            Size = size;
            StartDomain = startDomain;
        }
        public Segment(Segment s)
        {
            Size = s.Size;
            StartDomain = new List<int>(s.StartDomain);
        }

    }
}
