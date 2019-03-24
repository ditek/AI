/************************
 * @author Łukasz Jakowski
 * @since  08.04.2014 14:47
 * 
 ************************/

using System;
using System.Collections.Generic;

namespace AI_2048
{
    public class Game
    {
        public int[][] iBoard;
        public double Rating;

        public int iScore = 0, iBest = 0;

        private int addNum = 2;

        private static Random oR = new Random();

        public Boolean gameOver = false;

        private int iNewX, iNewY;

        public Boolean kTOP, kRIGHT, kBOTTOM, kLEFT;

        public Boolean bRender = true;

        const double log2 = 0.30102999566398119521373889472449;

        public Game()
        {
            this.iBoard = new int[4][];
            for (int i = 0; i < 4; i++)
                iBoard[i] = new int[4];
        }

        public Game(Game game)
        {
            int i, j;
            this.iBoard = new int[4][];
            for (i = 0; i < 4; i++)
            {
                iBoard[i] = new int[4];
                iBoard[i][0] = game.iBoard[i][0];
                iBoard[i][1] = game.iBoard[i][1];
                iBoard[i][2] = game.iBoard[i][2];
                iBoard[i][3] = game.iBoard[i][3];
                //for (j = 0; j < 4; j++)
                //    iBoard[i][j] = game.iBoard[i][j];
            }
            //for (int i = 0; i < 4; i++)
            //    iBoard[i] = (int[])game.iBoard[i].Clone();
        }

        /* ******************************************** */
        //Generate a new random block
        //At game beginning 'addNum' will be 2 adding 2 new blocks
        // after that it will 1 only if the board blocks are successfully moved
        public void Update()
        {
            checkGameOver();
            List<Tuple<int, int>> free = this.getFree();
            while (!gameOver && addNum > 0)
            {
                int index = oR.Next(0, free.Count - 1);
                iNewX = free[index].Item1;
                iNewY = free[index].Item2;
                iBoard[iNewX][iNewY] = oR.Next(0, 9) == 0 ? 4 : 2;
                --addNum;
            }
        }

        public bool moveBoard(MoveDir nDirection)
        {
            Boolean bAdd = false;       //flag indicating that a cell has been moved
            switch (nDirection)
            {
                case MoveDir.Up:
                    for (int i = 0; i < 4; i++)
                        for (int j = 0; j < 4; j++)
                            for (int k = j + 1; k < 4; k++)
                            {
                                if (iBoard[i][k] == 0)
                                    continue;
                                //[i,k] and [i,j] will collide after the move
                                //if they are equal combine them
                                if (iBoard[i][k] == iBoard[i][j])
                                {
                                    iBoard[i][j] *= 2;
                                    iScore += iBoard[i][j];
                                    iBoard[i][k] = 0;
                                    bAdd = true;
                                    break;
                                }
                                //if [i,j] is empty replace it with [i,k]
                                else if (iBoard[i][j] == 0)
                                {
                                    iBoard[i][j] = iBoard[i][k];
                                    iBoard[i][k] = 0;
                                    j--;
                                    bAdd = true;
                                    break;
                                }
                                else
                                    break;
                            }
                    break;
                case MoveDir.Right:
                    for (int j = 0; j < 4; j++)
                        for (int i = 3; i >= 0; i--)
                            for (int k = i - 1; k >= 0; k--)
                            {
                                if (iBoard[k][j] == 0)
                                    continue;
                                else if (iBoard[k][j] == iBoard[i][j])
                                {
                                    iBoard[i][j] *= 2;
                                    iScore += iBoard[i][j];
                                    iBoard[k][j] = 0;
                                    bAdd = true;
                                    break;
                                }
                                else
                                {
                                    if (iBoard[i][j] == 0 && iBoard[k][j] != 0)
                                    {
                                        iBoard[i][j] = iBoard[k][j];
                                        iBoard[k][j] = 0;
                                        i++;
                                        bAdd = true;
                                        break;
                                    }
                                    else if (iBoard[i][j] != 0)
                                    {
                                        break;
                                    }
                                }
                            }
                    break;
                case MoveDir.Down:
                    for (int i = 0; i < 4; i++)
                        for (int j = 3; j >= 0; j--)
                            for (int k = j - 1; k >= 0; k--)
                            {
                                if (iBoard[i][k] == 0)
                                    continue;
                                if (iBoard[i][k] == iBoard[i][j])
                                {
                                    iBoard[i][j] *= 2;
                                    iScore += iBoard[i][j];
                                    iBoard[i][k] = 0;
                                    bAdd = true;
                                    break;
                                }
                                else if (iBoard[i][j] == 0)
                                {
                                    iBoard[i][j] = iBoard[i][k];
                                    iBoard[i][k] = 0;
                                    j++;
                                    bAdd = true;
                                    break;
                                }
                                else
                                    break;
                            }
                    break;
                case MoveDir.Left:
                    for (int j = 0; j < 4; j++)
                        for (int i = 0; i < 4; i++)
                            for (int k = i + 1; k < 4; k++)
                            {
                                if (iBoard[k][j] == 0)
                                    continue;
                                else if (iBoard[k][j] == iBoard[i][j])
                                {
                                    iBoard[i][j] *= 2;
                                    iScore += iBoard[i][j];
                                    iBoard[k][j] = 0;
                                    bAdd = true;
                                    break;
                                }
                                else
                                {
                                    if (iBoard[i][j] == 0 && iBoard[k][j] != 0)
                                    {
                                        iBoard[i][j] = iBoard[k][j];
                                        iBoard[k][j] = 0;
                                        i--;
                                        bAdd = true;
                                        break;
                                    }
                                    else if (iBoard[i][j] != 0)
                                    {
                                        break;
                                    }
                                }
                            }
                    break;
            }

            if (iScore > iBest)
                iBest = iScore;

            if (bAdd)
                ++addNum;

            /* ----- GAME OVER ----- */

            checkGameOver();
            bRender = true;
            return bAdd;
        }

