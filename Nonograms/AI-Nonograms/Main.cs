using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace AI_Nonograms
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

        private List<string> InputLines = new List<string>();
        public static List<Node> Nodes = new List<Node>();
        //Stores the current variables values when revising a constraint
        //(Equivalent to eContext.Variables in dynamic functions)
        private static List<int> VarsValues = new List<int>(100);
        private static List<List<Pattern>> ColDomains = new List<List<Pattern>>();
        private List<Label> labels = new List<Label>();
        List<List<int>> SpecsRows = new List<List<int>>();
        List<List<int>> SpecsCols = new List<List<int>>();
        List<Pattern> comColPatterns = new List<Pattern>();
        List<Pattern> comRowPatterns = new List<Pattern>();

        private int numRows { get { return Settings.GridHeight; } set { numRows = value; } }
        private int numCols { get { return Settings.GridWidth; } set { numCols = value; } }
        private int rowSize { get { return numCols; } set { rowSize = value; } }
        private int colSize { get { return numRows; } set { colSize = value; } }

        //Pre-created variables to avoid runtime creation
        static int tempInt;

        /** GAC specific declarations **/
        //Lists required for the algorithm
        private static List<List<Pattern>> VarDomains = new List<List<Pattern>>();
        public static List<List<Pattern>> StateDomains = new List<List<Pattern>>();
        private static List<List<Pattern>> StateDomainsBuffer = new List<List<Pattern>>();
        public static List<Constraint> Constraints = new List<Constraint>();
        public static List<int> ConstraintsPerVar = new List<int>();
        private static Queue<Instance> ReviseQ = new Queue<Instance>();
        //Struct containing a variable instance, constrain instance pair
        private struct Instance
        {
            public int var;
            public Constraint constraint;
        };

        //The constraint function called by ReviseRecursive()
        static bool ConstraintFunction(Constraint c)
        {
            foreach (Pattern p in ColDomains[c.FuncParam])
                if (Pattern.IsEqual(p, VarsValues))
                    return true;
            return false;
        }

        //Create all possible patterns from the given specs
        List<Pattern> PatternsFromSpecs(List<int> patternSpec, int patternSize)
        {
            //Create a list of sigments and set it up
            List<Segment> segmentsList = new List<Segment>();
            foreach (int segmentSize in patternSpec)
                segmentsList.Add(new Segment(segmentSize));
            int startPtr = 0;
            int[] ASAPStart = new int[patternSpec.Count];
            int[] ALAPStart = new int[patternSpec.Count];
            for (int i = 0; i < patternSpec.Count; i++)
            {
                ASAPStart[i] = startPtr;
                //Shift the pointer by block size
                startPtr += patternSpec[i];
                //Shift it by one for an empty slot
                startPtr++;
            }
            //Now we calculate the max start point of each block
            //first we put the pointer at the end of the pattern
            startPtr = patternSize;
            for (int i = patternSpec.Count - 1; i >= 0; i--)
            {
                //Shift the pointer back by block size
                startPtr -= patternSpec[i];
                ALAPStart[i] = startPtr;
                //Shift it by one for an empty slot
                startPtr--;
            }
            for (int segmentIndex = 0; segmentIndex < patternSpec.Count; segmentIndex++)
            {
                List<int> startDomain = new List<int>();
                for (int start = ASAPStart[segmentIndex]; start <= ALAPStart[segmentIndex]; start++)
                    startDomain.Add(start);
                segmentsList[segmentIndex].StartDomain = startDomain.ToList();
            }

            List<Pattern> patternsList = new List<Pattern>();
            createPatterns(segmentsList, 0, patternsList, patternSize);
            return patternsList;
        }

        //Recursive function called by PatternsFromSpecs()
        void createPatterns(List<Segment> segmentList, int nextSegIndex, List<Pattern> patterns, int patternSize)
        {
            if (nextSegIndex < segmentList.Count)
                foreach (int value in segmentList[nextSegIndex].StartDomain)
                {
                    List<Segment> segList = new List<Segment>();
                    foreach (Segment x in segmentList)
                        segList.Add(new Segment(x));
                    Segment currentSeg = segList[nextSegIndex];
                    currentSeg.StartDomain.Clear();
                    currentSeg.StartDomain.Add(value);
                    //Check if current value will give a valid pattern
                    if (nextSegIndex > 0)
                    {
                        Segment prevSeg = segList[nextSegIndex - 1];
                        if (value <= prevSeg.StartDomain[0] + prevSeg.Size)
                            continue;
                    }
                    createPatterns(segList, nextSegIndex + 1, patterns, patternSize);
                }
            else
                patterns.Add(createPattern(segmentList, patternSize));
        }

        //Create a single pattern. Called by createPatterns()
        Pattern createPattern(List<Segment> segmentList, int patternSize)
        {
            Pattern pattern = new Pattern(patternSize);
            for (int i = 0; i < patternSize; i++)
                pattern.Add(0);
            foreach (Segment seg in segmentList)
                for (int i = seg.StartDomain[0]; i < seg.StartDomain[0] + seg.Size; i++)
                    pattern[i] = 1;
            return pattern;
        }

        //Filter rows based on columns common 1's
        void filterRowsOnes()
        {
            //-------- Find common column patterns and filter rows -----------//
            //For each column create a domain  that contains the common 1's in its domains
            Pattern comPattern = new Pattern(colSize);
            comColPatterns.Clear();
            foreach (List<Pattern> colPatterns in ColDomains)
            {
                comPattern.Clear();
                //Set up the common pattern with 1's
                for (int j = 0; j < colSize; j++)
                    comPattern.Add(1);
                //AND all patterns to find the common 1's
                foreach (Pattern p in colPatterns)
                    comPattern &= p;
                comColPatterns.Add(new Pattern(comPattern));
            }
            //Remove the row patterns that don't match the common column cell
            List<Pattern> toRemove = new List<Pattern>();
            for (int i = 0; i < numRows; i++)
            {
                toRemove.Clear();
                foreach (Pattern rowPattern in VarDomains[i])
                    for (int j = 0; j < rowSize; j++)
                        if (rowPattern[j] < comColPatterns[j][i])
                        {
                            toRemove.Add(rowPattern);
                            break;
                        }
                //foreach (Pattern p in toRemove)
                //VarDomains[i].Remove(p);
                HashSet<Pattern> toRemoveHash = new HashSet<Pattern>(toRemove);
                VarDomains[i].RemoveAll(item => toRemoveHash.Contains(item));
            }
            //----------------------------------------------------------------//
        }

        //Filter rows based on columns common 0's
        void filterRowsZeros()
        {
            //-------- Find common column patterns and filter rows -----------//
            //For each column create a domain  that contains the common 0's in its domains
            Pattern comPattern = new Pattern(colSize);
            comColPatterns.Clear();
            foreach (List<Pattern> colPatterns in ColDomains)
            {
                comPattern.Clear();
                //Set up the common pattern with 0's
                for (int j = 0; j < colSize; j++)
                    comPattern.Add(0);
                //OR all patterns to find the common 0's
                foreach (Pattern p in colPatterns)
                    comPattern |= p;
                comColPatterns.Add(new Pattern(comPattern));
            }
            //Remove the row patterns that don't match the common column cell
            List<Pattern> toRemove = new List<Pattern>();
            for (int i = 0; i < numRows; i++)
            {
                toRemove.Clear();
                foreach (Pattern rowPattern in VarDomains[i])
                    for (int j = 0; j < rowSize; j++)
                        if (rowPattern[j] > comColPatterns[j][i])
                        {
                            toRemove.Add(rowPattern);
                            break;
                        }
                //foreach (Pattern p in toRemove)
                //    VarDomains[i].Remove(p);
                HashSet<Pattern> toRemoveHash = new HashSet<Pattern>(toRemove);
                VarDomains[i].RemoveAll(item => toRemoveHash.Contains(item));
            }
            //----------------------------------------------------------------//
        }

        //Filter columns based on rows common 1's
        void filterColsOnes()
        {
            //-------- Find common column patterns and filter rows -----------//
            //For each row create a domain  that contains the common 1's in its domains
            Pattern comRowPattern = new Pattern(rowSize);
            comRowPatterns.Clear();
            foreach (List<Pattern> rowPatterns in VarDomains)
            {
                comRowPattern.Clear();
                //Set up the common pattern with 1's
                for (int j = 0; j < rowSize; j++)
                    comRowPattern.Add(1);
                //AND all patterns to find the common 1's
                foreach (Pattern p in rowPatterns)
                    comRowPattern &= p;
                comRowPatterns.Add(new Pattern(comRowPattern));
            }
            //Remove the row patterns that don't match the common column cell
            List<Pattern> toRemove = new List<Pattern>();
            for (int i = 0; i < numCols; i++)
            {
                toRemove.Clear();
                foreach (Pattern colPattern in ColDomains[i])
                    for (int j = 0; j < colSize; j++)
                        if (colPattern[j] < comRowPatterns[j][i])
                        {
                            toRemove.Add(colPattern);
                            break;
                        }
                //foreach (Pattern p in toRemove)
                //    ColDomains[i].Remove(p);
                HashSet<Pattern> toRemoveHash = new HashSet<Pattern>(toRemove);
                ColDomains[i].RemoveAll(item => toRemoveHash.Contains(item));
            }
            //----------------------------------------------------------------//
        }

        //Filter columns based on rows common 0's
        void filterColsZeros()
        {
            //-------- Find common column patterns and filter rows -----------//
            //For each row create a domain  that contains the common 0's in its domains
            Pattern comRowPattern = new Pattern(rowSize);
            comRowPatterns.Clear();
            foreach (List<Pattern> rowPatterns in VarDomains)
            {
                comRowPattern.Clear();
                //Set up the common pattern with 0's
                for (int j = 0; j < rowSize; j++)
                    comRowPattern.Add(0);
                //AND all patterns to find the common 0's
                foreach (Pattern p in rowPatterns)
                    comRowPattern |= p;
                comRowPatterns.Add(new Pattern(comRowPattern));
            }
            //Remove the row patterns that don't match the common column cell
            List<Pattern> toRemove = new List<Pattern>();
            for (int i = 0; i < numCols; i++)
            {
                toRemove.Clear();
                foreach (Pattern colPattern in ColDomains[i])
                    for (int j = 0; j < colSize; j++)
                        if (colPattern[j] > comRowPatterns[j][i])
                        {
                            toRemove.Add(colPattern);
                            break;
                        }
                //foreach (Pattern p in toRemove)
                //    ColDomains[i].Remove(p);
                HashSet<Pattern> toRemoveHash = new HashSet<Pattern>(toRemove);
                ColDomains[i].RemoveAll(item => toRemoveHash.Contains(item));
            }
            //----------------------------------------------------------------//
        }

        //Sets up initial var domains, evaluation expression and ReviseQ
        private void init()
        {
            VarDomains.Clear();
            Constraints.Clear();
            ReviseQ.Clear();
            VarsValues.Clear();
            ColDomains.Clear();
            comColPatterns.Clear();
            comRowPatterns.Clear();
            //Fill variable space with each variable's domain
            //Create the set of patterns for each variable (row/col)
            foreach (List<int> spec in SpecsRows)
                VarDomains.Add(PatternsFromSpecs(spec, rowSize));
            foreach (List<int> spec in SpecsCols)
                ColDomains.Add(PatternsFromSpecs(spec, colSize));
            //Filtering stage 1
            filterRowsOnes();
            filterColsOnes();
            filterRowsZeros();
            filterColsZeros();
            //Filtering stage 2
            filterRowsOnes();
            filterColsOnes();
            filterRowsZeros();
            filterColsZeros();

            /********************General GAC operations, only minor changes required********************/
            //Set up VarsValues list
            for (int i = 0; i < numRows; i++)
                VarsValues.Add(0);

            //Add a constraint to constraints list for each column
            for (int j = 0; j < numCols; j++)
            {
                Constraint constraint = new Constraint();
                constraint.Vars = new List<int>();
                //Add all rows indeces to the vars list
                for (int i = 0; i < numRows; i++)
                    constraint.Vars.Add(i);
                constraint.FuncParam = j;
                Constraints.Add(constraint);
            }

            //Create a list to hold the number of constrains that a var apears in
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

        private static bool ReviseRecursion(Instance reviseElement, List<List<Pattern>> Domains, List<int> VarsList)
        {
            int var = VarsList[0];
            //Foreach value in the non-focal var domain:
            foreach (Pattern varValue in Domains[var])
            {
                VarsValues[var] = varValue[reviseElement.constraint.FuncParam];
                List<int> newVarsList = new List<int>(VarsList);// VarsList.Where(item => item != var).ToList();
                newVarsList.Remove(var);
                if (newVarsList.Count > 0)
                {
                    if (ReviseRecursion(reviseElement, Domains, newVarsList))
                        return true;
                }
                else
                {
                    tempInt++;
                    if (ConstraintFunction(reviseElement.constraint))
                        return true;
                }
            }
            return false;
        }

        //Takes a var-constrain pair and updates the var domain
        private void Revise(Instance reviseElement, List<List<Pattern>> Domains)
        {
            int focalVar = reviseElement.var;
            List<Pattern> ToRemove = new List<Pattern>();

            //Check each value of the focal variable against the constrain
            foreach (Pattern focalValue in Domains[focalVar])
            {
                //Put the value in row 'focalVar' that corresponds to the constraint column in VarsValues
                VarsValues[focalVar] = focalValue[reviseElement.constraint.FuncParam];
                //Foreach non-focal variable in constrain variables
                List<int> nonFocalVars = reviseElement.constraint.Vars.Where(item => item != focalVar).ToList();
                //Remove the value that doesn't fulfill the constrain from variable domain 
                if (!ReviseRecursion(reviseElement, Domains, nonFocalVars))
                    ToRemove.Add(focalValue);
            }
            //Execute the removal
            foreach (Pattern x in ToRemove)
                Domains[focalVar].Remove(x);
        }

        //Calls Revise for each item in ReviseQ and pushs new items for vars with modified domains
        /* (while RQ.Count!=0):
         * Pop a revise request (Xi,Ci)
         * Call Revise 
         * If domain got reduced: push (Xj,Cj) for each Cj!=Ci where Xi appears and each Xj!=Xi */
        private void DomainFilterLoop(List<List<Pattern>> Domains)
        {
            tempInt = 0;
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
                    foreach (Constraint constraint in Constraints.Where(item => !Constraint.IsEqual(item, reviseInstance.constraint) && item.Vars.Contains(reviseVar)))
                        foreach (int x in constraint.Vars.Where(item => item != reviseVar))
                        {
                            Instance new_rev;
                            new_rev.var = x;
                            new_rev.constraint = constraint;
                            ReviseQ.Enqueue(new_rev);
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
            //StateDomains = VarDomains.ToList();
            //This is done just for drawing convinience. Normally the prev. statement is used
            StateDomains = VarDomains;
            //Load the queue with REVISE requests
            init();
            //Call DomainFilterLoop for the initial variables domain
            DomainFilterLoop(StateDomains);
            //A buffer for the search state
            StateDomainsBuffer = StateDomains.ToList();
            //Check if the problem is solvable
            foreach (List<Pattern> domain in StateDomains)
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
            //txtResult.Text = tempInt.ToString();
            txtTime.Text = stopwatch.ElapsedMilliseconds.ToString();
            running = false;
        }

        //Returns the var assumed by A*
        int getAssumedVar()
        {
            for (int i = 0; i < StateDomains.Count; i++)
                if (StateDomains[i].Count != StateDomainsBuffer[i].Count)
                    return i;
            //Nothing has been changed
            return -1;
        }

        //Converts the read file into nodes and edges
        /* Input format:
         * [Num. cols.] [Num. rows]
         * [Row 0 specs]
         * ...
         * [Row n-1 specs]
         * [Col 0 specs]
         * ...
         * [Col m-1 specs]
         */
        void parse()
        {
            int num;
            SpecsRows.Clear();
            SpecsCols.Clear();
            Nodes.Clear();
            foreach (Label lbl in labels)
                this.Controls.Remove(lbl);
            labels.Clear();
            //Read dimentions
            string[] parameters = InputLines[0].Split(' ');
            Settings.GridWidth = Convert.ToInt32(parameters[0]);
            Settings.GridHeight = Convert.ToInt32(parameters[1]);
            //Add rows and columns specs
            //for (int i = 1; i <= numRows; i++)
            //Reversed to get data in correct order for processing
            for (int i = numRows; i > 0; i--)
            {
                List<int> Spec = new List<int>();
                parameters = InputLines[i].Split(' ');
                foreach (string str in parameters)
                {
                    if (Int32.TryParse(str, out num))
                        Spec.Add(num);
                    else
                        MessageBox.Show("Check your input at line " + i.ToString());
                }
                SpecsRows.Add(Spec);
                //Add a label to show row specs
                Label lblSpec = new Label();
                lblSpec.AutoSize = true;
                lblSpec.Location = new System.Drawing.Point(0, picCanvas.Top + Math.Abs(i - numRows) * Settings.BlockSize + Settings.BlockSize / 2);
                lblSpec.Text = InputLines[i];
                this.Controls.Add(lblSpec);
                labels.Add(lblSpec);
            }
            for (int i = numRows + 1; i <= numRows + numCols; i++)
            {
                List<int> Spec = new List<int>();
                parameters = InputLines[i].Split(' ');
                foreach (string str in parameters)
                {
                    if (Int32.TryParse(str, out num))
                        Spec.Add(num);
                    else
                        MessageBox.Show("Check your input at line " + i.ToString());
                }
                SpecsCols.Add(Spec);
                //Add a label to show column specs
                Label lblSpec = new Label();
                lblSpec.AutoSize = false;
                lblSpec.Size = new System.Drawing.Size(19, 13 * InputLines[i].Length / 2);
                lblSpec.Size = new System.Drawing.Size(19, picCanvas.Top);
                lblSpec.Location = new System.Drawing.Point(picCanvas.Left + (i - numRows - 1) * Settings.BlockSize + Settings.BlockSize / 2, 0);
                lblSpec.Text = InputLines[i];
                this.Controls.Add(lblSpec);
                labels.Add(lblSpec);
            }

            //Add a node for each grid
            for (int y = 0; y < Settings.GridHeight; y++)
                for (int x = 0; x < Settings.GridHeight; x++)
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
            tmrDraw.Interval = 100;
            tmrDraw.Start();
            UpdateValues(null, null);
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
            picCanvas.Width = Settings.GridWidth * Settings.BlockSize;
            picCanvas.Height = Settings.GridHeight * Settings.BlockSize;
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
            picCanvas.Width = Settings.GridWidth * Settings.BlockSize;
            picCanvas.Height = Settings.GridHeight * Settings.BlockSize;
            Graphics canvas = e.Graphics;
            //Reverse the axis of the drawing surface
            //Matrix mx = new Matrix(1, 0, 0, -1, 0, picCanvas.Height);
            //canvas.Transform = mx;

            //Draw the current picture
            for (int y = 0; y < numRows && StateDomains.Count == numRows; y++)
                for (int x = 0; x < numCols; x++)
                {
                    if (StateDomains[y].Count == 0)
                        continue;
                    Brush blockColor = StateDomains[y][0][x] == 1 ? Brushes.Blue : Brushes.Transparent;
                    DrawNodeRectangle(canvas, blockColor, new Node(x, y));
                }

            //draw grid lines
            Pen gridPen = Pens.Black;
            for (int x = 0; x < Settings.GridWidth * Settings.BlockSize; x += Settings.BlockSize)
            {
                Point start = new Point(x, 0);
                Point end = new Point(x, Settings.GridHeight * Settings.BlockSize);
                canvas.DrawLine(gridPen, start, end);
            }
            for (int y = 0; y < Settings.GridHeight * Settings.BlockSize; y += Settings.BlockSize)
            {
                Point start = new Point(0, y);
                Point end = new Point(Settings.GridWidth * Settings.BlockSize, y);
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

        private void picCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            //ToolTip toolTip1 = new ToolTip();
            //int clickX = e.X / Settings.BlockSize;
            //int clickY = Math.Abs(e.Y - picCanvas.Height) / Settings.BlockSize;
            //Node clickNode = new Node(clickX, clickY);
            //int index = 0;
            //if (StateDomains.Count > 0 && clickNode.IsContained(Nodes, ref index))
            //{
            //    toolTip1.ShowAlways = true;
            //    toolTip1.SetToolTip(picCanvas, str.ToString());
            //}
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            running = true;
        }
    }
}
