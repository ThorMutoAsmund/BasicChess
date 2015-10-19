using ChessCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngines
{
    public class AlphaBeta : BasicChessEngine
    {
        protected override Task Run(List<ChessMove> moves)
        {
            Int32 alpha = Int32.MinValue ;
            Int32 beta = Int32.MaxValue;
            Int32 evaluation = Int32.MinValue;

            var result = new List<ChessMoveWithEvaluation>();

            var task = Task.Factory.StartNew(() =>
            {
                //Parallel.ForEach(moves, move =>
                foreach (var move in moves)
                {
                    ChessPiece capturedChessPiece;
                    var myBoard = this.Board.DeepClone();
                    myBoard.DoMove(move, out capturedChessPiece);

                    int logPos = GetLogPos();
                    var e = TryGetCachedEvaluation(myBoard, this.MaxDepth, () =>
                    {
                        if (this.MaxDepth == 0)
                        {
                            return Evaluate(myBoard, this.MySuit, 0);
                        }
                        return RunRecursive(myBoard, (short)(this.MaxDepth - 1), this.MySuit == ChessPiece.White ? ChessPiece.Black : ChessPiece.White,
                            alpha, beta, false);
                    });
                    evaluation = Math.Max(evaluation, e);
                    Log(this.MaxDepth, "<m t=\"max\" d=\"" + "0" + "\" m=\"" + move.ToString() + "\" e=\"" + e + "\">", logPos);
                    Log(this.MaxDepth, "</m>");

                    result.Add(new ChessMoveWithEvaluation()
                    {
                        Move = move,
                        Score = e
                    });

                    alpha = Math.Max(alpha, evaluation);
                    if (beta <= alpha)
                    {
                        //     break;
                    }
                }

                this.BestMoves = result.Where(m => m.Score == evaluation).ToList();
            });


            return task;// Task.Factory.StartNew(() => { });
        }

        private Int32 RunRecursive(ChessBoard myBoard, Int16 depth, ChessPiece turn,
            Int32 alpha, Int32 beta, bool maximizingPlayer)
        {
            if (depth == 0)
            {
                return TryGetCachedEvaluation(myBoard, 0, () =>
                {
                    return Evaluate(myBoard, turn, 0);
                });
            }

            List<ChessMove> moves = myBoard.GetAllMoves(turn);

            ChessPiece capturedChessPiece;
            Int32 evaluation;
            if (maximizingPlayer)
            {
                evaluation = Int32.MinValue;
                foreach (var move in moves)
                {
                    myBoard.DoMove(move, out capturedChessPiece);

                    int logPos = GetLogPos();
                    var e = TryGetCachedEvaluation(myBoard, depth, () =>
                    {
                        return RunRecursive(myBoard, (short)(depth - 1), turn == ChessPiece.White ? ChessPiece.Black : ChessPiece.White,
                            alpha, beta, false);
                    });
                    evaluation = Math.Max(evaluation, e);
                    Log(depth, "<m t=\"max\" d=\"" + (this.MaxDepth-depth) +"\" m=\"" + move.ToString() + "\" e=\"" + e + "\">", logPos);
                    Log(depth, "</m>");

                    myBoard.UndoMove(move, capturedChessPiece);
                    if (evaluation > alpha)
                    {
                        Log(depth, "<a>" + evaluation + " gt " + alpha + "</a>");
                    }
                    alpha = Math.Max(alpha, evaluation);
                    if (beta <= alpha)
                    {
                        Log(depth, "<c>" + beta + " lte " + alpha + "</c>");
                        break;
                    }
                }
            }
            else
            {
                evaluation = Int32.MaxValue;
                foreach (var move in moves)
                {
                    myBoard.DoMove(move, out capturedChessPiece);

                    int logPos = GetLogPos();
                    var e = TryGetCachedEvaluation(myBoard, depth, () =>
                    {
                        return RunRecursive(myBoard, (short)(depth - 1), turn == ChessPiece.White ? ChessPiece.Black : ChessPiece.White,
                            alpha, beta, true);
                    });
                    evaluation = Math.Min(evaluation, e);
                    Log(depth, "<m t=\"min\" d=\"" + (this.MaxDepth - depth) + "\" m=\"" + move.ToString() + "\" e=\"" + e + "\">", logPos);
                    Log(depth, "</m>");

                    myBoard.UndoMove(move, capturedChessPiece);

                    if (evaluation < beta)
                    {
                        Log(depth, "<b>" + evaluation + " lt " + beta + "</b>");
                    }

                    beta = Math.Min(beta, evaluation);
                    if (beta <= alpha)
                    {
                        Log(depth, "<c>" + beta + " lte " + alpha + "</c>");
                        break;
                    }
                }
            }

            return evaluation;
        }

        protected override Int32 Evaluate(ChessBoard myBoard, ChessPiece turn, Int16 depth)
        {
            //bool canMove;
            IncrementSearchCount();
            var materialValue = myBoard.GetMaterialValue(this.ChessPieceRelativeValues, depth);
            var positionalValue = myBoard.GetPositionalValue();

            /*  TBD!!
            if (!canMove)
            {
                if (turn == this.MySuit)
                {
                    // I am mated or stale mated
                    return myBoard.InCheck(turn) ? -WonScore : StaleMateScore;
                }
                else
                {
                    // Opponent is mated or stale mated
                    return myBoard.InCheck(turn) ? WonScore : StaleMateScore;
                }
            }*/

            return MaterialFactor * materialValue + PositionalFactor * positionalValue;
        }
    }
}