        public void checkGameOver()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (iBoard[i][j] == 0)
                        return;
                    if (i - 1 >= 0)
                        if (iBoard[i - 1][j] == iBoard[i][j])
                            return;
                    if (i + 1 < 4)
                        if (iBoard[i + 1][j] == iBoard[i][j])
                            return;
                    if (j - 1 >= 0)
                        if (iBoard[i][j - 1] == iBoard[i][j])
                            return;
                    if (j + 1 < 4)
                        if (iBoard[i][j + 1] == iBoard[i][j])
                            return;
                }
            }
            gameOver = true;
        }

        public void resetGameData()
        {
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    this.iBoard[i][j] = 0;
            this.addNum = 2;
            this.iScore = 0;
            this.gameOver = false;
            this.bRender = true;
        }

        public List<StateTransition> getAllMoveStates()
        {
            List<StateTransition> allMoves = new List<StateTransition>();
            Game next;

            next = new Game(this);
            if (next.moveBoard(MoveDir.Right))
                allMoves.Add(new StateTransition(next, MoveDir.Right));

            next = new Game(this);
            if (next.moveBoard(MoveDir.Left))
                allMoves.Add(new StateTransition(next, MoveDir.Left));

            next = new Game(this);
            if (next.moveBoard(MoveDir.Up))
                allMoves.Add(new StateTransition(next, MoveDir.Up));

            next = new Game(this);
            if (next.moveBoard(MoveDir.Down))
                allMoves.Add(new StateTransition(next, MoveDir.Down));

            return allMoves;
        }

        public List<Game> getAllRandom()
        {
            List<Game> res = new List<Game>();
            List<Tuple<int, int>> free = this.getFree();

            foreach (Tuple<int, int> x in free)
            {
                Game next;
                next = new Game(this);
                next.iBoard[x.Item1][x.Item2] = 2;
                //next.iBoard[x.Item1][x.Item2] = oR.Next(0, 9) == 0 ? 4 : 2;
                res.Add(next);

                next = new Game(this);
                next.iBoard[x.Item1][x.Item2] = 4;
                res.Add(next);
            }

            return res;
        }

        //Returns the indices of all free blocks
        private List<Tuple<int, int>> getFree()
        {
            List<Tuple<int, int>> free = new List<Tuple<int, int>>();
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    if (this.iBoard[i][j] == 0)
                        free.Add(new Tuple<int, int>(i, j));

            return free;
        }

        public Tuple<int, int> MaxValue(out int maxVal)
        {
            Tuple<int, int> maxIndices = new Tuple<int, int>(0, 0);
            maxVal = 0;
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    if (this.iBoard[i][j] > maxVal)
                    {
                        maxVal = this.iBoard[i][j];
                        maxIndices = new Tuple<int, int>(i, j);
                    }
            return maxIndices;
        }

        private double getMonotonicity()
        {
            //Monotonicity
            double[] totals = { 0, 0, 0, 0 };
            // up/down direction
            for (int i = 0; i < 4; i++)
            {
                int current = 0, next = 1;
                double currentVal = 0, nextVal = 0;
                while (next < 4)
                {
                    while (next < 4 && iBoard[i][next] == 0)
                        next++;
                    if (next == 4) next--;
                    if (iBoard[i][current] > 0)
                        currentVal = getLog(iBoard[i][current]);
                    if (iBoard[i][next] > 0)
                        nextVal = getLog(iBoard[i][next]);
                    if (currentVal > nextVal)
                        totals[0] += nextVal - currentVal;
                    else if (nextVal > currentVal)
                        totals[1] += currentVal - nextVal;
                    current = next;
                    next++;
                }
            }

            // left/right direction
            for (int j = 0; j < 4; j++)
            {
                int current = 0, next = 1;
                double currentVal = 0, nextVal = 0;
                while (next < 4)
                {
                    while (next < 4 && iBoard[next][j] == 0)
                        next++;
                    if (next == 4) next--;
                    if (iBoard[current][j] > 0)
                        currentVal = getLog(iBoard[current][j]);
                    if (iBoard[next][j] > 0)
                        nextVal = getLog(iBoard[next][j]);
                    if (currentVal > nextVal)
                        totals[2] += nextVal - currentVal;
                    else if (nextVal > currentVal)
                        totals[3] += currentVal - nextVal;
                    current = next;
                    next++;
                }
            }
            return Math.Max(totals[0], totals[1]) + Math.Max(totals[2], totals[3]);
        }

        public static Dictionary<int, int> logDict = new Dictionary<int, int>
        {{2,1},{4,2},{8,3},{16,4},{32,5},
        {64,6},{128,7},{256,8},{512,9},{1024,10},{2048,11},{4096,12}};

        private double getLog(double num)
        {
            return logDict[(int)num];
            switch ((int)num)
            {
                case 2:
                    return 1;
                case 4:
                    return 2;
                case 8:
                    return 3;
                case 16:
                    return 4;
                case 32:
                    return 5;
                case 64:
                    return 6;
                case 128:
                    return 7;
                case 256:
                    return 8;
                case 512:
                    return 9;
                case 1024:
                    return 10;
                case 2048:
                    return 11;
                default:
                    return Math.Log((int)num) / log2;
            }
        }

        private double getSmoothness()
        {
            double smoothness = 0;
            //Smoothing
            //for (int i = 0; i < 4; i++)
            //    for (int j = 0; j < 3; j++)
            //        Rating -= Math.Abs(iBoard[i][j] - iBoard[i][j + 1]) / 256;
            //        if (iBoard[i][j] !=0 && iBoard[i][j + 1]!=0)
            //Rating -= (iBoard[i][j] >= iBoard[i][j + 1] ? iBoard[i][j] / iBoard[i][j + 1] : iBoard[i][j+1] / iBoard[i][j]);
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (iBoard[i][j] != 0)
                    {
                        double value = getLog(iBoard[i][j]);
                        double nextCell1 = iBoard[i][j + 1];
                        if (nextCell1 != 0)
                        {
                            nextCell1 = getLog(nextCell1);
                            smoothness -= Math.Abs(value - nextCell1);
                        }
                        double nextCell2 = iBoard[i + 1][j];
                        if (nextCell2 != 0)
                        {
                            nextCell2 = getLog(nextCell2);
                            smoothness -= Math.Abs(value - nextCell2);
                        }
                    }
            return smoothness;
        }

        public double calcRating()
        {
            Rating = 0;
            int maxVal, emptyCells = 0;
            double smoothness = 0, monotonicity = 0, emptyCellsVal;
            double emptyWeight = 2.7, smoothWeight = 0.1, maxWeight = 1.0, monoWeight = 1;

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    if (this.iBoard[i][j] == 0)
                        emptyCells += 1;
            Tuple<int, int> maxIndices = MaxValue(out maxVal);
            //Rating += 18 - 3 * (maxIndices.Item1 + maxIndices.Item2);
            //int blockSum = iBoard[0][0] + iBoard[0][1] + iBoard[1][0] + iBoard[1][1];
            //double blockScore = blockSum / maxVal * 10;
            //Rating += blockScore;
            ////Rating += maxVal / 100;

            //if (maxVal == 2048)
            //    Rating += 100;
            ////Ordering
            //for (int i = 0; i < 4; i++)
            //    for (int j = 0; j < 3; j++)
            //    {
            //            if (iBoard[i][j] >= iBoard[i][j + 1])
            //                Rating+=1;
            //            else
            //                Rating-=1;
            //    }

            smoothness = getSmoothness();
            monotonicity = getMonotonicity();
            emptyCellsVal = Math.Log(emptyCells);
            Rating = 0;
            Rating += smoothness * smoothWeight;
            Rating += emptyCellsVal * emptyWeight;
            Rating += maxVal * maxWeight;
            Rating += monotonicity * monoWeight;
            Rating += 4 * (maxIndices.Item1 + maxIndices.Item2);

            return Rating;
        }

        public static double minimax(Game root, int depth, bool player)
        {
            double curRating;
            if (depth == 0)
                return root.calcRating();
            //Max phase
            if (player)
            {
                double maxRating = 0;
                List<StateTransition> moves = root.getAllMoveStates();
                if (moves.Count == 0)
                    return -100;
                StateTransition selectedMove = moves[0];
                foreach (StateTransition move in moves)
                {
                    move.rating = Game.minimax(move.nextState, depth - 1, !player);
                    maxRating = Math.Max(maxRating, move.rating);
                }
                return maxRating;
            }
            //Min phase
            else
            {
                double minRating = 10000;
                List<Game> rnd = root.getAllRandom();
                foreach (Game g in rnd)
                {
                    curRating = Game.minimax(g, depth - 1, !player);
                    minRating = Math.Min(minRating, curRating);
                }
                return minRating;
            }
        }

        public static double alphabetarate(Game root, int depth, double alpha, double beta, bool player)
        {
            if (depth == 0)
                return root.calcRating();
            //Max phase
            if (player)
            {
                List<StateTransition> moves = root.getAllMoveStates();
                if (moves.Count == 0)
                    return -100;
                StateTransition selectedMove = moves[0];
                foreach (StateTransition move in moves)
                {
                    alpha = Math.Max(alpha, Game.alphabetarate(move.nextState, depth - 1, alpha, beta, !player));
                    if (beta <= alpha)
                        break;
                }
                return alpha;
            }
            //Min phase
            else
            {
                List<Game> rnd = root.getAllRandom();
                foreach (Game g in rnd)
                {
                    beta = Math.Min(beta, Game.alphabetarate(g, depth - 1, alpha, beta, !player));
                    if (beta <= alpha)
                        break;
                }
                return beta;
            }
        }

        public static double expectimax(Game root, int depth, bool player)
        {
            double curRating;
            if (depth == 0)
                return root.calcRating();
            //Max phase
            if (player)
            {
                double maxRating = 0;
                List<StateTransition> moves = root.getAllMoveStates();
                if (moves.Count == 0)
                    return -100;
                StateTransition selectedMove = moves[0];
                foreach (StateTransition move in moves)
                {
                    move.rating = Game.expectimax(move.nextState, depth - 1, !player);
                    maxRating = Math.Max(maxRating, move.rating);
                }
                return maxRating;
            }
            //Min phase
            else
            {
                double minRating = 10000;
                double probability;
                List<Game> rnd = root.getAllRandom();
                foreach (Game g in rnd)
                {
                    probability = (rnd.IndexOf(g) % 2 == 0) ? 0.9F : 0.1F;
                    curRating = probability * Game.expectimax(g, depth - 1, !player);
                    minRating = Math.Min(minRating, curRating);
                }
                return minRating;
            }
        }

    }
}
