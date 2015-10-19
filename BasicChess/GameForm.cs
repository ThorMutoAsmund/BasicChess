using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ChessCore;
using System.IO;

namespace BasicChess
{
    public partial class GameForm : Form
    {
        private ChessGame Game;
        private IVisualizer Visualizer;
        private IChessEngine ChessEngine;

        public GameForm()
        {
            InitializeComponent();
        }


        private void ExitButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private string fileToLoad = null;

        private void StartGameButton_Click(object sender, EventArgs e)
        {
            this.Game = new ChessGame();
            if (!String.IsNullOrEmpty(fileToLoad))
            {
                this.Game.Board.LoadFromFile(fileToLoad);
            }
            ChessPiece computerSuit;
            if (WhiteRadioButton.Checked)
            {
                this.Game.PlayerTypes[ChessPiece.White] = PlayerType.Human;
                this.Game.PlayerTypes[ChessPiece.Black] = PlayerType.Computer;
                computerSuit = ChessPiece.Black;
            }
            else
            {
                this.Game.PlayerTypes[ChessPiece.White] = PlayerType.Computer;
                this.Game.PlayerTypes[ChessPiece.Black] = PlayerType.Human;
                computerSuit = ChessPiece.White;
            }

            this.Visualizer = new BasicVisualizer.BasicVizualizer();
            this.Visualizer.Init(this.Game);
            var engine = new ChessEngines.AlphaBeta();
            engine.LoggingEnabled = true;
            string logFileName = DateTime.Now.ToString("LOG_yyyy-MM-dd_HH-mm-ss") + ".txt";
            engine.LogFilePath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), logFileName);
            this.ChessEngine = engine;
            this.Game.Engine = this.ChessEngine;
            this.ChessEngine.Init(this.Game.Board, computerSuit, true);

            // Start game
            this.Game.Start();
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    fileToLoad = dialog.FileName;
                }
            }
        }
    }
}

