using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.IO;
using Ciloci.Flee;

namespace AI_Flow
{
    public partial class Main : Form
    {
        //Flag for task in progress
        bool running = false;
        private static Brush[] colorArr = { Brushes.Red, Brushes.Green, Brushes.DeepSkyBlue, Brushes.Yellow, Brushes.Lime, Brushes.Orange,
                               Brushes.Bisque, Brushes.Fuchsia, Brushes.BlueViolet, Brushes.BurlyWood, 
                               Brushes.Pink, Brushes.Crimson };
        private Stopwatch stopwatch = new Stopwatch();
        private AStar.PathStatus status;
        private int tryNumber;
        private bool timerStandby = false;

        private List<string> InputLines = new List<string>();
        public static List<Node> Nodes = new List<Node>();
        public static List<List<int>> Flows = new List<List<int>>();
        public static List<Node> EndPoints = new List<Node>();
		//Stores the current variables values when revising a constraint
		//(Equivalent to eContext.Variables in dynamic functions)
        private static List<int> VarsValues = new List<int>(100);

        //Pre-created variables to avoid runtime creation
        static int index, tempLength, tempNewLength, tempInt;
        static Constraint tempConstraint;
        static bool found;
        static int[] Array3 = new int[3];
        static int[] Array2 = new int[2];
        static int[] Array1 = new int[1];

        /** GAC specific declarations **/
        //Lists required for the algorithm
        private static List<List<int>> VarDomains = new List<List<int>>();
        public static List<List<int>> StateDomains = new List<List<int>>();
        private static List<List<int>> StateDomainsBuffer = new List<List<int>>();
        public static List<Constraint> Constraints = new List<Constraint>();
        public static List<int> ConstraintsPerVar = new List<int>();
        private static Queue<Instance> ReviseQ = new Queue<Instance>();
        //Struct containing a variable instance, constrain instance pair
        private struct Instance
        {
            public int var;
            public Constraint constraint;
        };

        static bool ConstraintFunction(Constraint c)
        {
            tempInt = VarsValues[c.Vars[0]];
            int condition = 1;
            if (!c.IsEndpoint)
                condition = 2;
            switch (c.Vars.Count)
            {
                case 5:
                    return equal(tempInt, VarsValues[c.Vars[1]]) + equal(tempInt, VarsValues[c.Vars[2]]) + equal(tempInt, VarsValues[c.Vars[3]]) + equal(tempInt, VarsValues[c.Vars[4]]) >= condition;
                case 4:
                    return equal(tempInt, VarsValues[c.Vars[1]]) + equal(tempInt, VarsValues[c.Vars[2]]) + equal(tempInt, VarsValues[c.Vars[3]]) >= condition;
                default:
                    return equal(tempInt, VarsValues[c.Vars[1]]) + equal(tempInt, VarsValues[c.Vars[2]]) >= condition;
            }
        }

        //Sets up initial var domains, evaluation expression and ReviseQ
        private void init()
        {
            VarDomains.Clear();
            Constraints.Clear();
            ReviseQ.Clear();
            VarsValues.Clear();
            //Fill variable space with each variable's domain
            foreach (Node n in Nodes)
            {
                List<int> Domain = new List<int>();
                int index = 0;
                // If the node is and EndPoint then its color is its index/2
                if (n.IsContained(EndPoints, ref index))
                    Domain.Add(index / 2);
                else
                    for (int i = 0; i < Settings.NumColors; i++)
                        Domain.Add(i);
                VarDomains.Add(Domain);
				//Set up VarsValues list
                VarsValues.Add(0);
            }
            int[] neighborX = { -1, 1, 0, 0 };
            int[] neighborY = { 0, 0, -1, 1 };
            //Add a constraint to constraints list for each node in the graph
            foreach (Node n in Nodes)
            {
                Constraint constraint = new Constraint();
                constraint.Vars = new List<int>();
                //Add the central node to the constraint
                constraint.Vars.Add(Nodes.IndexOf(n));
                //Add the neighbors to the constraint
                for (int i = 0; i < 4; i++)
                {
                    int x = (int)n.X + neighborX[i];
                    int y = (int)n.Y + neighborY[i];
                    int index = 0;
                    if (x >= 0 && x < Settings.GridSize && y >= 0 && y < Settings.GridSize)
                    {
                        Node neighbor = new Node(x, y);
                        neighbor.IsContained(Nodes, ref index);
                        constraint.Vars.Add(index);
                    }
                }
                constraint.IsEndpoint = n.IsContained(EndPoints);
                Constraints.Add(constraint);
            }

            //Create a list to hold the number of constrains theat a var apears in
            //(can be used when finding successor based on num of constraints)
            for (int i = 0; i < Nodes.Count; i++)
                ConstraintsPerVar.Insert(i, 0);

            //Populate ReviseQ with (var,constrain) pairs
            foreach (Constraint constraint in Constraints)
                foreach (int var in constraint.Vars)
                {
                    Instance rev;
                    rev.var = var;
                    rev.constraint = constraint;
                    ReviseQ.Enqueue(rev);
                    ConstraintsPerVar[var]++;
                }
        }

