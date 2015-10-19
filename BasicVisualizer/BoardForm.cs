using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ChessCore;

namespace BasicVisualizer
{
    public partial class BoardForm : Form
    {
        private Bitmap BoardImage;
        private ChessGame Game;
        private TurnStage Stage = TurnStage.Waiting;
        private int StartField;
        private int DestinationField;

        private List<ChessMove> AllMoves = null;

        private delegate void SetNumberOfSearchesCallback(int numberOfSearches, int numberOfCacheHits);
        private delegate void VoidCallback();
        private delegate void ShowResultCallback(string result);
        private delegate void ShowExecutionTimeCallback(string executionTime);

        public BoardForm(ChessGame game)
        {
            InitializeComponent();

            Game = game;
            BoardImage = new Bitmap(240, 240, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        }

        /// <summary>
        /// Thread safe draw game
        /// </summary>
        public void DrawGame()
        {
            if (this.BoardPictureBox.InvokeRequired)
            {
                var d = new VoidCallback(DrawGame);
                this.Invoke(d);
            }
            else
            {
                var graphics = Graphics.FromImage(this.BoardImage);
                var blackBrush = new SolidBrush(Color.BurlyWood);
                var whiteBrush = new SolidBrush(Color.White);
                for (int i = 0; i < 64; ++i)
                {
                    int x = (i % 8) * 30;
                    int y = 210 - (i / 8) * 30;
                    graphics.FillRectangle((i + (i / 8)) % 2 == 0 ? blackBrush : whiteBrush, x, y, 30, 30);
                    var chessPiece = this.Game.Board.Fields[i];
                    if (chessPiece != ChessPiece.None)
                    {
                        Image chessPieceImage = null;
                        switch (chessPiece)
                        {
                            case ChessPiece.BPawn:
                                chessPieceImage = Resources.b_6_pawn;
                                break;
                            case ChessPiece.BKnight:
                                chessPieceImage = Resources.b_4_knight;
                                break;
                            case ChessPiece.BBishop:
                                chessPieceImage = Resources.b_3_bishop;
                                break;
                            case ChessPiece.BRook:
                                chessPieceImage = Resources.b_5_rook;
                                break;
                            case ChessPiece.BQueen:
                                chessPieceImage = Resources.b_2_queen;
                                break;
                            case ChessPiece.BKing:
                                chessPieceImage = Resources.b_1_king;
                                break;
                            case ChessPiece.WPawn:
                                chessPieceImage = Resources.w_6_pawn;
                                break;
                            case ChessPiece.WKnight:
                                chessPieceImage = Resources.w_4_knight;
                                break;
                            case ChessPiece.WBishop:
                                chessPieceImage = Resources.w_3_bishop;
                                break;
                            case ChessPiece.WRook:
                                chessPieceImage = Resources.w_5_rook;
                                break;
                            case ChessPiece.WQueen:
                                chessPieceImage = Resources.w_2_queen;
                                break;
                            case ChessPiece.WKing:
                                chessPieceImage = Resources.w_1_king;
                                break;
                        }
                        graphics.DrawImage(chessPieceImage, new Rectangle(x + 2, y + 2, 26, 26));
                    }
                }

                this.HistoryTextBox.Text = Game.GetHistoryAsText();
                this.HistoryTextBox.ScrollToCaret();
                this.BoardPictureBox.Image = this.BoardImage;
            }
        }

        /// <summary>
        /// Thread safe, sets the label showing number of searches
        /// </summary>
        /// <param name="numberOfSearches"></param>
        public void ShowNumber(int numberOfSearches, int numberOfCacheHits)
        {
            if (this.NumberOfSearchesLabel.InvokeRequired)
            {
                var d = new SetNumberOfSearchesCallback(ShowNumber);
                this.Invoke(d, new object[] { numberOfSearches, numberOfCacheHits });
            }
            else
            {
                this.NumberOfSearchesLabel.Text = "Searches: " + numberOfSearches.ToString() + " Cache hits: " + numberOfCacheHits.ToString();
            }
        }

        /// <summary>
        /// Thread safe, sets the label showing the result
        /// </summary>
        /// <param name="result"></param>
        public void ShowResult(string result)
        {
            if (this.ResultLabel.InvokeRequired)
            {
                var d = new ShowResultCallback(ShowResult);
                this.Invoke(d, new object[] { result });
            }
            else
            {
                this.ResultLabel.Text = result;
            }
        }

        /// <summary>
        /// Thread safe, sets the label showing execution time
        /// </summary>
        /// <param name="executionTime"></param>
        public void ShowExecutionTime(string executionTime)
        {
            if (this.ResultLabel.InvokeRequired)
            {
                var d = new ShowExecutionTimeCallback(ShowExecutionTime);
                this.Invoke(d, new object[] { executionTime });
            }
            else
            {
                this.ExecutionTimeLabel.Text = "Execution time: " + executionTime;
            }
        }


        /// <summary>
        /// Handle mouse clicks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BoardPictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            // Do not allow play if computer turn
            if (Game.PlayerTypes[Game.Board.Turn] == PlayerType.Computer || Game.GameResult != GameResult.Undecided)
                return;

            // Get all possible moves
            if (AllMoves == null)
                AllMoves = Game.Board.GetAllMoves(Game.Board.Turn);

            int field = (e.X / 30) + ((240-e.Y) / 30) * 8;
            bool drawSquare = false;
            Pen squarePen = null;
            if (Stage == TurnStage.Waiting)
            {
                StartField = field;
                if (Game.Board.Fields[StartField] != ChessPiece.None && Game.Board.Turn == (Game.Board.Fields[StartField] & ChessPiece.Suit))
                {
                    Stage = TurnStage.StartSelected;
                    drawSquare = true;
                    squarePen = new Pen(Color.Red);
                }
            }
            else if (Stage == TurnStage.StartSelected)
            {
                DestinationField = field;

                if (DestinationField == StartField)
                {
                    DrawGame();
                    Stage = TurnStage.Waiting;
                }
                //else if (Game.Board.IsLegalMove(Game.Board.Fields[StartField], DestinationField))
                else if (AllMoves.Any(m => m.From == StartField && m.To == DestinationField))
                {
                    Stage = TurnStage.DestinationSelected;
                    drawSquare = true;
                    squarePen = new Pen(Color.Red);
                    DestinationSelectedTimer.Start();
                }
            }

            if (drawSquare)
            {
                squarePen.Width = 2;
                Graphics graphics = Graphics.FromImage(BoardImage);
                int x = (field % 8) * 30;
                int y = 210 - (field / 8) * 30;
                graphics.DrawRectangle(squarePen, x, y, 30, 30);
                BoardPictureBox.Image = BoardImage;
            }
        }

        private enum TurnStage
        {
            Waiting,
            StartSelected,
            DestinationSelected
        }

        private void DestinationSelectedTimer_Tick(object sender, EventArgs e)
        {
            DestinationSelectedTimer.Stop();
            AllMoves = null;
            Game.Move(new ChessMove(Game.Board.Fields[this.StartField], this.StartField, this.DestinationField));
            Stage = TurnStage.Waiting;
        }

        private void UndoButton_Click(object sender, EventArgs e)
        {
            // Do not allow play if computer turn
            if (Game.PlayerTypes[Game.Board.Turn] == PlayerType.Computer || Game.GameResult != GameResult.Undecided)
                return;

            Game.UndoLastMove();
        }
    }

}
