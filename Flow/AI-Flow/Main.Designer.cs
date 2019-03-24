namespace AI_Flow
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
            this.picCanvas = new System.Windows.Forms.PictureBox();
            this.tmrDraw = new System.Windows.Forms.Timer(this.components);
            this.lblGameOver = new System.Windows.Forms.Label();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.btnReset = new System.Windows.Forms.Button();
            this.txtPathLength = new System.Windows.Forms.TextBox();
            this.txtPoppedNodes = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.txtSpeed = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.btnZoomIn = new System.Windows.Forms.Button();
            this.btnZoomOut = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnGo = new System.Windows.Forms.Button();
            this.btnInput = new System.Windows.Forms.Button();
            this.txtTreeNodes = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtTime = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.dlgOpenFile = new System.Windows.Forms.OpenFileDialog();
            this.theradMain = new System.ComponentModel.BackgroundWorker();
            this.tmrTryAgain = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.picCanvas)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // picCanvas
            // 
            this.picCanvas.BackColor = System.Drawing.SystemColors.ControlLight;
            this.picCanvas.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.picCanvas.Location = new System.Drawing.Point(0, 0);
            this.picCanvas.Name = "picCanvas";
            this.picCanvas.Size = new System.Drawing.Size(862, 662);
            this.picCanvas.TabIndex = 0;
            this.picCanvas.TabStop = false;
            this.picCanvas.Paint += new System.Windows.Forms.PaintEventHandler(this.picCanvas_Paint);
            // 
            // tmrDraw
            // 
            this.tmrDraw.Interval = 30;
            this.tmrDraw.Tick += new System.EventHandler(this.UpdateScreen);
            // 
            // lblGameOver
            // 
            this.lblGameOver.AutoSize = true;
            this.lblGameOver.Location = new System.Drawing.Point(299, 64);
            this.lblGameOver.Name = "lblGameOver";
            this.lblGameOver.Size = new System.Drawing.Size(0, 13);
            this.lblGameOver.TabIndex = 3;
            // 
            // txtResult
            // 
            this.txtResult.Enabled = false;
            this.txtResult.Location = new System.Drawing.Point(66, 117);
            this.txtResult.Name = "txtResult";
            this.txtResult.Size = new System.Drawing.Size(63, 20);
            this.txtResult.TabIndex = 7;
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(47, 281);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(57, 23);
            this.btnReset.TabIndex = 6;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // txtPathLength
            // 
            this.txtPathLength.Location = new System.Drawing.Point(107, 363);
            this.txtPathLength.Name = "txtPathLength";
            this.txtPathLength.Size = new System.Drawing.Size(46, 20);
            this.txtPathLength.TabIndex = 22;
            this.txtPathLength.Tag = "1";
            // 
            // txtPoppedNodes
            // 
            this.txtPoppedNodes.Location = new System.Drawing.Point(107, 337);
            this.txtPoppedNodes.Name = "txtPoppedNodes";
            this.txtPoppedNodes.Size = new System.Drawing.Size(46, 20);
            this.txtPoppedNodes.TabIndex = 21;
            this.txtPoppedNodes.Tag = "1";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(35, 366);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(65, 13);
            this.label11.TabIndex = 19;
            this.label11.Text = "Path Length";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(14, 340);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(87, 13);
            this.label12.TabIndex = 20;
            this.label12.Text = "Expanded nodes";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(22, 120);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(37, 13);
            this.label13.TabIndex = 1;
            this.label13.Text = "Status";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(21, 94);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(38, 13);
            this.label14.TabIndex = 1;
            this.label14.Text = "Speed";
            // 
            // txtSpeed
            // 
            this.txtSpeed.Location = new System.Drawing.Point(66, 91);
            this.txtSpeed.Name = "txtSpeed";
            this.txtSpeed.Size = new System.Drawing.Size(63, 20);
            this.txtSpeed.TabIndex = 7;
            this.txtSpeed.Text = "10000";
            this.txtSpeed.TextChanged += new System.EventHandler(this.txtSpeed_TextChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.checkBox1);
            this.panel2.Controls.Add(this.comboBox1);
            this.panel2.Controls.Add(this.btnZoomIn);
            this.panel2.Controls.Add(this.btnZoomOut);
            this.panel2.Controls.Add(this.btnStart);
            this.panel2.Controls.Add(this.btnStop);
            this.panel2.Controls.Add(this.btnGo);
            this.panel2.Controls.Add(this.btnInput);
            this.panel2.Controls.Add(this.txtPathLength);
            this.panel2.Controls.Add(this.txtTreeNodes);
            this.panel2.Controls.Add(this.txtPoppedNodes);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.label11);
            this.panel2.Controls.Add(this.label12);
            this.panel2.Controls.Add(this.txtSpeed);
            this.panel2.Controls.Add(this.txtTime);
            this.panel2.Controls.Add(this.txtResult);
            this.panel2.Controls.Add(this.btnReset);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.label14);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.label15);
            this.panel2.Controls.Add(this.label13);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(699, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(163, 662);
            this.panel2.TabIndex = 23;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(25, 200);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBox1.Size = new System.Drawing.Size(75, 17);
            this.checkBox1.TabIndex = 29;
            this.checkBox1.Text = "Use Flows";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Ascending",
            "Descending"});
            this.comboBox1.Location = new System.Drawing.Point(66, 169);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(85, 21);
            this.comboBox1.TabIndex = 28;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.Location = new System.Drawing.Point(59, 35);
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(25, 23);
            this.btnZoomIn.TabIndex = 27;
            this.btnZoomIn.Text = "+";
            this.btnZoomIn.UseVisualStyleBackColor = true;
            this.btnZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.Location = new System.Drawing.Point(90, 35);
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(25, 23);
            this.btnZoomOut.TabIndex = 26;
            this.btnZoomOut.Text = "-";
            this.btnZoomOut.UseVisualStyleBackColor = true;
            this.btnZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(110, 281);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(49, 23);
            this.btnStart.TabIndex = 25;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Visible = false;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(80, 252);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(49, 23);
            this.btnStop.TabIndex = 25;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnGo
            // 
            this.btnGo.Location = new System.Drawing.Point(27, 252);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(51, 23);
            this.btnGo.TabIndex = 25;
            this.btnGo.Text = "GO";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // btnInput
            // 
            this.btnInput.Location = new System.Drawing.Point(26, 223);
            this.btnInput.Name = "btnInput";
            this.btnInput.Size = new System.Drawing.Size(103, 23);
            this.btnInput.TabIndex = 24;
            this.btnInput.Text = "Load input file";
            this.btnInput.UseVisualStyleBackColor = true;
            this.btnInput.Click += new System.EventHandler(this.btnInput_Click);
            // 
            // txtTreeNodes
            // 
            this.txtTreeNodes.Location = new System.Drawing.Point(107, 310);
            this.txtTreeNodes.Name = "txtTreeNodes";
            this.txtTreeNodes.Size = new System.Drawing.Size(46, 20);
            this.txtTreeNodes.TabIndex = 21;
            this.txtTreeNodes.Tag = "1";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 313);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(89, 13);
            this.label6.TabIndex = 20;
            this.label6.Text = "Generated nodes";
            // 
            // txtTime
            // 
            this.txtTime.Enabled = false;
            this.txtTime.Location = new System.Drawing.Point(66, 143);
            this.txtTime.Name = "txtTime";
            this.txtTime.Size = new System.Drawing.Size(63, 20);
            this.txtTime.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(50, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Zoom In\\Out";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 172);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Mode";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(23, 146);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(30, 13);
            this.label15.TabIndex = 1;
            this.label15.Text = "Time";
            // 
            // dlgOpenFile
            // 
            this.dlgOpenFile.DefaultExt = "txt";
            this.dlgOpenFile.FileName = "test.txt";
            this.dlgOpenFile.Filter = "Text|*.txt";
            // 
            // theradMain
            // 
            this.theradMain.DoWork += new System.ComponentModel.DoWorkEventHandler(this.threadMain_DoWork);
            this.theradMain.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.theradMain_RunWorkerCompleted);
            // 
            // tmrTryAgain
            // 
            this.tmrTryAgain.Interval = 90000;
            this.tmrTryAgain.Tick += new System.EventHandler(this.tmrTryAgain_Tick);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(862, 662);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.lblGameOver);
            this.Controls.Add(this.picCanvas);
            this.Name = "Main";
            this.Text = "AI Flow Solver";
            ((System.ComponentModel.ISupportInitialize)(this.picCanvas)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picCanvas;
        private System.Windows.Forms.Timer tmrDraw;
        private System.Windows.Forms.Label lblGameOver;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.TextBox txtPathLength;
        private System.Windows.Forms.TextBox txtPoppedNodes;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtSpeed;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.OpenFileDialog dlgOpenFile;
        private System.Windows.Forms.Button btnInput;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.TextBox txtTime;
        private System.Windows.Forms.Label label15;
        private System.ComponentModel.BackgroundWorker theradMain;
        private System.Windows.Forms.TextBox txtTreeNodes;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnZoomIn;
        private System.Windows.Forms.Button btnZoomOut;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Timer tmrTryAgain;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}

