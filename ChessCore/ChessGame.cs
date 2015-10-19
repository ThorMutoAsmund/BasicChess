using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChessCore
{
    public delegate void ResultChangedEventHanlder(ChessGame game, GameResult result);
    public delegate void ChessGameEventHanlder(ChessGame game);
    public delegate void ExecutionTimeEventHandler(TimeSpan executionTime);
    public delegate void NumberEventHandler(int numberOfSearches, int numberOfCacheHits);
    public delegate void MoveHandler(ChessMove move);

    public class ChessGame
    {
        public ChessBoard Board { get; set; }
        public List<HistoricalChessMove> History = new List<HistoricalChessMove>();
        public Dictionary<ChessPiece, PlayerType> PlayerTypes { get; set; }

        private DateTime StartTime { get; set;  }

        private IChessEngine _engine;
        public IChessEngine Engine
        {
            get
            {
                return this._engine;
            }
            set
            {
                if (this._engine != null)
                {
                    this._engine.NumberChanged -= OnNumberOfSearchesChanged;
                    this._engine.MoveReady -= ComputerMoveReady;
                }
                this._engine = value;
                this._engine.NumberChanged += OnNumberOfSearchesChanged;
                this._engine.MoveReady += ComputerMoveReady;
            }
        }

        private GameResult _gameResult = GameResult.Undecided;
        public GameResult GameResult
        {
            get
            {
                return _gameResult;
            }
            set
            {
                if (value != _gameResult)
                {
                    _gameResult = value;
                    OnResultChanged();
                }
            }
        }

        public event ChessGameEventHanlder BoardChanged;
        protected void OnBoardChanged()
        {
            if (BoardChanged != null)
            {
                BoardChanged(this);
            }
        }

        public event ResultChangedEventHanlder ResultChanged;
        protected void OnResultChanged()
        {
            if (ResultChanged != null)
            {
                ResultChanged(this, this.GameResult);
            }
        }

        public event ExecutionTimeEventHandler ExecutionTimeUpdated;
        protected void OnExecutionTimeUpdated(TimeSpan executionTime)
        {
            if (ExecutionTimeUpdated != null)
            {
                ExecutionTimeUpdated(executionTime);
            }
        }

        public event NumberEventHandler NumberChanged;
        protected void OnNumberOfSearchesChanged(int numberOfSearches, int numberOfCacheHits)
        {
            if (NumberChanged != null)
            {
                NumberChanged(numberOfSearches, numberOfCacheHits);
            }
        }

        public ChessGame()
        {
            Board = new ChessBoard();
            Board.Reset();
            PlayerTypes = new Dictionary<ChessPiece, PlayerType>() { { ChessPiece.Black, PlayerType.Human }, { ChessPiece.White, PlayerType.Human } };
        }

        public void Start()
        {
            GameResult = GameResult.Undecided;
            AfterTurnChanged();
        }

        public void ComputerMoveReady(ChessMove move)
        {
            DateTime stopTime = DateTime.Now;
            TimeSpan duration = stopTime - this.StartTime;

            OnExecutionTimeUpdated(duration);

            Move(move);
        }

        public void Move(ChessMove move)
        {
            HistoricalChessMove historicalMove = new HistoricalChessMove(move, this.Board.Fields[move.To]);
            this.History.Add(historicalMove);
            
            // Update board
            this.Board.DoMove(move);

            // New turn
            this.Board.ChangeTurn();

            // Event
            OnBoardChanged();

            // After turn
            AfterTurnChanged();
        }

        public void UndoLastMove()
        {
            if (this.History.Count > 1)
            {
                var historicalMove = this.History.Last();
                this.History.Remove(historicalMove);
                this.Board.UndoMove(historicalMove.Move, historicalMove.Captures);
                historicalMove = this.History.Last();
                this.History.Remove(historicalMove);
                this.Board.UndoMove(historicalMove.Move, historicalMove.Captures);

                // Event
                OnBoardChanged();
            }
        }

        private void AfterTurnChanged()
        {
            // Check for lost game
            if (!(this.Board.Turn == ChessPiece.Black ? this.Board.Black : this.Board.White).
                Any(c => (this.Board.Fields[c] & ChessPiece.Type) == ChessPiece.King))
            {
                this.GameResult = this.Board.Turn == ChessPiece.Black ? GameResult.BlackKingTaken : GameResult.WhiteKingTaken;
            }
            else
            {
                MateType mateType = MateType.None;
                if (this.Board.IsMate(ref mateType))
                {
                    this.GameResult =
                        mateType == MateType.Mate ?
                            (Board.Turn == ChessPiece.Black ? GameResult.BlackCheckMate : GameResult.WhiteCheckMate) :
                            GameResult.StaleMate;
                }
            }

            // Do not continue if game is over
            if (this.GameResult != GameResult.Undecided)
            {
                return;
            }

            // Let computer move
            if (PlayerTypes[Board.Turn] == PlayerType.Computer && Engine != null)
            {
                this.StartTime = DateTime.Now;
                Engine.Turn();
            }
        }

        public string GetHistoryAsText()
        {
            string result = String.Empty;
            int turn = 0;
            foreach (var historicalMove in this.History)
            {
                if (turn % 2 == 0 && turn > 0)
                {
                    result += Environment.NewLine;
                }

                if (turn % 2 == 0)
                {
                    result += ((turn / 2) + 1) + ". ";
                }

                if (turn % 2 == 1)
                {
                    result += " ";
                }
                result += historicalMove.ToString();
                turn++;
            }

            return result;
        }
    }
}
