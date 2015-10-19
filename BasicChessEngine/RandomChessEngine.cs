using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ChessCore;

namespace ChessEngines
{
    public class RandomChessEngine : IChessEngine
    {
        private ChessBoard Board { get; set; }
        private ChessPiece MySuit { get; set; }

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
            this.Board = board;
            this.MySuit = mySuit;
        }

        public void Turn()
        {
            List<ChessMove> moves = this.Board.GetAllMoves(MySuit);

            Random randomGenerator = new Random();
            int r = randomGenerator.Next(0, moves.Count);

            MoveReady(moves[r]);
        }
    }
}
