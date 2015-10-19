using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ChessCore
{
    public class ChessBoard
    {
        private UInt64[] HashTable { get; set; }
        private Dictionary<ChessPiece, List<ValidMoveSet>>[] FieldMoves { get; set; }

        public ChessPiece[] Fields { get; private set; }
        public List<int> White { get; private set; }
        public List<int> Black { get; private set; }
        public ChessPiece Turn { get; set; }
        public Int16 Depth { get; set; }
        public UInt64 Hash { get; private set; }

        public ChessBoard()
        {
            Clear();

            InitializeHashTable(1234);
            InitializeFieldMoves();
        }

        private void InitializeHashTable(int seed)
        {
            var generator = new Random(seed);
            this.HashTable = new UInt64[(int)ChessPiece.Max * 64];
            foreach (var chessPiece in new ChessPiece[] {
                ChessPiece.BPawn, ChessPiece.BRook, ChessPiece.BKnight, ChessPiece.BBishop, ChessPiece.BQueen, ChessPiece.BKnight,
                ChessPiece.WPawn, ChessPiece.WRook, ChessPiece.WKnight, ChessPiece.WBishop, ChessPiece.WQueen, ChessPiece.WKnight })
            {
                for (int f = 0; f < 64; ++f)
                {
                    // Calculate random hash
                    UInt64 hash = 0;
                    for (int h = 0; h < 63; ++h)
                    {
                        hash <<= 1;
                        if (generator.Next(2) == 1)
                        {
                            hash |= 1;
                        }
                    }
                    this.HashTable[(int)chessPiece * 64 + f] = hash;
                }
            }
        }

        private void InitializeFieldMoves()
        {
            this.FieldMoves = new Dictionary<ChessPiece, List<ValidMoveSet>>[64];
            for (int i=0; i<64; ++i)
            {
                int x = i % 8;
                int y = i / 8;
                this.FieldMoves[i] = new Dictionary<ChessPiece, List<ValidMoveSet>>();
                ValidMoveSet validMoves;

                List<ValidMoveSet> moveListBlackPawn = new List<ValidMoveSet>();
                List<ValidMoveSet> moveListWhitePawn = new List<ValidMoveSet>();
                List<ValidMoveSet> moveListKnight = new List<ValidMoveSet>();
                List<ValidMoveSet> moveListBishop = new List<ValidMoveSet>();
                List<ValidMoveSet> moveListRook = new List<ValidMoveSet>();
                List<ValidMoveSet> moveListQueen = new List<ValidMoveSet>();
                List<ValidMoveSet> moveListKing = new List<ValidMoveSet>();


                // White pawn
                if (y < 7 && y > 0)
                {
                    validMoves = new ValidMoveSet(x, y, ValidMoveCaptureRule.MustNotCapture);
                    validMoves.AddMove(x, y + 1);
                    if (y == 1)
                    {
                        validMoves.AddMove(x, y + 2);
                    }
                    moveListWhitePawn.Add(validMoves);
                }
                if (y < 7 && y > 0)
                {
                    validMoves = new ValidMoveSet(x, y, ValidMoveCaptureRule.MustCapture, false);
                    if (x > 0)
                    {
                        validMoves.AddMove(x - 1, y + 1);
                    }
                    if (x < 7)
                    {
                        validMoves.AddMove(x + 1, y + 1);
                    }
                    moveListWhitePawn.Add(validMoves);
                }

                // TBD: En passant

                // Black pawn
                if (y < 7 && y > 0)
                {
                    validMoves = new ValidMoveSet(x, y, ValidMoveCaptureRule.MustNotCapture);
                    validMoves.AddMove(x, y - 1);
                    if (y == 6)
                    {
                        validMoves.AddMove(x, y - 2);
                    }
                    moveListBlackPawn.Add(validMoves);
                }
                if (y < 7 && y > 0)
                {
                    validMoves = new ValidMoveSet(x, y, ValidMoveCaptureRule.MustCapture, false);
                    if (x > 0)
                    {
                        validMoves.AddMove(x - 1, y - 1);
                    }
                    if (x < 7)
                    {
                        validMoves.AddMove(x + 1, y - 1);
                    }
                    moveListBlackPawn.Add(validMoves);
                }                

                // TBD: En passant

                // Knight
                validMoves = new ValidMoveSet(x, y, ValidMoveCaptureRule.None, false);
                int [] knightMoves = new int[16]{ -1,-2, 1,-2, -2,-1, 2,-1, -2,1, 2,1, -1,2, 1,2 };
                for (int j=0; j<16; j+=2)
                {
                    if (x + knightMoves[j] < 0 || x + knightMoves[j] > 7 || y + knightMoves[j + 1] < 0 || y + knightMoves[j + 1] > 7)
                        continue;
                    validMoves.AddMove(x + knightMoves[j], y + knightMoves[j + 1]);
                }
                moveListKnight.Add(validMoves);

                // Bishop/queen
                if (x < 7 && y < 7)
                {
                    validMoves = new ValidMoveSet(x, y);
                    for (int j = 1; j < 8; ++j)
                    {
                        if (x + j > 7 || y + j > 7)
                            break;
                        validMoves.AddMove(x + j, y + j);
                    }
                    moveListBishop.Add(validMoves);
                    moveListQueen.Add(validMoves);
                }

                if (x < 7 && y > 0)
                {
                    validMoves = new ValidMoveSet(x, y);
                    for (int j = 1; j < 8; ++j)
                    {
                        if (x + j > 7 || y - j < 0)
                            break;
                        validMoves.AddMove(x + j, y - j);
                    }
                    moveListBishop.Add(validMoves);
                    moveListQueen.Add(validMoves);
                }

                if (x > 0 && y < 7)
                {
                    validMoves = new ValidMoveSet(x, y);
                    for (int j = 1; j < 8; ++j)
                    {
                        if (x - j < 0 || y + j > 7)
                            break;
                        validMoves.AddMove(x - j, y + j);
                    }
                    moveListBishop.Add(validMoves);
                    moveListQueen.Add(validMoves);
                }

                if (x > 0 && y > 0)
                {
                    validMoves = new ValidMoveSet(x, y);
                    for (int j = 1; j < 8; ++j)
                    {
                        if (x - j < 0 || y - j < 0)
                            break;
                        validMoves.AddMove(x - j , y - j);
                    }
                    moveListBishop.Add(validMoves);
                    moveListQueen.Add(validMoves);
                }

                // Rook/queen
                if (x < 7)
                {
                    validMoves = new ValidMoveSet(x, y);
                    for (int j = 1; j < 8; ++j)
                    {
                        if (x + j > 7)
                            break;
                        validMoves.AddMove(x + j, y);
                    }
                    moveListRook.Add(validMoves);
                    moveListQueen.Add(validMoves);
                }
                
                if (x > 0)
                {
                    validMoves = new ValidMoveSet(x, y);
                    for (int j = 1; j < 8; ++j)
                    {
                        if (x - j < 0)
                            break;
                        validMoves.AddMove(x - j, y);
                    }
                    moveListRook.Add(validMoves);
                    moveListQueen.Add(validMoves);
                }

                if (y < 7)
                {
                    validMoves = new ValidMoveSet(x, y);
                    for (int j = 1; j < 8; ++j)
                    {
                        if (y + j > 7)
                            break;
                        validMoves.AddMove(x, y + j);
                    }
                    moveListRook.Add(validMoves);
                    moveListQueen.Add(validMoves);
                }
                
                if (y > 0)
                {
                    validMoves = new ValidMoveSet(x, y);
                    for (int j = 1; j < 8; ++j)
                    {
                        if (y - j < 0)
                            break;
                        validMoves.AddMove(x, y - j);
                    }

                    moveListRook.Add(validMoves);
                    moveListQueen.Add(validMoves);
                }

                // King
                validMoves = new ValidMoveSet(x, y, ValidMoveCaptureRule.None, false);
                int[] kingMoves = new int[16] { -1, -1, 0, -1, 1, -1, -1, 0, 1, 0, -1, 1, 0, 1, 1, 1 };
                for (int j = 0; j < 16; j += 2)
                {
                    if (x + kingMoves[j] < 0 || x + kingMoves[j] > 7 || y + kingMoves[j + 1] < 0 || y + kingMoves[j + 1] > 7)
                        continue;
                    validMoves.AddMove(x + kingMoves[j], y + kingMoves[j + 1]);
                }
                moveListKing.Add(validMoves);

                // Todo: Castling

                // Todo: Promotion

                // Store
                FieldMoves[i][ChessPiece.BPawn] = moveListBlackPawn;
                FieldMoves[i][ChessPiece.WPawn] = moveListWhitePawn;
                FieldMoves[i][ChessPiece.BKnight] = moveListKnight;
                FieldMoves[i][ChessPiece.WKnight] = moveListKnight;
                FieldMoves[i][ChessPiece.BBishop] = moveListBishop;
                FieldMoves[i][ChessPiece.WBishop] = moveListBishop;
                FieldMoves[i][ChessPiece.BRook] = moveListRook;
                FieldMoves[i][ChessPiece.WRook] = moveListRook;
                FieldMoves[i][ChessPiece.BQueen] = moveListQueen;
                FieldMoves[i][ChessPiece.WQueen] = moveListQueen;
                FieldMoves[i][ChessPiece.BKing] = moveListKing;
                FieldMoves[i][ChessPiece.WKing] = moveListKing;
            }
        }

        public ChessBoard DeepClone()
        {
            ChessBoard clone = new ChessBoard();
            for (int i = 0; i < 64; ++i)
            {
                if (this.Fields[i] != ChessPiece.None)
                {
                    clone.Fields[i] = this.Fields[i];
                }
            }
            foreach (var piece in this.Black)
            {
                clone.Black.Add(piece);
            }
            foreach (var piece in this.White)
            {
                clone.White.Add(piece);
            }
            clone.Turn = this.Turn;
            clone.Hash = this.Hash;
            clone.Depth = this.Depth;

            return clone;
        }

        public void Clear()
        {
            this.Fields = new ChessPiece[64];
            this.White = new List<int>();
            this.Black = new List<int>();
            this.Turn = ChessPiece.White;
            this.Hash = 0;
        }

        public void Reset()
        {
            Clear();

            Add(0, ChessPiece.WRook);
            Add(1, ChessPiece.WKnight);
            Add(2, ChessPiece.WBishop);
            Add(3, ChessPiece.WQueen);
            Add(4, ChessPiece.WKing);
            Add(5, ChessPiece.WBishop);
            Add(6, ChessPiece.WKnight);
            Add(7, ChessPiece.WRook);

            Add(56 + 0, ChessPiece.BRook);
            Add(56 + 1, ChessPiece.BKnight);
            Add(56 + 2, ChessPiece.BBishop);
            Add(56 + 3, ChessPiece.BQueen);
            Add(56 + 4, ChessPiece.BKing);
            Add(56 + 5, ChessPiece.BBishop);
            Add(56 + 6, ChessPiece.BKnight);
            Add(56 + 7, ChessPiece.BRook);

            for (int i = 0; i < 8; ++i)
            {
                Add(8 + i, ChessPiece.WPawn);
                Add(48 + i, ChessPiece.BPawn);
            }

            Console.WriteLine("Hash: " + this.Hash);
        }

        public void LoadFromFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                Clear();

                int i = 56;
                var lines = File.ReadAllLines(fileName);
                foreach (var _line in lines)
                {
                    var line = _line.Trim();
                    if (line.StartsWith("//"))
                    {
                        continue;
                    }

                    // Max 8 chess pieces per line
                    if (line.Length > 8)
                    {
                        line = line.Substring(0, 8);
                    }

                    // Enumerate letters on line
                    foreach (var letter in line.ToCharArray())
                    {
                        switch (letter)
                        {
                            case 'P':
                                Add(i, ChessPiece.WPawn); break;
                            case 'R':
                                Add(i, ChessPiece.WRook); break;
                            case 'N':
                                Add(i, ChessPiece.WKnight); break;
                            case 'B':
                                Add(i, ChessPiece.WBishop); break;
                            case 'Q':
                                Add(i, ChessPiece.WQueen); break;
                            case 'K':
                                Add(i, ChessPiece.WKing); break;
                            case 'p':
                                Add(i, ChessPiece.BPawn); break;
                            case 'r':
                                Add(i, ChessPiece.BRook); break;
                            case 'n':
                                Add(i, ChessPiece.BKnight); break;
                            case 'b':
                                Add(i, ChessPiece.BBishop); break;
                            case 'q':
                                Add(i, ChessPiece.BQueen); break;
                            case 'k':
                                Add(i, ChessPiece.BKing); break;
                            default:
                                break;
                        }
                        i++;
                    }
                    i -= 16;

                    // Stop after 8 lines
                    if (i < 0)
                    {
                        break;
                    }
                }
            }
        }

        public void ChangeTurn()
        {
            this.Turn = this.Turn == ChessPiece.White ? ChessPiece.Black : ChessPiece.White;
        }

        private void Add(int position, ChessPiece piece)
        {
            this.Hash ^= this.HashTable[(int)piece*64 + position];
            this.Fields[position] = piece;
            if ((piece & ChessPiece.Suit) == ChessPiece.White)
            {
                this.White.Add(position);
            }
            else
            {
                this.Black.Add(position);
            }
        }

        private void Remove(int position)
        {
            this.Hash ^= this.HashTable[(int)this.Fields[position] * 64 + position];
            if ((this.Fields[position] & ChessPiece.Suit) == ChessPiece.White)
            {
                this.White.Remove(position);
            }
            else
            {
                this.Black.Remove(position);
            }
            this.Fields[position] = ChessPiece.None;
        }

        public void DoMove(ChessMove move, out ChessPiece chessPiece)
        {
            chessPiece = this.Fields[move.To];
            DoMove(move);
        }

        public void DoMove(ChessMove move)
        {
            var chessPiece = this.Fields[move.From];
            if (this.Fields[move.To] != ChessPiece.None)
            {
                Remove(move.To);
            }
            Remove(move.From);
            Add(move.To, chessPiece);
            this.Hash ^= 0x80000000;
        }

        public void UndoMove(ChessMove move, ChessPiece capturedChessPiece)
        {
            var chessPiece = this.Fields[move.To];
            Add(move.From, chessPiece);
            Remove(move.To);
            if (capturedChessPiece != ChessPiece.None)
            {
                Add(move.To, capturedChessPiece);
            }
            this.Hash ^= 0x80000000;
        }

        public bool InCheck(ChessPiece suit)
        {
            var myChessPieces = suit == ChessPiece.Black ? this.Black : this.White;

            foreach (var c in myChessPieces)
            {
                if ((this.Fields[c] & ChessPiece.Type) == ChessPiece.King)
                {
                    return GetAllMoves(suit == ChessPiece.Black ? ChessPiece.White : ChessPiece.Black).Any(m => m.To == c);
                }
            }

            return false;
        }

        public List<ChessMove> GetAllMoves(ChessPiece suit)
        {
            var myChessPieces = suit == ChessPiece.Black ? this.Black : this.White;

            var moves = new List<ChessMove>();

            foreach (var chessPiece in myChessPieces)
            {
                foreach (var moveSet in this.FieldMoves[chessPiece][this.Fields[chessPiece]])
                {
                    foreach (var move in moveSet.Moves)
                    {
                        if (this.Fields[move.To] == ChessPiece.None)
                        {
                            if (moveSet.CaptureRule != ValidMoveCaptureRule.MustCapture)
                            {
                                moves.Add(new ChessMove(this.Fields[chessPiece], chessPiece, move.To));
                            }
                        }
                        else if ((Fields[move.To] & ChessPiece.Suit) != suit)
                        {
                            if (moveSet.CaptureRule != ValidMoveCaptureRule.MustNotCapture)
                            {
                                moves.Add(new ChessMove(this.Fields[chessPiece], chessPiece, move.To));
                            }

                            if (moveSet.Breakable)
                            {
                                break;
                            }
                        }
                        else //if (Game.Board.Fields[moveTo].Suit == MySuit)
                        {
                            if (moveSet.Breakable)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            return moves;
        }

        public bool IsMate(ref MateType mateType)
        {
            // Check if anything is possible without being in check
            var testBoard = this.DeepClone();
            var allMoves = testBoard.GetAllMoves(this.Turn);
            bool mate = true;
            foreach (var move in allMoves)
            {
                ChessPiece capturedChessPiece;
                testBoard.DoMove(move, out capturedChessPiece);

                //var myKing = (Board.Turn == Suit.Black ? Board.Black : Board.White).First(c => c.Type == ChessPieceType.King);
                //var allMovesOpponent = testBoard.GetAllMoves(Board.Turn == Suit.Black ? Suit.White : Suit.Black);
                //if (!allMovesOpponent.Any(m => m.To == myKing.Location))
                if (!testBoard.InCheck(this.Turn))
                {
                    mate = false;
                    testBoard.UndoMove(move, capturedChessPiece);
                    break;
                }
                testBoard.UndoMove(move, capturedChessPiece);
            }

            if (mate)
            {
                mateType = testBoard.InCheck(this.Turn) ? MateType.Mate : MateType.StaleMate;
            }

            return mate;
        }

        static ChessPiece[] BothSuits = new ChessPiece[] { ChessPiece.Black, ChessPiece.White };

        public Int32 GetMaterialValue(Int32[] chessPieceRelativeValues, Int32 depth)
        {
            Int32 value = 0;
            bool kingIsAlive = false, opponentKingIsAlive = false;
            foreach (var chessPiece in this.Black)
            {
                if ((this.Fields[chessPiece] & ChessPiece.Type) == ChessPiece.King)
                {
                    kingIsAlive = true;
                }

                value += chessPieceRelativeValues[(int)(this.Fields[chessPiece] & ChessPiece.Type)];
            }
            foreach (var chessPiece in this.White)
            {
                if ((this.Fields[chessPiece] & ChessPiece.Type) == ChessPiece.King)
                {
                    opponentKingIsAlive = true;
                }

                value -= chessPieceRelativeValues[(int)(this.Fields[chessPiece] & ChessPiece.Type)];
            }

            return kingIsAlive ? (opponentKingIsAlive ? value : (1000 + 100*depth)) : -(1000 + 100 * depth);
        }

        public Int32 GetPositionalValue()//out bool canMove)
        {
            //canMove = false;
            Int32 mobilityValue = 0;
            Int32 pawnAdvancement = 0;

            foreach (var suit in BothSuits)
            {
                var m = (suit == ChessPiece.Black ? 1 : -1);

                foreach (var chessPiece in suit == ChessPiece.Black ? this.Black : this.White)
                {
                    // Mobility
                    foreach (var moveSet in this.FieldMoves[chessPiece][this.Fields[chessPiece]])
                    {
                        foreach (var move in moveSet.Moves)
                        {
                            if (this.Fields[move.To] == ChessPiece.None)
                            {
                                if (moveSet.CaptureRule != ValidMoveCaptureRule.MustCapture)
                                {
                                    mobilityValue += m;
                                    //canMove = true;
                                }
                                continue;
                            }
                            if ((this.Fields[move.To] & ChessPiece.Suit) != suit && moveSet.CaptureRule != ValidMoveCaptureRule.MustNotCapture)
                            {
                                mobilityValue += m;
                                //canMove = true;
                            }
                            if (moveSet.Breakable)
                            {
                                break;
                            }
                        }
                    }

                    // Pawn advancement
                    if ((this.Fields[chessPiece] & ChessPiece.Type) == ChessPiece.Pawn)
                    {
                        var advancement = (this.Fields[chessPiece] & ChessPiece.Suit) == ChessPiece.Black ? 
                            (6 - (chessPiece / 8)) :
                            ((chessPiece / 8) - 1);
                        pawnAdvancement += m * advancement;
                    }
                }
            }

            return mobilityValue + pawnAdvancement;
        }
    }
}
