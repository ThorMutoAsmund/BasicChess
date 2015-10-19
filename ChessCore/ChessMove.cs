using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChessCore
{
    public class ChessMove
    {
        public ChessPiece ChessPiece { get; set; }
        public int From { get; set; }
        public int To { get; set; }

        //public ChessMove(int from, int to)
        //{
        //    this.ChessPiece = ChessPiece.None;
        //    this.From = from;
        //    this.To = to;
        //}

        public ChessMove(ChessPiece chessPiece, int from, int to)
        {
            this.ChessPiece = chessPiece;
            this.From = from;
            this.To = to;
        }

        public ChessMove Clone()
        {
            return new ChessMove(this.ChessPiece, this.From, this.To);
        }

        public override string ToString()
        {
            return String.Format("{0} {1} {2}", 
                HistoricalChessMove.ChessPieceTypeToNotation(this.ChessPiece),
                HistoricalChessMove.LocationToNotation(this.From),
                HistoricalChessMove.LocationToNotation(this.To));
        }
    }

    public class ChessMoveWithEvaluation
    {
        public ChessMove Move { get; set; }
        public double Score { get; set; }

        public override string ToString()
        {
            return this.Move.ToString();
        }
    }

}