        //Finds the last child of the given flow defined by a point and its parents
        public static Node FindLastChild(int myIndex, List<int> Fathers)
        {
            Constraint c = Constraints[myIndex];
            List<int> Neighbours = c.Vars.Where(item => item != myIndex && !Fathers.Contains(item)).ToList();
            //for each neighbour of the end point check if it has the same color of the endpoint
            foreach (int x in Neighbours)
                if (StateDomains[x].Count == 1 && StateDomains[x][0] == StateDomains[myIndex][0])
                {
                    Fathers.Add(myIndex);
                    return FindLastChild(x, Fathers);
                }
            return Nodes[myIndex];
        }

        //Finds the next potential childs of the given flow, or return the endpoint if the flow is complete
        public static List<Node> FindNextVoids(int myIndex, List<int> Fathers)
        {
            Constraint c = Constraints[myIndex];
            List<Node> Voids = new List<Node>();
            List<int> Neighbours = c.Vars.Where(item => item != myIndex && !Fathers.Contains(item)).ToList();
            //if this is an endpoint and there are previous fathers (this isn't the initial node) 
            // then no need to check this color (just return a list with the endpoint)
            if (Nodes[myIndex].IsContained(EndPoints) && Fathers.Count > 0)
            {
                Voids.Add(Nodes[myIndex]);
                return Voids;
            }
            //for each neighbour of the current point check if it has the same color
            int color = StateDomains[myIndex][0];
            foreach (int x in Neighbours)
                if (StateDomains[x].Count == 1 && StateDomains[x][0] == color)
                {
                    Fathers.Add(myIndex);
                    return FindNextVoids(x, Fathers);
                }
            //If we reached here then this is the last child, now we need to find the next potential children
            Fathers.Add(myIndex);
            foreach (int x in Neighbours)
                if (StateDomains[x].Count > 1 && StateDomains[x].Contains(color))
                    Voids.Add(Nodes[x]);

            return Voids;
        }

        private static bool ReviseRecursion(Instance reviseElement, List<List<int>> Domains, List<int> VarsList)
        {
                int var = VarsList[0];
                //Foreach value in the non-focal var domain:
                foreach (int varValue in Domains[var])
                {
                    VarsValues[var] = varValue;
                    List<int> newVarsList = new List<int>(VarsList);// VarsList.Where(item => item != var).ToList();
                    newVarsList.Remove(var);
                    if (newVarsList.Count > 0)
                    {
                        if (ReviseRecursion(reviseElement, Domains, newVarsList))
                            return true;
                    }
                    else
                    {
                        if (ConstraintFunction(reviseElement.constraint))
                            return true;
                    }
                }
            return false;
        }

        private static int[] getList(int length)
        {
            switch (length)
            {
                case 3:
                    return Array3;
                case 2:
                    return Array2;
                default:
                    return Array1;
            }
        }

