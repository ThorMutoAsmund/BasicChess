using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChessCore
{
    public enum GameResult
    {
        Undecided,
        StaleMate,
        WhiteCheckMate,
        BlackCheckMate,
        WhiteKingTaken,
        BlackKingTaken,
        WhiteResigned,
        BlackResigned,
        Draw
    }

    public enum ChessPiece
    {
        None = 0,
        Black = 0,
        BPawn = 1,
        Pawn = 1,
        BKnight = 2,
        Knight = 2,
        BBishop = 3,
        Bishop = 3,
        BRook = 4,
        Rook = 4,
        BQueen = 5,
        Queen = 5,
        BKing = 6,
        King = 6,
        Type = 15,

        Suit = 16,
        White = 16,
        WPawn = 1 + 16,
        WKnight = 2 + 16,
        WBishop = 3 + 16,
        WRook = 4 + 16,
        WQueen = 5 + 16,
        WKing = 6 + 16,
        Max = 32
    }

    public enum CastlingType
    {
        None = 0,
        KingSide,
        QueenSide
    }

    public enum CheckType
    {
        None = 0,
        Check,
        CheckAndMate
    }

    public enum MateType
    {
        None = 0,
        Mate,
        StaleMate
    }

    public enum ValidMoveCaptureRule
    {
        None,
        MustCapture,
        MustNotCapture
    }
    
    public enum PlayerType
    {
        Human,
        Computer
    }


}
