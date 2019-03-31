using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Nonograms
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
                    break;
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
                        break;
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
        private List<List<Pattern>> VarSpace = new List<List<Pattern>>();
        private static Element lastChild;
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

        //Initializes A*
        public static void init(object startPoint)
        {
            AStar.OpenList.Clear();
            AStar.ClosedList.Clear();
            AStar.Successors.Clear();
            AStar.OpenListDic.Clear();
            AStar.ClosedDic.Clear();
            List<List<Pattern>> initSpace = startPoint as List<List<Pattern>>;
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
        public static List<List<Pattern>> pop()
        {
            lastChild = AStar.OpenList[0];
            AStar.OpenListDic.Remove(lastChild.hash);
            AStar.OpenList.RemoveAt(0);
            return lastChild.VarSpace;
        }

        //Pushes a new element to be processed by A*
        public static void push(object newElement)
        {
            List<List<Pattern>> initSpace = newElement as List<List<Pattern>>;
            if (initSpace != null)
            {
                Element e = new Element();
                e.VarSpace = initSpace;
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
            //Get guessed element based on its domain size 
            List<int> domainSizes = new List<int>();
            for (int i = 0; i < VarSpace.Count; i++)
                domainSizes.Add(VarSpace[i].Count);
            int minDomain;
            int minDomainNode=0;
            minDomain = domainSizes.Where(x => x > 1).Min();
            minDomainNode = domainSizes.IndexOf(minDomain);
            //for (int i = 0; i < VarSpace.Count; i++)
            //    if (VarSpace[i].Count == minDomain && VarSpace[i].Count > 1)
            //    {
            //        minDomain = VarSpace[i].Count;
            //        minDomainNode = i;
            //    }
            foreach (Pattern value in VarSpace[minDomainNode])
            {
                sElement = new Element();
                //Clone this.VarSpace to sElements
                foreach (List<Pattern> tempDomain in VarSpace)
                {
                    List<Pattern> temp = new List<Pattern>(tempDomain);
                    sElement.VarSpace.Add(temp);
                }
                sElement.VarSpace[minDomainNode].Clear();
                sElement.VarSpace[minDomainNode].Add(value);
                sElement.Parent = this;
                sElement.hash = sElement.GetHash();
                sElements.Add(sElement);
            }

            return sElements;
        }

        //Creates a hash for the element
        private int GetHash()
        {
            int newHash = 0;
            foreach (List<Pattern> domain in VarSpace)
                foreach (Pattern value in domain)
                    newHash += value.Hash;
            return newHash;
        }
    }
}