        //The function ReviseRecursion is rebuilt using arrays for performance
        private static bool ReviseRecursion2(Instance reviseElement, List<List<int>> Domains, int[] VarsList)
        {
            index = 0;
            tempLength = VarsList.Length;
            tempNewLength = tempLength - 1;
                int var = VarsList[0];
                //Foreach value in the non-focal var domain:
                for (int j = 0; j < Domains[var].Count; j++)
                //foreach (int varValue in Domains[var])
                {
                    //VarsValues[var] = varValue;
                    VarsValues[var] = Domains[var][j];
                    if (tempNewLength > 0)
                    {
                        for (int i = 0; i < tempLength; i++)
                            if (VarsList[i] == var)
                            {
                                index = i;
                                break;
                            }
                        //int[] newVarsList = new int[tempNewLength];// VarsList.Where(item => item != var).ToList();
                        int[] newVarsList = getList(tempNewLength);// VarsList.Where(item => item != var).ToList();
                        //newVarsList.RemoveAt(index);
                        if (index > 0)
                            Array.Copy(VarsList, 0, newVarsList, 0, index);
                        if (index < tempNewLength)
                            Array.Copy(VarsList, index + 1, newVarsList, index, tempNewLength - index);

                        if (ReviseRecursion2(reviseElement, Domains, newVarsList))
                            return true;
                        tempLength++;
                        tempNewLength++;
                    }
                    else
                    {
                        if (ConstraintFunction(reviseElement.constraint))
                            return true;
                    }
                }
            return false;
        }

        //private static int[] RemoveAt(int[] source, int index)
        //{
        //    int[] dest = new int[source.Length - 1];
        //    if (index > 0)
        //        Array.Copy(source, 0, dest, 0, index);

        //    if (index < source.Length - 1)
        //        Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

        //    return dest;
        //}

        //Takes a var-constrain pair and updates the var domain
        private static void Revise(Instance reviseElement, List<List<int>> Domains)
        {
            int focalVar = reviseElement.var;
            List<int> ToRemove = new List<int>();

            //Check each value of the focal variable against the constrain
            foreach (int focalValue in Domains[focalVar])
            {
                VarsValues[focalVar] = focalValue;
                //Foreach non-focal variable in constrain variables
                //List<int> nonFocalVars = reviseElement.constraint.Vars.Where(item => item != focalVar).ToList();
                //int[] nonFocalVars = reviseElement.constraint.Vars.Where(item => item != focalVar).ToArray();
                tempLength = reviseElement.constraint.Vars.Count;
                int[] nonFocalVars = new int[tempLength - 1];
                index = 0;
                for (int i = 0; i < tempLength; i++)
                    if (reviseElement.constraint.Vars[i] != focalVar)
                        nonFocalVars[index++] = reviseElement.constraint.Vars[i];
                //Remove the value that doesn't fulfill the constrain from variable domain 
                if (!ReviseRecursion2(reviseElement, Domains, nonFocalVars))
                    ToRemove.Add(focalValue);
            }
            //Execute the removal
            foreach (int x in ToRemove)
                Domains[focalVar].Remove(x);
        }

        //Calls Revise for each item in ReviseQ and pushs new items for vars with modified domains
        /* (while RQ.Count!=0):
         * Pop a revise request (Xi,Ci)
         * Call Revise 
         * If domain got reduced: push (Xj,Cj) for each Cj!=Ci where Xi appears and each Xj!=Xi */
        private static void DomainFilterLoop(List<List<int>> Domains)
        {
            Instance reviseInstance;
            int reviseVar;
            while (ReviseQ.Count != 0)
            {
                //Pop a revise request (Xi,Ci) and call Revise
                reviseInstance = ReviseQ.Dequeue();
                reviseVar = reviseInstance.var;
                int initialCount = Domains[reviseVar].Count;
                Revise(reviseInstance, Domains);
                //If domain got reduced: push (Xj,Cj) for each Cj!=Ci where Xi appears and each Xj!=Xi
                if (Domains[reviseVar].Count != initialCount)
                {
                    //foreach (Constraint constraint in Constraints.Where(item => !Constraint.IsEqual(item, reviseInstance.constraint) && item.Vars.Contains(reviseVar)))
                    for (int i = 0; i < Constraints.Count; i++)
                    {
                        tempConstraint = Constraints[i];
                        if (Constraint.IsEqual(tempConstraint, reviseInstance.constraint))
                            continue;
                        //check if item.Vars.Contains(reviseVar)
                        found = false;
                        //for(int j=0;j<tempConstraint.Vars.Count;j++)
                        //    if(tempConstraint.Vars[j]==reviseVar)
                        //    {
                        //        found = true;
                        //        break;
                        //    }
                        //Previous loop unrolled:
                        switch (tempConstraint.Vars.Count)
                        {
                            case 5:
                                if (tempConstraint.Vars[0] == reviseVar || tempConstraint.Vars[1] == reviseVar ||
                                    tempConstraint.Vars[2] == reviseVar || tempConstraint.Vars[3] == reviseVar ||
                                    tempConstraint.Vars[4] == reviseVar)
                                    found = true;
                                break;
                            case 4:
                                if (tempConstraint.Vars[0] == reviseVar || tempConstraint.Vars[1] == reviseVar ||
                                   tempConstraint.Vars[2] == reviseVar || tempConstraint.Vars[3] == reviseVar)
                                    found = true;
                                break;
                            case 3:
                                if (tempConstraint.Vars[0] == reviseVar || tempConstraint.Vars[1] == reviseVar ||
                                    tempConstraint.Vars[2] == reviseVar)
                                    found = true;
                                break;
                        }
                        if (!found)
                            continue;
                        //foreach (Constraint constraint in Constraints.Where(item => !Constraint.IsEqual(item, reviseInstance.constraint) && item.Vars.Contains(reviseVar)))
                        for (int j = 0; j < tempConstraint.Vars.Count; j++)
                            if (tempConstraint.Vars[j] != reviseVar)
                            //foreach (int x in tempConstraint.Vars.Where(item => item != reviseVar))
                            {
                                Instance new_rev;
                                new_rev.var = tempConstraint.Vars[j];
                                new_rev.constraint = tempConstraint;
                                ReviseQ.Enqueue(new_rev);
                            }
                    }
                }
            }
        }

