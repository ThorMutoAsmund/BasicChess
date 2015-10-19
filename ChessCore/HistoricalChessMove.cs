using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChessCore
{
    public class HistoricalChessMove
    {
        public ChessMove Move { get; set; }
        public ChessPiece Captures { get; set; }
        public bool EnPassant { get; set; }
        public ChessPiece PromotedTo { get; set; }
        public CastlingType Castling { get; set; }
        public CheckType Check { get; set; }

        public HistoricalChessMove(ChessMove move, ChessPiece captures)
        {
            this.Move = move.Clone();
            this.Captures = captures;
        }

        public override string ToString()
        {
            if (Castling != CastlingType.None)
                return CastlingTypeToNotation(Castling);

            string result = String.Empty;
            if ((this.Move.ChessPiece & ChessPiece.Type) == ChessPiece.Pawn)
            {
                result = LocationToNotation(this.Move.From)[0] + 
                    (this.Captures != ChessPiece.None ? "x" : "") + 
                    LocationToNotation(this.Move.To) + 
                    (this.EnPassant ? " e.p." : "") + 
                    ChessPieceTypeToNotation(this.PromotedTo);
            }
            else
            {
                result = ChessPieceTypeToNotation(this.Move.ChessPiece) + 
                    LocationToNotation(this.Move.From) + 
                    (this.Captures != ChessPiece.None ? "x" : "") + 
                    LocationToNotation(this.Move.To);
            }

            result += CheckTypeToNotation(Check);

            return result;
        }

        public static string CastlingTypeToNotation(CastlingType castlingType)
        {
            switch (castlingType)
            {
                case CastlingType.KingSide:
                    return "O-O";
                case CastlingType.QueenSide:
                    return "O-O-O";
            }

            return "";
        }

        public static string CheckTypeToNotation(CheckType checkType)
        {
            switch (checkType)
            {
                case CheckType.Check:
                    return "+";
                case CheckType.CheckAndMate:
                    return "#";
            }

            return "";
        }

        public static string ChessPieceTypeToNotation(ChessPiece chessPiece)
        {
            switch (chessPiece & ChessPiece.Type)
            {
                case ChessPiece.Pawn:
                    return "P";
                case ChessPiece.Knight:
                    return "N";
                case ChessPiece.Bishop:
                    return "B";
                case ChessPiece.Rook:
                    return "R";
                case ChessPiece.Queen:
                    return "Q";
                case ChessPiece.King:
                    return "K";
            }
            return "";
        }

        public static string LocationToNotation(int location)
        {
            string notation = String.Empty;
            switch (location % 8)
            {
                case 0: notation = "a"; break;
                case 1: notation = "b"; break;
                case 2: notation = "c"; break;
                case 3: notation = "d"; break;
                case 4: notation = "e"; break;
                case 5: notation = "f"; break;
                case 6: notation = "g"; break;
                case 7: notation = "h"; break;
            }
            notation += (location / 8) + 1;

            return notation;
        }
    }
}
