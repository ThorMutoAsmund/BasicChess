using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ChessCore;
using System.Threading.Tasks;
using System.IO;

namespace ChessEngines
{
    public abstract class BasicChessEngine : IChessEngine
    {
        public bool LoggingEnabled { get; set; }
        public string LogFilePath { get; set; }
        
        private StreamWriter LogStream { get; set; }
        private StringBuilder AggregatedMessage { get; set; }

        protected const int LowestScore = -256000;
        protected const int HighestScore = 256000;
        protected const int WonScore = 128000;
        protected const int StaleMateScore = 128000;
        protected const int MaterialFactor = 128;
        protected const int PositionalFactor = 1;
        protected const int LeaveCheckedScore = 130000;

        protected Int16 MaxDepth { get; set; }
        protected double CutLimit { get; set; }
        protected ChessBoard Board { get; set; }
        protected ChessPiece MySuit { get; set; }
        protected List<int> MyChessPieces { get; set; }
        protected Int32[] ChessPieceRelativeValues { get; set; }
        protected int Searches { get; set; }
        protected int CacheHits { get; set; }
        protected List<ChessMoveWithEvaluation> BestMoves { get; set; }

        protected ZobristHashTree<Evaluation> EvaluationHash { get; set; }

        protected class Evaluation
        {
            public Int32 Value { get; set; }
            public Int16 Depth { get; set; }
        }

        public event NumberEventHandler NumberChanged;
        protected void OnNumberChanged(int numberOfSearches, int numberOfCacheHits)
        {
            if (NumberChanged != null)
            {
                NumberChanged(numberOfSearches, numberOfCacheHits);
            }
        }

        public event MoveHandler MoveReady;
        protected void OnMoveReady(ChessMove move)
        {
            if (MoveReady != null)
            {
                MoveReady(move);
            }
        }

        public void Init(ChessBoard board, ChessPiece mySuit, bool multiCore = false)
        {
            this.AggregatedMessage = new StringBuilder();
            this.Log("<game>");

            this.MaxDepth = 6;
            this.CutLimit = 257;

            this.Board = board;
            this.MySuit = mySuit;
            this.MyChessPieces = mySuit == ChessPiece.White ? this.Board.White : this.Board.Black;

            this.ChessPieceRelativeValues = new Int32[7] { 0, 1, 3, 3, 5, 9, 0 };
            this.EvaluationHash = new ZobristHashTree<Evaluation>();

            if (this.LoggingEnabled)
            {
                this.LogStream = new System.IO.StreamWriter(this.LogFilePath, false);
                this.LogStream.AutoFlush = true;
            }
            var now = DateTime.Now;
            Log("<message>LOG STARTED " + now.ToLongDateString() + " " + now.ToLongTimeString() + "</message>");
            Log("<message>0 = " + (mySuit == ChessPiece.Black ? "black" : "white") + "</message>");
            Log("<message>1 = " + (mySuit == ChessPiece.Black ? "white" : "black") + "</message>");
            Log();
            this.LogStore();
        }

        ~BasicChessEngine()
        {
            if (this.LogStream != null)
            {
                //this.Log("</game>");
                //this.LogStore();
                //this.LogStream.Close();
            }
        }


        public void Turn()
        {
            this.AggregatedMessage.Clear();
            Log("<turn start=\"" + DateTime.Now.ToLongTimeString() + "\">");
            if (this.MaxDepth < 0)
            {
                throw new Exception("Illegal max depth value");
            }

            this.Searches = 0;
            this.CacheHits = 0;
            this.BestMoves = new List<ChessMoveWithEvaluation>() { new ChessMoveWithEvaluation() { Score = LowestScore } };

            List<ChessMove> moves = this.Board.GetAllMoves(this.MySuit);

            if (moves.Count == 0)
            {
                throw new Exception("No moves possible");
            }

            //var task = BreadthFirst(moves);
            var task = Run(moves);

            task.ContinueWith((antecedent) =>
            {
                var bestMove = this.BestMoves[new Random().Next(this.BestMoves.Count)];
                if (bestMove.Move == null)
                {
                    throw new Exception("No best move");
                }

                Console.WriteLine("Score: " + bestMove.Score);
                foreach (var move in this.BestMoves)
                {
                    Console.WriteLine(" " + move);
                }

                OnNumberChanged(this.Searches, this.CacheHits);
                MoveReady(bestMove.Move);
                Log("</turn>");
                LogStore();
            });

        }

        protected abstract Task Run(List<ChessMove> moves);

        protected Int32 TryGetCachedEvaluation(ChessBoard board, Int16 depth, Func<Int32> evaluationFunction)
        {
            Evaluation evaluation;

            lock (this.EvaluationHash)
            {
                if (this.EvaluationHash.TryGetValue(board.Hash, out evaluation))
                {
                    if (evaluation.Depth >= depth)
                    {
                        IncrementCacheHit();
                        return evaluation.Value;
                    }
                    this.EvaluationHash.Remove(board.Hash);
                }
            }

            Int32 evaluationValue = evaluationFunction();

            lock (this.EvaluationHash)
            {
                this.EvaluationHash.SetValue(board.Hash, new Evaluation() { Depth = depth, Value = evaluationValue });
            }

            return evaluationValue;
        }

        protected abstract Int32 Evaluate(ChessBoard myBoard, ChessPiece turn, Int16 depth);

        protected void IncrementSearchCount()
        {
            this.Searches++;
            if (this.Searches % 50000 == 0)
            {
                OnNumberChanged(this.Searches, this.CacheHits);
            }
        }

        private void IncrementCacheHit()
        {
            this.CacheHits++;
        }

        protected void Log(string message = "", int index = -1)
        {
            if (this.LoggingEnabled)
            {
                if (index == -1)
                {
                    this.AggregatedMessage.Append(message);
                }
                else
                {
                    this.AggregatedMessage.Insert(index, message);
                }
            }
        }

        protected void Log(int depth, string message, int pos = -1)
        {
            Log(message.PadLeft(message.Length + 2 * (this.MaxDepth - depth)), pos);
        }

        protected int GetLogPos()
        {
            return this.AggregatedMessage.Length;
        }


        private void LogStore()
        {
            if (this.LoggingEnabled)
            {
                this.LogStream.WriteLine(this.AggregatedMessage.ToString());
                this.AggregatedMessage.Clear();
            }
        }
    }
}