        //Launch the Rerun given the assumed variable
        private static void Rerun(int guessedVar)
        {
            /*For each constrain that contains the guessed variable and each variable except the 
             * guessed one, create a new variable/constrain instance and add it to the queue     */
            foreach (Constraint constrain in Constraints.Where(item => item.Vars.Contains(guessedVar)))
                foreach (int x in constrain.Vars.Where(item => item != guessedVar))
                {
                    Instance new_rev;
                    new_rev.var = x;
                    new_rev.constraint = constrain;
                    ReviseQ.Enqueue(new_rev);
                }
        }

        private void threadMain_DoWork(object sender, DoWorkEventArgs e)
        {
            //A variable to store search status
            status = AStar.PathStatus.working;
            //Load the queue with REVISE requests
            init();
            StateDomains = VarDomains.ToList();
            //Call DomainFilterLoop for the initial variables domain
            DomainFilterLoop(StateDomains);
            //A buffer for the search state
            StateDomainsBuffer = StateDomains.ToList();
            //Check if the problem is solvable
            foreach (List<int> domain in StateDomains)
                if (domain.Count == 0)
                {
                    e.Result = AStar.PathStatus.nonexistent;
                    return;
                }
            //Choose the required A* algorithm
            AStar.RunMode = AStar.Mode.best_first;
            //Initializes A* and give it the start point
            AStar.init(StateDomains);
            //Main search loop
            while (status == AStar.PathStatus.working && running == true)
            {
                //Make an assumption and return working status
                status = AStar.Run();
                //Detect unsolvable problem
                if (AStar.OpenList.Count == 0)
                    status = AStar.PathStatus.not_found;
                if (status != AStar.PathStatus.working)
                    break;
                //Get the new search state
                StateDomains = AStar.pop();
                //Control execution speed. If speed>1000 work at max power
                if (Settings.Speed <= 1000)
                    Thread.Sleep(1000 / Settings.Speed);
                //Launch the Rerun given the assumed variable
                Rerun(getAssumedVar());
                //Call DomainFilterLoop for the current search state
                DomainFilterLoop(StateDomains);
                //A buffer for the search state
                StateDomainsBuffer = StateDomains.ToList();
                //Prepare for a new assumption
                AStar.push(StateDomains);
            }
            e.Result = status;
        }

        private void theradMain_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            stopwatch.Stop();
            txtTreeNodes.Text = (AStar.ClosedDic.Count + AStar.OpenList.Count).ToString();
            txtPoppedNodes.Text = AStar.ClosedDic.Count.ToString();
            txtPathLength.Text = AStar.Path.Count.ToString();
            txtResult.Text = e.Result.ToString();
            txtTime.Text = stopwatch.ElapsedMilliseconds.ToString();
            running = false;
        }

        //Returns the var assumed by A*
        int getAssumedVar()
        {
            for (int i = 0; i < StateDomains.Count; i++)
            {
                if (StateDomains[i].Count != StateDomainsBuffer[i].Count)
                    return i;
            }
            //Nothing has been changed
            return -1;
        }

