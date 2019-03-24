using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ciloci.Flee;

namespace AI_Flow
{
    public class Constraint
    {
        public IGenericExpression<bool> Func{ get; set; }
        public List<int> Vars{ get; set; }
        public bool IsEndpoint { get; set; }
        public static bool IsEqual(Constraint c1, Constraint c2)
        {
            //if (c1.Func != c2.Func)
            //    return false;
            if (c1.Vars.Count!=c2.Vars.Count || c1.IsEndpoint != c2.IsEndpoint)
                return false;
            for (int i = 0; i < c1.Vars.Count; i++)
                if (c1.Vars[i] != c2.Vars[i])
                    return false;
            return true;
        }
    }
}
