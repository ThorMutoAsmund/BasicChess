using ChessCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngines
{
    public class BreadthFirst : BasicChessEngine
    {
        protected override Task Run(List<ChessMove> moves)
        {
            var task = Task.Factory.StartNew(() =>
                Parallel.ForEach(moves, move =>
                {
                    ChessPiece capturedChessPiece;
                    var myBoard = this.Board.DeepClone();
                    myBoard.DoMove(move, out capturedChessPiece);

                    Int32 evaluation = TryGetCachedEvaluation(myBoard, this.MaxDepth, () =>
                    {
                        return -RunRecursive(myBoard, (short)(this.MaxDepth - 1), this.MySuit == ChessPiece.White ? ChessPiece.Black : ChessPiece.White);
                    });


                    if (evaluation >= this.BestMoves[0].Score)
                    {
                        lock (this.BestMoves)
                        {
                            if (evaluation == this.BestMoves[0].Score)
                            {
                                this.BestMoves.Add(new ChessMoveWithEvaluation()
                                {
                                    Move = move,
                                    Score = evaluation
                                });
                            }
                            else if (evaluation > this.BestMoves[0].Score)
                            {
                                this.BestMoves = new List<ChessMoveWithEvaluation>()
                                {
                                    new ChessMoveWithEvaluation()
                                    {
                                        Move = move,
                                        Score = evaluation
                                    }
                                };
                            }
                        }
                    }
                })
            );

            return task;
        }

        private Int32 RunRecursive(ChessBoard myBoard, Int16 depth, ChessPiece turn)
        {
            List<ChessMove> moves = myBoard.GetAllMoves(turn);

            if (moves.Count == 0)
            {
                if (turn == this.MySuit)
                {
                    // I am mated or stale mated
                    return this.Board.InCheck(turn) ? -WonScore : StaleMateScore;
                }
                else
                {
                    // Opponent is mated or stale mated
                    return this.Board.InCheck(turn) ? WonScore : StaleMateScore;
                }
            }

            Int32 evaluation;
            Int32 bestEvaluation = LowestScore;
            ChessPiece capturedChessPiece;
            foreach (ChessMove move in moves)
            {
                myBoard.DoMove(move, out capturedChessPiece);

                if (depth > 1)
                {
                    // Pre evaluation to determine cut
                    evaluation = TryGetCachedEvaluation(myBoard, 1, () =>
                    {
                        return Evaluate(myBoard, turn, depth);
                    });

                    if (evaluation > this.CutLimit)
                    {
                        myBoard.UndoMove(move, capturedChessPiece);
                        return evaluation;
                    }

                    // Full evaluation
                    evaluation = TryGetCachedEvaluation(myBoard, depth, () =>
                    {
                        return -RunRecursive(myBoard, (short)(depth - 1), turn == ChessPiece.White ? ChessPiece.Black : ChessPiece.White);
                    });
                }
                else
                {
                    evaluation = TryGetCachedEvaluation(myBoard, depth, () =>
                    {
                        return Evaluate(myBoard, turn, depth);
                    });
                }

                if (evaluation > bestEvaluation)
                {
                    bestEvaluation = evaluation;
                }
                myBoard.UndoMove(move, capturedChessPiece);
            }

            return bestEvaluation;
        }

        protected override Int32 Evaluate(ChessBoard myBoard, ChessPiece turn, Int16 depth)
        {
            IncrementSearchCount();
            var materialValue = (turn == ChessPiece.White ? 1 : -1) * myBoard.GetMaterialValue(this.ChessPieceRelativeValues, depth);
            var positionalValue = (turn == ChessPiece.White ? 1 : -1) * myBoard.GetPositionalValue();

            return MaterialFactor * materialValue + PositionalFactor * positionalValue;
        }
    }
}
