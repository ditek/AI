namespace AI_2048
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lblNewGame = new System.Windows.Forms.Label();
            this.lblAIStep = new System.Windows.Forms.Label();
            this.lblAIStart = new System.Windows.Forms.Label();
            this.bgWorker1 = new System.ComponentModel.BackgroundWorker();
            this.cboMode = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cboDepth = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblScore = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // lblNewGame
            // 
            this.lblNewGame.AutoSize = true;
            this.lblNewGame.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNewGame.Font = new System.Drawing.Font("Tahoma", 8F);
            this.lblNewGame.Location = new System.Drawing.Point(12, 9);
            this.lblNewGame.Name = "lblNewGame";
            this.lblNewGame.Size = new System.Drawing.Size(60, 15);
            this.lblNewGame.TabIndex = 1;
            this.lblNewGame.Text = "New Game";
            this.lblNewGame.Click += new System.EventHandler(this.btnNewGame_Click);
            // 
            // lblAIStep
            // 
            this.lblAIStep.AutoSize = true;
            this.lblAIStep.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblAIStep.Font = new System.Drawing.Font("Tahoma", 8F);
            this.lblAIStep.Location = new System.Drawing.Point(78, 9);
            this.lblAIStep.Name = "lblAIStep";
            this.lblAIStep.Size = new System.Drawing.Size(45, 15);
            this.lblAIStep.TabIndex = 1;
            this.lblAIStep.Text = "AI Step";
            this.lblAIStep.Click += new System.EventHandler(this.lblAI_Click);
            // 
            // lblAIStart
            // 
            this.lblAIStart.AutoSize = true;
            this.lblAIStart.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblAIStart.Font = new System.Drawing.Font("Tahoma", 8F);
            this.lblAIStart.Location = new System.Drawing.Point(129, 9);
            this.lblAIStart.Name = "lblAIStart";
            this.lblAIStart.Size = new System.Drawing.Size(47, 15);
            this.lblAIStart.TabIndex = 1;
            this.lblAIStart.Text = "Start AI";
            this.lblAIStart.Click += new System.EventHandler(this.lblAIStart_Click);
            // 
            // bgWorker1
            // 
            this.bgWorker1.WorkerSupportsCancellation = true;
            this.bgWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgWorker1_DoWork);
            this.bgWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgWorker1_RunWorkerCompleted);
            // 
            // cboMode
            // 
            this.cboMode.FormattingEnabled = true;
            this.cboMode.Items.AddRange(new object[] {
            "MiniMax",
            "AlphaBeta",
            "ExpectiMax"});
            this.cboMode.Location = new System.Drawing.Point(302, 6);
            this.cboMode.Name = "cboMode";
            this.cboMode.Size = new System.Drawing.Size(82, 21);
            this.cboMode.TabIndex = 2;
            this.cboMode.SelectedIndexChanged += new System.EventHandler(this.cboMode_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(235, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Algorithm";
            // 
            // cboDepth
            // 
            this.cboDepth.FormattingEnabled = true;
            this.cboDepth.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5"});
            this.cboDepth.Location = new System.Drawing.Point(302, 33);
            this.cboDepth.Name = "cboDepth";
            this.cboDepth.Size = new System.Drawing.Size(82, 21);
            this.cboDepth.TabIndex = 2;
            this.cboDepth.SelectedIndexChanged += new System.EventHandler(this.cboDepth_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(235, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Depth";
            // 
            // lblScore
            // 
            this.lblScore.AutoSize = true;
            this.lblScore.Location = new System.Drawing.Point(129, 36);
            this.lblScore.Name = "lblScore";
            this.lblScore.Size = new System.Drawing.Size(35, 13);
            this.lblScore.TabIndex = 3;
            this.lblScore.Text = "Score";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(88, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Score";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(396, 600);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblScore);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cboDepth);
            this.Controls.Add(this.cboMode);
            this.Controls.Add(this.lblAIStart);
            this.Controls.Add(this.lblAIStep);
            this.Controls.Add(this.lblNewGame);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Main";
            this.Text = "2048";
            this.TopMost = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label lblNewGame;
        private System.Windows.Forms.Label lblAIStep;
        private System.Windows.Forms.Label lblAIStart;
        private System.ComponentModel.BackgroundWorker bgWorker1;
        private System.Windows.Forms.ComboBox cboMode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboDepth;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblScore;
        private System.Windows.Forms.Label label3;
    }
}

