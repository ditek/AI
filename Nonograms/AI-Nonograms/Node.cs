using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Nonograms
{
    public class Node
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Node Parent { get; set; }

        public Node()
        {
            X = Y = 0;
            Parent = null;
        }

        public Node(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void Set(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static bool IsEqual(Node node1, Node node2)
        {
            return node1.X == node2.X && node1.Y == node2.Y;
        }

        /// <summary>
        /// If the node is contained in nodeList return true else return false
        /// </summary>
        public virtual bool IsContained(List<Node> nodeList)
        {
            foreach (Node n in nodeList)
                if (Node.IsEqual(this, n))
                    return true;
            return false;
        }

        /// <summary>
        /// If the node is contained in nodeList return true and its index, else return false and set index to -1
        /// </summary>
        public bool IsContained(List<Node> nodeList, ref int index)
        {
            foreach (Node n in nodeList)
                if (Node.IsEqual(this, n))
                {
                    index = nodeList.IndexOf(n);
                    return true;
                }
            index = -1;
            return false;
        }

        public int IndexIn(List<Node> nodeList)
        {
            int index;
            foreach (Node n in nodeList)
                if (Node.IsEqual(this, n)) 
                    return nodeList.IndexOf(n);;
            index = -1;
            return index;
        }

        public static int Distance(Node n1, Node n2)
        {
            return (int)(Math.Abs(n1.X - n2.X) + Math.Abs(n1.Y - n2.Y));
        }
    }
}
