using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Flow
{
    public class AStar : Element
    {
        public static List<Element> OpenList = new List<Element>();
        public static List<Element> ClosedList = new List<Element>();
        public static List<Element> Successors = new List<Element>();
        public static List<Element> Path = new List<Element>();

        public enum PathStatus
        {
            not_found,
            found,
            nonexistent,
            input_error,
            working
        };

        public enum Mode
        {
            depth_first,
            breadth_first,
            best_first
        };

        public static Mode RunMode { get; set; }

        public static PathStatus Run()
        {
            switch (RunMode)
            {
                case Mode.best_first:
                    return bestFirst();
                case Mode.breadth_first:
                    return breadth_first();
                case Mode.depth_first:
                    return depth_first();
                default:
                    return PathStatus.input_error;
            }
        }
        public static Dictionary<int, Element> ClosedDic = new Dictionary<int, Element>();
        public static Dictionary<int, Element> OpenListDic = new Dictionary<int, Element>();
        // Populates Path list with path nodes
        public static void createPath(Element n)
        {
            Path.Clear();
            Path.Add(n);
            while (n.Parent != null)
            {
                n = n.Parent;
                Path.Add(n);
            }
        }

        public static PathStatus bestFirst()
        {
            Element currentElement = new Element();
            if (OpenList.Count == 0)
                return PathStatus.not_found;
            //Pop the first item off the open list.
            currentElement = OpenList[0];
            OpenListDic.Remove(currentElement.hash);
            OpenList.RemoveAt(0);

            //add the item to the closed list
            //ClosedList.Add(currentElement);
            if (!ClosedDic.ContainsKey(currentElement.hash))
                ClosedDic.Add(currentElement.hash, currentElement);

            //create path (in Flow there's no need to do this for each step)
            //createPath(currentElement);

            //Check if the current Element is the goal
            //(This can be checked for each child instead)
            if (currentElement.H == 0)
            {
                createPath(currentElement);
                return PathStatus.found;
            }
            List<Element> sElements = currentElement.getSuccessors();
            //foreach (Element sElement in sElements)       //foreach isn't used for it doesn't allow element mod.
            for (int i = 0; i < sElements.Count; i++)
            {
                Element sElement = sElements[i];

                //There's no actual need for this (drawing purposes)
                //Successors.Add(sElement);

                bool found = false;
                //Look for the Element in the OpenList
                if (OpenListDic.ContainsKey(sElement.hash))
                {
                    Element o = OpenListDic[sElement.hash];
                    if (sElement.G < o.G)
                    {
                        o.Parent = currentElement;
                        o.G = sElement.G;
                    }
                    sElement = o;
                    found = true;
                }
                //Look for the Element in the ClosedList                   
                if (!found)
                {
                    //if(ClosedDic.TryGetValue(sElement.hash,out c))
                    if (ClosedDic.ContainsKey(sElement.hash))
                    {
                        Element c = ClosedDic[sElement.hash];
                        if (sElement.G < c.G)
                        {
                            c.Parent = currentElement;
                            c.G = sElement.G;
                            //propagate_path_improvements(sElement);  //Required if propogate alg. is used
                        }
                        sElement = c;
                        found = true;
                    }
                    //Alternative (slower) methode
                    //foreach (Element c in ClosedList)
                    //    if (Element.IsEqual(sElement, c))
                    //    {
                    //        if (sElement.G < c.G)
                    //        {
                    //            c.Parent = currentElement;
                    //            c.G = sElement.G;
                    //            //propagate_path_improvements(sElement);  //Required if propogate alg. is used
                    //        }
                    //        sElement = c;
                    //        found = true;
                    //        break;
                    //    }
                }
                //currentElement.Kids.Add(sElement);        //Required if propogate alg. is used

                //if it's not in both lists add it to the OpenList
                if (!found)
                {
                    OpenList.Add(sElement);
                    OpenListDic.Add(sElement.hash, sElement);
                }
            }
            //sort OpenList
            OpenList = OpenList.OrderBy(n => n.F).ThenBy(n => n.H).ToList();

            return PathStatus.working;
        }

        /*  //Required if propogate alg. is used
        private static void propagate_path_improvements(Element e)
        {
            foreach (Element kid in e.Kids)
                if (e.G + normalCost < kid.G)
                {
                    kid.Parent = e;
                    kid.G = e.G + normalCost;
                    propagate_path_improvements(kid);
                }
        }
        */

        public static PathStatus depth_first_iterative(Element currentElement)
        {
            ClosedList.Add(currentElement);

            List<Element> sElements = currentElement.getSuccessors();
            foreach (Element sElement in sElements)
            {
                //Check if the Element is colored
                if (sElement.IsContained(ClosedList))
                    continue;

                ClosedList.Add(sElement);
                if (sElement.H == 0)
                    return PathStatus.found;
                else
                    return depth_first_iterative(sElement);
            }
            return PathStatus.not_found;
        }

        public static PathStatus depth_first()
        {
            Element currentElement = new Element();
            if (OpenList.Count == 0)
                return PathStatus.not_found;

            //Pop the top of the open list.
            currentElement = OpenList.Last();
            OpenList.RemoveAt(OpenList.Count - 1);

            //add the item to the colored list
            ClosedList.Add(currentElement);

            //create path
            createPath(currentElement);
            List<Element> sElements = currentElement.getSuccessors();
            foreach (Element sElement in sElements)
            {
                //Check if the Element is colored
                if (sElement.IsContained(ClosedList))
                    continue;
                //Check if the current Element is the goal
                if (sElement.H == 0)
                    return PathStatus.found;
                sElement.Parent = currentElement;
                OpenList.Add(sElement);
            }
            return PathStatus.working;
        }

        public static PathStatus breadth_first()
        {
            Element currentElement = new Element();
            if (OpenList.Count == 0)
                return PathStatus.not_found;

            //Pop the top of the open list.
            currentElement = OpenList[0];
            OpenList.RemoveAt(0);

            //Check if the Element is already processed
            if (currentElement.IsContained(ClosedList))
                return PathStatus.working;

            //else:add the item to the colored list
            ClosedList.Add(currentElement);

            //create path
            createPath(currentElement);
            List<Element> sElements = currentElement.getSuccessors();
            foreach (Element sElement in sElements)
            {
                //Check if the Element is colored
                if (sElement.IsContained(ClosedList))
                    continue;
                //Check if the current Element is the goal
                if (sElement.H == 0)
                    return PathStatus.found;
                OpenList.Add(sElement);
            }
            return PathStatus.working;
        }
    }

    interface IElement
    {
        bool IsContained(List<Element> nodeList);
        List<Element> getSuccessors();
    }

    public class Element : IElement
    {
        private List<List<int>> VarSpace = new List<List<int>>();
        private static Element lastChild;
        public List<List<int>> Flows = new List<List<int>>();
        public Element Parent { get; set; }
        public int hash { get; set; }
        private int g = 0, h = -1, f;
        public int G
        {
            get { return g; }
            set { g = value; }
        }

        public int H
        {
            get
            {
                //Give an initial value of -1 so it gets calculated only once
                if (h == -1)
                {
                    h = 0;
                    //   foreach (List<int> domain in VarSpace)
                    for (int i = 0; i < VarSpace.Count; i++)
                    {
                        //if it's an invalid solution just give a negative H
                        if (VarSpace[i].Count == 0)
                        {
                            h = -10;
                            return h;
                        }
                        h += VarSpace[i].Count - 1;
                    }
                    //Add to H the distance b/w the last point in each flow and the next endpoint
                    List<int> Fathers;
                    for (int i = 0; i < Main.EndPoints.Count; i += 2)
                    {
                        //Node n = Main.EndPoints[i];
                        //Fathers = new List<int>();
                        //int index = n.IndexIn(Main.Nodes);
                        int flowID = i / 2;
                        //Fathers = new List<int>( Flows[flowID]);
                        //Node lastChild = Main.FindLastChild(index, Fathers);
                        //h += Node.Distance(lastChild, Main.EndPoints[i + 1]);
                        h += Node.Distance(Main.Nodes[Flows[flowID][Flows[flowID].Count-1]], Main.EndPoints[i + 1]);
                    }
                }
                return h;
            }
            set { h = value; }
        }

        public int F
        {
            get { return G + H; }
            set { f = value; }
        }

        public Element()
        {
            int flowID;
            //Initialize each flow with the first point of each color
            for (int i = 0; i < Main.EndPoints.Count; i += 2)
            {
                flowID = i / 2;
                //Flows.Insert(i, new List<int>());
                Flows.Add( new List<int>());
                Flows[flowID].Add(Main.EndPoints[i].IndexIn(Main.Nodes));
            }
        }

        //Initializes A*
        public static void init(object startPoint)
        {
            AStar.OpenList.Clear();
            AStar.ClosedList.Clear();
            AStar.Successors.Clear();
            AStar.OpenListDic.Clear();
            AStar.ClosedDic.Clear();
            List<List<int>> initSpace = startPoint as List<List<int>>;
            if (initSpace != null)
            {
                Element e = new Element();
                e.VarSpace = initSpace;
                e.hash = e.GetHash();
                AStar.OpenList.Add(e);
                AStar.OpenListDic.Add(e.hash, e);
            }
        }

        //Pops the top element of OpenList
        public static List<List<int>> pop()
        {
            lastChild = AStar.OpenList[0];
            AStar.OpenListDic.Remove(lastChild.hash);
            AStar.OpenList.RemoveAt(0);
            Main.Flows = lastChild.Flows;
            return lastChild.VarSpace;
        }

        //Pushes a new element to be processed by A*
        public static void push(object newElement)
        {
            List<List<int>> initSpace = newElement as List<List<int>>;
            if (initSpace != null)
            {
                Element e = new Element();
                e.VarSpace = initSpace;
                e.Flows = Main.Flows;
                e.hash = e.GetHash();
                e.Parent = lastChild.Parent;
                AStar.OpenList.Insert(0, e);
                AStar.OpenListDic.Add(e.hash, e);
            }
        }

        //Returns wheather two elements are equal
        public static bool IsEqual(Element element1, Element element2)
        {
            return element1.hash == element2.hash;
        }

        //Returns wheather an element is included in a list
        public bool IsContained(List<Element> elementsList)
        {
            foreach (Element n in elementsList)
                if (Element.IsEqual(this, n))
                    return true;
            return false;
        }

        //Get element sucessors based on the num of constrains it apears in
        public List<Element> getSuccessors()
        {
            List<Element> sElements = new List<Element>();
            if (this.H < 0)
                return sElements;

            Element sElement;
            /*
            //Get guessed element based on the num of constrains it apears in
            int maxConst = 0;
            int maxConstVar = 0;
            for (int i = 0; i < Main.ConstraintsPerVar.Count; i++)
                if (Main.ConstraintsPerVar[i] > maxConst && VarSpace[i].Count > 1)
                {
                    maxConst = Main.ConstraintsPerVar[i];
                    maxConstVar = i;
                }
            foreach (int value in VarSpace[maxConstVar])
            {
                sElement = new Element();
                //Clone this.VarSpace to sElements
                foreach (List<int> tempDomain in VarSpace)
                {
                    List<int> temp = new List<int>(tempDomain);
                    sElement.VarSpace.Add(temp);
                }
                sElement.VarSpace[maxConstVar].Clear();
                sElement.VarSpace[maxConstVar].Add(value);
                sElement.Parent = this;
                sElement.hash = sElement.GetHash();
                sElements.Add(sElement);
            }
            */
            /*
            //Get guessed element based on its domain size 
            List<int> domainSizes = new List<int>();
            int minDomain = 100;
            int minDomainNode=0;
            //minDomain = domainSizes.Where(x => x > 1).Min();
           // minDomainNode = domainSizes.IndexOf(minDomain);
            for (int i = 0; i < VarSpace.Count; i++)
                if (VarSpace[i].Count < minDomain && VarSpace[i].Count > 1)
                {
                    minDomain = Main.ConstraintsPerVar[i];
                    minDomainNode = i;
                }
            foreach (int value in VarSpace[minDomainNode])
            {
                sElement = new Element();
                //Clone this.VarSpace to sElements
                foreach (List<int> tempDomain in VarSpace)
                {
                    List<int> temp = new List<int>(tempDomain);
                    sElement.VarSpace.Add(temp);
                }
                sElement.VarSpace[minDomainNode].Clear();
                sElement.VarSpace[minDomainNode].Add(value);
                sElement.Parent = this;
                sElement.hash = sElement.GetHash();
                sElements.Add(sElement);
            }
            */

            //Get successors for the next incomplete color based on their domain sizes
            List<Node> nextSearchNodes = null;
            Node n;
            int index = 0;
            int lastIndex, flowID=0;
            int color = 0;
            List<int> Fathers;
            //for each endpoint find the next uncolored node in the path and add it to the list of candidates to guess
            for (int i = 0; i < Main.EndPoints.Count; i += 2)
            {
                n = Main.EndPoints[i];
                Fathers = new List<int>();
                index = n.IndexIn(Main.Nodes);
                color = VarSpace[index][0];
                //nextSearchNodes = Main.FindNextVoids(index, Fathers);
                flowID = i/2;
                lastIndex = Flows[flowID].Count - 1;
                //Find the next voids of the last node of this flow, with the Fathers list is the flow except this value
                //Fathers = Flows[flowID].Where(x => Flows[flowID].IndexOf(x) != lastIndex).ToList();
                Fathers = new List<int>(Flows[flowID]);
                Fathers.RemoveAt(lastIndex);
                nextSearchNodes = Main.FindNextVoids(Flows[flowID][lastIndex], Fathers);
                //is this an invalid solution?
                Flows[flowID] = Fathers;
                if (nextSearchNodes.Count == 0)
                    return sElements;
                //if the flow is complete from end to end, remove this color from other nodes domains
                // and move to the next color
                if (nextSearchNodes[0].IsContained(Main.EndPoints))
                {
                    Flows[flowID].Add(nextSearchNodes[0].IndexIn(Main.Nodes));
                    if (Settings.UseFlows)
                        foreach (List<int> domain in VarSpace.Where(item => !Flows[flowID].Contains(VarSpace.IndexOf(item))))
                            domain.Remove(color);
                    else
                        foreach (List<int> domain in VarSpace.Where(item => item.Count > 1))
                            domain.Remove(color);
                    continue;
                }
                //else: break and add the nodes to sElements
                else
                    break;
            }
            //foreach (int value in VarSpace[maxConstVar])
            for (int i = 0; i < nextSearchNodes.Count; i++)
            {
                int nodeIndex = nextSearchNodes[i].IndexIn(Main.Nodes);
                sElement = new Element();
                //Clone this.VarSpace to sElement
                for (int j = 0; j < VarSpace.Count; j++)
                    sElement.VarSpace.Add(new List<int>(VarSpace[j]));
                sElement.VarSpace[nodeIndex].Clear();
                sElement.VarSpace[nodeIndex].Add(color);
                //Clone this.Flows to sElement
                for (int j = 0; j < Flows.Count; j++)
                    sElement.Flows[j]=new List<int>(Flows[j]);
                //Add the point to the flow if it's not an endpoint
                if (nextSearchNodes.Count>1 || !nextSearchNodes[0].IsContained(Main.EndPoints))
                    sElement.Flows[flowID].Add(nextSearchNodes[i].IndexIn(Main.Nodes));
                sElement.Parent = this;
                sElement.hash = sElement.GetHash();
                sElements.Add(sElement);
            }

            return sElements;
        }

        //Creates a hash for the element
        private int GetHash()
        {
            StringBuilder str = new StringBuilder();
            char[] arr = new char[3000];
            int i = 0;
            //foreach (List<int> domain in VarSpace)
            foreach (List<int> domain in Flows)
                foreach (int value in domain)
                {
                    //str.Append(value);
                    arr[i++] = (char)value;
                }
            str.Append(arr);
            return str.ToString().GetHashCode();
            //return (int)xxHash.CalculateHash(Encoding.UTF8.GetBytes(str.ToString()));
        }
    }
}
