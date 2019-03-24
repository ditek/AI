using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;

namespace AI_2048
{
    public partial class Main : Form
    {
        private Game oGame;
        int depth;
        private Graphics gGraphics, gG;
        private Bitmap bBackground;

        bool AIRunning = false;

        public Main()
        {
            InitializeComponent();

            bBackground = new Bitmap(396, 600);

            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            gGraphics = this.CreateGraphics();
            gG = Graphics.FromImage(bBackground);

            oGame = new Game();
            UpdateGame();
            Drawer.init();
            cboMode.SelectedIndex = 1;
            cboDepth.SelectedIndex = 4;
        }

        /* ******************************************** */

        public void UpdateGame()
        {
            oGame.Update();
        }

        public void Draw(Graphics g)
        {
            g.Clear(Color.FromArgb(251, 248, 239));
            Drawer.Draw(g, oGame);
        }

        /* ******************************************** */

        private void timer1_Tick(object sender, EventArgs e)
        {
            //UpdateGame();
            if (oGame.bRender)
            {
                Draw(gG);
                gGraphics.DrawImage(bBackground, new Point(0, 0));
            }
            lblScore.Text = oGame.iScore.ToString();
        }

        /* ******************************************** */

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!oGame.kTOP && !oGame.kRIGHT && !oGame.kBOTTOM && (e.KeyCode == Keys.A || e.KeyCode == Keys.Left))
            {
                oGame.kLEFT = true;
                oGame.moveBoard(MoveDir.Left);
            }
            else if (!oGame.kLEFT && !oGame.kRIGHT && !oGame.kBOTTOM && (e.KeyCode == Keys.W || e.KeyCode == Keys.Up))
            {
                oGame.kTOP = true;
                oGame.moveBoard(MoveDir.Up);
            }
            else if (!oGame.kTOP && !oGame.kLEFT && !oGame.kBOTTOM && (e.KeyCode == Keys.D || e.KeyCode == Keys.Right))
            {
                oGame.kRIGHT = true;
                oGame.moveBoard(MoveDir.Right);
            }
            else if (!oGame.kTOP && !oGame.kRIGHT && !oGame.kLEFT && (e.KeyCode == Keys.S || e.KeyCode == Keys.Down))
            {
                oGame.kBOTTOM = true;
                oGame.moveBoard(MoveDir.Down);
            }
            else if (e.KeyCode == Keys.Z)
                lblAI_Click(null, null);
            else if (e.KeyCode == Keys.X)
                lblAIStart_Click(null, null);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (oGame.kLEFT && (e.KeyCode == Keys.A || e.KeyCode == Keys.Left))
            {
                oGame.kLEFT = false;
            }

            if (oGame.kTOP && (e.KeyCode == Keys.W || e.KeyCode == Keys.Up))
            {
                oGame.kTOP = false;
            }

            if (oGame.kRIGHT && (e.KeyCode == Keys.D || e.KeyCode == Keys.Right))
            {
                oGame.kRIGHT = false;
            }

            if (oGame.kBOTTOM && (e.KeyCode == Keys.S || e.KeyCode == Keys.Down))
            {
                oGame.kBOTTOM = false;
            }
        }

        private void btnNewGame_Click(object sender, EventArgs e)
        {
            oGame.resetGameData();
            UpdateGame();
        }

        void AI_Step()
        {
            List<StateTransition> moves = oGame.getAllMoveStates();
            if (moves.Count == 0)
            {
                oGame.gameOver = true;
                return;
            }
            StateTransition selectedMove = moves[0];
            //int depth = cboDepth.SelectedIndex;
            foreach (StateTransition move in moves)
            {
                switch (currentAlg)
                {
                    case Algorithm.MiniMax:
                        move.rating = Game.minimax(move.nextState, depth, false);
                        break;
                    case Algorithm.AlphaBeta:
                        move.rating = Game.alphabetarate(move.nextState, depth, float.MinValue, float.MaxValue, false);
                        break;
                    case Algorithm.ExpectiMax:
                        move.rating = Game.expectimax(move.nextState, depth, false);
                        break;
                }

                if (selectedMove.rating < move.rating)
                    selectedMove = move;
            }
            oGame.moveBoard(selectedMove.dir);
            UpdateGame();
        }

        private void lblAI_Click(object sender, EventArgs e)
        {
            AI_Step();
        }

        private void lblAIStart_Click(object sender, EventArgs e)
        {
            if (!AIRunning)
            {
                AIRunning = true;
                lblAIStart.Text = "Stop AI";
                bgWorker1.RunWorkerAsync();
            }
            else
            {
                AIRunning = false;
                lblAIStart.Text = "Start AI";
                bgWorker1.CancelAsync();
            }

        }

        private void bgWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            while (AIRunning && !oGame.gameOver)
            {
                if ((worker.CancellationPending == true))
                {
                    e.Cancel = true;
                    break;
                }
                AI_Step();
            }
            AIRunning = false;
        }
        enum Algorithm { MiniMax, AlphaBeta, ExpectiMax } ;
        Algorithm currentAlg = Algorithm.MiniMax;

        private void cboMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cboMode.SelectedItem.ToString())
            {
                case "MiniMax":
                    currentAlg = Algorithm.MiniMax; break;
                case "AlphaBeta":
                    currentAlg = Algorithm.AlphaBeta; break;
                case "ExpectiMax":
                    currentAlg = Algorithm.ExpectiMax; break;
            }
        }

        private void bgWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lblAIStart.Text = "Start AI";
        }

        private void cboDepth_SelectedIndexChanged(object sender, EventArgs e)
        {
            depth = cboDepth.SelectedIndex;
        }
    }
}
