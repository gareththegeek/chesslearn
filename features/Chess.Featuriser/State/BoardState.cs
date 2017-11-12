using Chess.Featuriser.Pgn;
using System.Collections.Generic;

namespace Chess.Featuriser.State
{
    public class BoardState
    {
        public BoardState()
        {
            Squares = new Piece[,]
            {
                {null, null, null, null, null, null, null, null},
                {null, null, null, null, null, null, null, null},
                {null, null, null, null, null, null, null, null},
                {null, null, null, null, null, null, null, null},
                {null, null, null, null, null, null, null, null},
                {null, null, null, null, null, null, null, null},
                {null, null, null, null, null, null, null, null},
                {null, null, null, null, null, null, null, null}
            };
        }

        protected BoardState(BoardState original)
        {
            Move = original.Move;
            IsWhite = original.IsWhite;
            WhiteCastleShort = original.WhiteCastleShort;
            WhiteCastleLong = original.WhiteCastleLong;
            BlackCastleShort = original.BlackCastleShort;
            BlackCastleLong = original.BlackCastleLong;
            MoveNumber = original.MoveNumber;
            HalfMoveClock = original.HalfMoveClock;
            EnPassantTarget = original.EnPassantTarget;

            Squares = new Piece[8, 8];
            for (var rank = 0; rank < 8; rank++)
            {
                for (var file = 0; file < 8; file++)
                {
                    Squares[rank, file] = original.Squares[rank, file]?.Clone();
                }
            }
        }

        public static BoardState Initial()
        {
            var result = new BoardState();

            result.IsWhite = true;
            result.WhiteCastleShort = true;
            result.WhiteCastleLong = true;
            result.BlackCastleShort = true;
            result.BlackCastleLong = true;
            result.MoveNumber = 1;
            result.HalfMoveClock = 0;
            result.EnPassantTarget = null;

            result.Squares = new[,]
            {
                {
                    new Piece(PieceType.Rook, PieceListIndex.Rook1, true),
                    new Piece(PieceType.Knight, PieceListIndex.Knight1, true),
                    new Piece(PieceType.Bishop, PieceListIndex.Bishop1, true),
                    new Piece(PieceType.Queen, PieceListIndex.Queen, true),
                    new Piece(PieceType.King, PieceListIndex.King, true),
                    new Piece(PieceType.Bishop, PieceListIndex.Bishop2, true),
                    new Piece(PieceType.Knight, PieceListIndex.Knight2, true),
                    new Piece(PieceType.Rook, PieceListIndex.Rook2, true)
                },
                {
                    new Piece(PieceType.Pawn, PieceListIndex.PawnA, true),
                    new Piece(PieceType.Pawn, PieceListIndex.PawnB, true),
                    new Piece(PieceType.Pawn, PieceListIndex.PawnC, true),
                    new Piece(PieceType.Pawn, PieceListIndex.PawnD, true),
                    new Piece(PieceType.Pawn, PieceListIndex.PawnE, true),
                    new Piece(PieceType.Pawn, PieceListIndex.PawnF, true),
                    new Piece(PieceType.Pawn, PieceListIndex.PawnG, true),
                    new Piece(PieceType.Pawn, PieceListIndex.PawnH, true)
                },
                {null, null, null, null, null, null, null, null},
                {null, null, null, null, null, null, null, null},
                {null, null, null, null, null, null, null, null},
                {null, null, null, null, null, null, null, null},
                {
                    new Piece(PieceType.Pawn, PieceListIndex.PawnA, false),
                    new Piece(PieceType.Pawn, PieceListIndex.PawnB, false),
                    new Piece(PieceType.Pawn, PieceListIndex.PawnC, false),
                    new Piece(PieceType.Pawn, PieceListIndex.PawnD, false),
                    new Piece(PieceType.Pawn, PieceListIndex.PawnE, false),
                    new Piece(PieceType.Pawn, PieceListIndex.PawnF, false),
                    new Piece(PieceType.Pawn, PieceListIndex.PawnG, false),
                    new Piece(PieceType.Pawn, PieceListIndex.PawnH, false)
                },
                {
                    new Piece(PieceType.Rook, PieceListIndex.Rook1, false),
                    new Piece(PieceType.Knight, PieceListIndex.Knight1, false),
                    new Piece(PieceType.Bishop, PieceListIndex.Bishop1, false),
                    new Piece(PieceType.Queen, PieceListIndex.Queen, false),
                    new Piece(PieceType.King, PieceListIndex.King, false),
                    new Piece(PieceType.Bishop, PieceListIndex.Bishop2, false),
                    new Piece(PieceType.Knight, PieceListIndex.Knight2, false),
                    new Piece(PieceType.Rook, PieceListIndex.Rook2, false)
                }
            };

            for (var rank = 0; rank < 8; rank++)
            {
                for (var file = 0; file < 8; file++)
                {
                    var piece = result.Squares[rank, file];
                    if (piece != null)
                    {
                        piece.Square = new Square(rank, file);
                    }
                }
            }

            return result;
        }

        public PgnMove Move { get; set; }
        public string Fen { get; set; }

        public float? Score { get; set; }

        public bool IsWhite { get; set; }
        public bool WhiteCastleShort { get; set; }
        public bool WhiteCastleLong { get; set; }
        public bool BlackCastleShort { get; set; }
        public bool BlackCastleLong { get; set; }//5
        public int MoveNumber { get; set; }
        public int HalfMoveClock { get; set; }
        public Square EnPassantTarget { get; set; }

        public Piece[,] Squares { get; set; }

        public Dictionary<PieceType, int> WhiteMaterialCount { get; set; }//6
        public Dictionary<PieceType, int> BlackMaterialCount { get; set; }//6

        public PieceListEntry[] WhitePieceList { get; set; }//80
        public PieceListEntry[] BlackPieceList { get; set; }//80

        public List<int>[] WhiteSlidingPieceMobility { get; set; }//24
        public List<int>[] BlackSlidingPieceMobility { get; set; }//24
        
        public PieceType[,] WhiteAttackMap { get; set; }//64
        public PieceType[,] BlackAttackMap { get; set; }//64

        public BoardState Clone()
        {
            return new BoardState(this);
        }
    }
}
