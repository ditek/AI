using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Nonograms
{
    public class Pattern:List<int>
    {
        private int hash = -1;
        public int Hash
        {
            get
            {
                if (hash == -1)
                    hash = GetHash();
                return hash;
            }
            set { }
        }

        public Pattern():base(){}
        public Pattern(int size) : base(size) { }
        public Pattern(List<int> newList) : base(newList) { }

        public static Pattern operator &(Pattern a, Pattern b)
        {
            Pattern result = new Pattern();
            for (int i = 0; i < Math.Min(a.Count, b.Count); i++)
                result.Add(a[i] & b[i]);
            return result;
        }

        public static Pattern operator |(Pattern a, Pattern b)
        {
            Pattern result = new Pattern();
            for (int i = 0; i < Math.Min(a.Count, b.Count); i++)
                result.Add(a[i] | b[i]);
            return result;
        }


        public static bool IsEqual(Pattern a, Pattern b)
        {
            //if (a.Count != b.Count())
            //    return false;
            for (int i = 0; i < a.Count; i++)
                if (a[i] != b[i])
                    return false;
            return true;
        }

        public static bool IsEqual(List<int> a, List<int> b)
        {
            if (a.Count != b.Count())
                return false;
            for (int i = 0; i < a.Count; i++)
                if (a[i] != b[i])
                    return false;
            return true;
        }

        private int GetHash()
        {
            StringBuilder str = new StringBuilder();
            char[] arr = new char[100];
            int i = 0;
            foreach (int value in this)
                arr[i++] = (char)value;
            str.Append(arr);
            return str.ToString().GetHashCode();
        }

    }
}
