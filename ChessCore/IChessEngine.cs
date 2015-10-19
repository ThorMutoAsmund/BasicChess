using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChessCore
{
    public interface IChessEngine
    {
        void Init(ChessBoard board, ChessPiece mySuit, bool multiCore = false);
        void Turn();
        event NumberEventHandler NumberChanged;
        event MoveHandler MoveReady;
    }
}