        //Converts the read file into nodes and edges
        /* Input format:
         * [Grid size] [Num. colors]
         * [index] [end point 1] [end point 2]
         * ...
         * ( index = 0 -> Num.colors-1 )
         */
        void parse()
        {
            EndPoints.Clear();
            Flows.Clear();
            Nodes.Clear();
            List<Node> endPoints;
            List<List<Node>> endPointsList = new List<List<Node>>();
            string[] parameters = InputLines[0].Split(' ');
            Settings.GridSize = Convert.ToInt32(parameters[0]);
            Settings.NumColors = Convert.ToInt32(parameters[1]);
            //Add the end points of each color to global EndPoints list and as a new record in Flows list
            for (int i = 1; i <= Settings.NumColors; i++)
            {
                parameters = InputLines[i].Split(' ');
                endPoints = new List<Node>();
                endPoints.Add(new Node(Convert.ToInt32(parameters[1]), Convert.ToInt32(parameters[2])));
                endPoints.Add(new Node(Convert.ToInt32(parameters[3]), Convert.ToInt32(parameters[4])));
                //Flows.Insert(Convert.ToInt32(parameters[0]), endPoints);
                endPointsList.Add(endPoints);
            }
            //Order the endpoints based on the distance between each pair
            if (Settings.sortMode == Settings.SortModes.Ascending)
                endPointsList = endPointsList.OrderBy(item => Node.Distance(item[0], item[1])).ToList();
            else
                endPointsList = endPointsList.OrderByDescending(item => Node.Distance(item[0], item[1])).ToList();
            foreach (List<Node> x in endPointsList)
            {
                EndPoints.AddRange(x);
                //Flows.Add(x);
            }

            //Add a node for each grid
            for (int y = 0; y < Settings.GridSize; y++)
                for (int x = 0; x < Settings.GridSize; x++)
                    Nodes.Add(new Node(x, y));
        }

        private static int equal(Int32 x, Int32 y)
        {
            if (x == y)
                return 1;
            else
                return 0;
        }

        public Main()
        {
            InitializeComponent();
            new Settings();
            tmrTryAgain.Interval = 90000;
            tmrDraw.Interval = 100;
            tmrDraw.Start();
            UpdateValues(null, null);
            comboBox1.SelectedIndex = 0;
            checkBox1.Checked = Settings.UseFlows;
        }

        private void UpdateValues(object sender, EventArgs e)
        {
            try
            {
                Settings.Speed = Convert.ToInt32(txtSpeed.Text);
            }
            catch
            {
                MessageBox.Show("Please check your inputs!");
            }
            picCanvas.Width = Settings.GridSize * Settings.BlockSize + Settings.BlockSize;
            picCanvas.Height = Settings.GridSize * Settings.BlockSize;
        }

        private void UpdateScreen(object sender, EventArgs e)
        {
            picCanvas.Invalidate();
        }

        private static void DrawNodeRectangle(Graphics canvas, Brush color, Node node)
        {
            canvas.FillRectangle(color, new Rectangle((int)(node.X * Settings.BlockSize), (int)(
                        node.Y * Settings.BlockSize), Settings.BlockSize, Settings.BlockSize));
        }

        private static void DrawNodeEllipse(Graphics canvas, Pen color, Node node)
        {
            canvas.DrawEllipse(color, node.X * Settings.BlockSize,
                    node.Y * Settings.BlockSize, Settings.BlockSize, Settings.BlockSize);
        }

        private static void FillNodeEllipse(Graphics canvas, Brush color, Node node)
        {
            canvas.FillEllipse(color, node.X * Settings.BlockSize,
                    node.Y * Settings.BlockSize, Settings.BlockSize, Settings.BlockSize);
        }

