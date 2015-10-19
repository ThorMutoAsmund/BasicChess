namespace BasicVisualizer
{
    partial class BoardForm
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
            this.BoardPictureBox = new System.Windows.Forms.PictureBox();
            this.HistoryTextBox = new System.Windows.Forms.TextBox();
            this.DestinationSelectedTimer = new System.Windows.Forms.Timer(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.ExecutionTimeLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.UndoButton = new System.Windows.Forms.Button();
            this.NumberOfSearchesLabel = new System.Windows.Forms.Label();
            this.ResultLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.BoardPictureBox)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // BoardPictureBox
            // 
            this.BoardPictureBox.Location = new System.Drawing.Point(24, 65);
            this.BoardPictureBox.Margin = new System.Windows.Forms.Padding(6);
            this.BoardPictureBox.Name = "BoardPictureBox";
            this.BoardPictureBox.Size = new System.Drawing.Size(480, 462);
            this.BoardPictureBox.TabIndex = 0;
            this.BoardPictureBox.TabStop = false;
            this.BoardPictureBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.BoardPictureBox_MouseClick);
            // 
            // HistoryTextBox
            // 
            this.HistoryTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.HistoryTextBox.Location = new System.Drawing.Point(516, 65);
            this.HistoryTextBox.Margin = new System.Windows.Forms.Padding(6);
            this.HistoryTextBox.Multiline = true;
            this.HistoryTextBox.Name = "HistoryTextBox";
            this.HistoryTextBox.ReadOnly = true;
            this.HistoryTextBox.Size = new System.Drawing.Size(330, 463);
            this.HistoryTextBox.TabIndex = 1;
            // 
            // DestinationSelectedTimer
            // 
            this.DestinationSelectedTimer.Interval = 200;
            this.DestinationSelectedTimer.Tick += new System.EventHandler(this.DestinationSelectedTimer_Tick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ExecutionTimeLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 624);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(2, 0, 28, 0);
            this.statusStrip1.Size = new System.Drawing.Size(890, 37);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // ExecutionTimeLabel
            // 
            this.ExecutionTimeLabel.Name = "ExecutionTimeLabel";
            this.ExecutionTimeLabel.Size = new System.Drawing.Size(45, 32);
            this.ExecutionTimeLabel.Text = "---";
            // 
            // UndoButton
            // 
            this.UndoButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.UndoButton.Location = new System.Drawing.Point(700, 544);
            this.UndoButton.Margin = new System.Windows.Forms.Padding(6);
            this.UndoButton.Name = "UndoButton";
            this.UndoButton.Size = new System.Drawing.Size(150, 44);
            this.UndoButton.TabIndex = 3;
            this.UndoButton.Text = "Undo";
            this.UndoButton.UseVisualStyleBackColor = true;
            this.UndoButton.Click += new System.EventHandler(this.UndoButton_Click);
            // 
            // NumberOfSearchesLabel
            // 
            this.NumberOfSearchesLabel.AutoSize = true;
            this.NumberOfSearchesLabel.Location = new System.Drawing.Point(21, 25);
            this.NumberOfSearchesLabel.Name = "NumberOfSearchesLabel";
            this.NumberOfSearchesLabel.Size = new System.Drawing.Size(33, 25);
            this.NumberOfSearchesLabel.TabIndex = 4;
            this.NumberOfSearchesLabel.Text = "---";
            // 
            // ResultLabel
            // 
            this.ResultLabel.AutoSize = true;
            this.ResultLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ResultLabel.Location = new System.Drawing.Point(19, 554);
            this.ResultLabel.Name = "ResultLabel";
            this.ResultLabel.Size = new System.Drawing.Size(169, 37);
            this.ResultLabel.TabIndex = 5;
            this.ResultLabel.Text = "Undecided";
            // 
            // BoardForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(890, 661);
            this.Controls.Add(this.ResultLabel);
            this.Controls.Add(this.NumberOfSearchesLabel);
            this.Controls.Add(this.UndoButton);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.HistoryTextBox);
            this.Controls.Add(this.BoardPictureBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(848, 511);
            this.Name = "BoardForm";
            this.Text = "Board";
            ((System.ComponentModel.ISupportInitialize)(this.BoardPictureBox)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox BoardPictureBox;
        private System.Windows.Forms.TextBox HistoryTextBox;
        private System.Windows.Forms.Timer DestinationSelectedTimer;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel ExecutionTimeLabel;
        private System.Windows.Forms.Button UndoButton;
        private System.Windows.Forms.Label NumberOfSearchesLabel;
        private System.Windows.Forms.Label ResultLabel;
    }
}