        private void picCanvas_Paint(object sender, PaintEventArgs e)
        {
            picCanvas.Width = picCanvas.Height = Settings.GridSize * Settings.BlockSize;
            Graphics canvas = e.Graphics;
            //Reverse the axis of the drawing surface
            Matrix mx = new Matrix(1, 0, 0, -1, 0, picCanvas.Height);
            canvas.Transform = mx;

            //draw nodes
            Brush nodeColor;
            foreach (Node x in Nodes)
            {
                int index = Nodes.IndexOf(x);
                try
                {
                    if (StateDomains.Count <= index)
                        nodeColor = Brushes.Transparent;
                    else if (StateDomains[index].Count == 0)
                        nodeColor = Brushes.Transparent;
                    else if (StateDomains[index].Count > 1)
                        nodeColor = Brushes.Black;
                    else
                        nodeColor = colorArr[StateDomains[index][0]];
                }
                catch
                {
                    nodeColor = Brushes.Transparent;
                }
                DrawNodeRectangle(canvas, nodeColor, x);
            }

            //Draw endpoints
            for (int i = 0; i < EndPoints.Count; i++)
            {
                Node x = EndPoints[i];
                nodeColor = colorArr[i / 2];
                FillNodeEllipse(canvas, nodeColor, x);
                DrawNodeEllipse(canvas, Pens.Black, x);
            }

            //draw grid lines
            Pen gridPen = Pens.Black;
            for (int x = 0; x < Settings.GridSize * Settings.BlockSize; x += Settings.BlockSize)
            {
                Point start = new Point(x, 0);
                Point end = new Point(x, Settings.GridSize * Settings.BlockSize);
                canvas.DrawLine(gridPen, start, end);
            }
            for (int y = 0; y < Settings.GridSize * Settings.BlockSize; y += Settings.BlockSize)
            {
                Point start = new Point(0, y);
                Point end = new Point(Settings.GridSize * Settings.BlockSize, y);
                canvas.DrawLine(gridPen, start, end);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (running)
                return;
            AStar.OpenList.Clear();
            AStar.ClosedList.Clear();
            AStar.Successors.Clear();
            AStar.Path.Clear();
            ConstraintsPerVar.Clear();
            VarDomains.Clear();
            StateDomainsBuffer.Clear();
            StateDomains.Clear();
            Constraints.Clear();
        }

        private void txtSpeed_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Settings.Speed = Convert.ToInt32(txtSpeed.Text);
                //tmrDraw.Interval = 1000 / Settings.Speed;
            }
            catch
            {
                txtSpeed.Text = "10";
            }
        }

        private void btnInput_Click(object sender, EventArgs e)
        {
            btnReset_Click(null, null);
            Nodes.Clear();
            Flows.Clear();
            DialogResult result = dlgOpenFile.ShowDialog();
            if (result == DialogResult.OK)
            {
                InputLines.Clear();
                using (StreamReader r = new StreamReader(dlgOpenFile.FileName))
                {
                    string line;
                    while ((line = r.ReadLine()) != null)
                    {
                        // Check line validity then add it to the list
                        InputLines.Add(line);
                    }
                }
                tryNumber = 1;
                parse();
            }
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            if (running)
                return;
            btnReset_Click(null, null);
            running = true;
            stopwatch.Restart();
            theradMain.RunWorkerAsync(null);
            //tmrTryAgain.Start();
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            Settings.BlockSize += 1;
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            if (Settings.BlockSize > 1)
                Settings.BlockSize -= 1;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            running = false;
        }

        ToolTip toolTip1 = new ToolTip();
        private void picCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            int clickX = e.X / Settings.BlockSize;
            int clickY = Math.Abs(e.Y - picCanvas.Height) / Settings.BlockSize;
            Node clickNode = new Node(clickX, clickY);
            int index = 0;
            if (StateDomains.Count > 0 && clickNode.IsContained(Nodes, ref index))
            {
                toolTip1.ShowAlways = true;
                //toolTip1.InitialDelay = 0;
                //toolTip1.ReshowDelay = 0;
                StringBuilder str = new StringBuilder();
                foreach (int x in StateDomains[index])
                    str.Append(x.ToString() + ", ");
                toolTip1.SetToolTip(picCanvas, str.ToString());
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            running = true;
        }

        private void tmrTryAgain_Tick(object sender, EventArgs e)
        {
            if (timerStandby)
            {
                timerStandby = false;
                tmrTryAgain.Interval = 90000;
                tmrTryAgain.Enabled = false;
                btnReset_Click(null, null);
                btnGo_Click(null, null);
            }
            else if (running)
            {
                tryNumber++;
                //change status so the working thread stops
                running = false;
                //wait until the working thread stops
                Settings.sortMode++;
                parse();
                timerStandby = true;
                tmrTryAgain.Interval = 2000;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.sortMode = (Settings.SortModes)comboBox1.SelectedIndex;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Settings.UseFlows = checkBox1.Checked;
        }
    }
}
