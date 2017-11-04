using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess.Featuriser
{
    public static class BoardStateExtensions
    {
        public static IEnumerable<Piece> Flatten(this BoardState boardState)
        {
            return boardState.Squares
                .Cast<Piece>()
                .Where(x => x != null);
        }

        public static bool PathIsClear(this BoardState boardState, Square origin, Square destination)
        {
            var start = new Square(origin.Rank, origin.File);
            var end = new Square(destination.Rank, destination.File);

            var rankStep = start.Rank == end.Rank ? 0 : 1;
            var fileStep = start.File == end.File ? 0 : 1;

            if (start.Rank > end.Rank)
            {
                rankStep *= -1;
            }
            if (start.File > end.File)
            {
                fileStep *= -1;
            }

            var rank = start.Rank;
            var file = start.File;

            while (rank * rankStep <= end.Rank * rankStep && file * fileStep <= end.File * fileStep)
            {
                //if (rank < 0 || rank > 7 || file < 0 || file > 7) return false;

                rank += rankStep;
                file += fileStep;

                if (rank == end.Rank && file == end.File)
                {
                    return true;
                }

                if (boardState.Squares[rank, file] != null)
                {
                    return false;
                }
            }

            return true;
        }

        public static BoardState Move(this BoardState boardState, Piece piece, Square destination)
        {
            var next = boardState.Clone();
            next.Squares[piece.Square.Rank, piece.Square.File] = null;
            next.Squares[destination.Rank, destination.File] = piece.Clone();
            return next;
        }

        public static bool IsInCheck(this BoardState boardState)
        {
            return IsInCheck(boardState, true) || IsInCheck(boardState, false);
        }

        public static bool IsInCheck(this BoardState boardState, bool kingIsWhite)
        {
            var target = boardState
                .Flatten()
                .SingleOrDefault(x => x.IsWhite == kingIsWhite && x.PieceType == PieceType.King)
                ?.Square;

            if (target == null)
            {
                return true;
            }

            return boardState
                .Flatten()
                .Where(x => x.IsWhite != kingIsWhite)
                .Any(x => boardState.CanTake(x, target, false));
        }

        public static bool CanTake(this BoardState state, Piece piece, Square target, bool considerCheck = true)
        {
            bool canTake;

            switch (piece.PieceType)
            {
                case PieceType.Pawn:
                    canTake = PawnCanTake(piece, target);
                    break;
                case PieceType.Knight:
                    canTake = KnightCanTake(piece, target);
                    break;
                case PieceType.Bishop:
                    canTake = BishopCanTake(state, piece, target);
                    break;
                case PieceType.Rook:
                    canTake = RookCanTake(state, piece, target);
                    break;
                case PieceType.Queen:
                    canTake = QueenCanTake(state, piece, target);
                    break;
                case PieceType.King:
                    canTake = KingCanTake(piece, target);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unexpected PieceType {piece.PieceType}");
            }

            if (!canTake)
            {
                return false;
            }

            if (!considerCheck)
            {
                return true;
            }

            var nextState = state.Move(piece, new Square(target.Rank, target.File));
            return !nextState.IsInCheck();
        }

        private static bool PawnCanTake(Piece piece, Square target)
        {
            var dfile = Math.Abs(piece.Square.File - target.File);
            if (dfile != 1)
            {
                return false;
            }

            var drank = piece.Square.Rank - target.Rank;
            var direction = piece.IsWhite ? -1 : 1;
            if (drank != direction)
            {
                return false;
            }

            return true;
        }

        private static bool KnightCanTake(Piece piece, Square target)
        {
            var drank = Math.Abs(piece.Square.Rank - target.Rank);
            var dfile = Math.Abs(piece.Square.File - target.File);

            if (drank + dfile != 3 || Math.Abs(drank - dfile) == 3)
            {
                return false;
            }

            return true;
        }

        private static bool BishopCanTake(BoardState state, Piece piece, Square target)
        {
            var drank = Math.Abs(piece.Square.Rank - target.Rank);
            var dfile = Math.Abs(piece.Square.File - target.File);

            if (drank != dfile)
            {
                return false;
            }

            if (!state.PathIsClear(piece.Square, new Square(target.Rank, target.File)))
            {
                return false;
            }

            return true;
        }

        private static bool RookCanTake(BoardState state, Piece piece, Square target)
        {
            var drank = Math.Abs(piece.Square.Rank - target.Rank);
            var dfile = Math.Abs(piece.Square.File - target.File);

            if (drank != 0 && dfile != 0)
            {
                return false;
            }

            if (!state.PathIsClear(piece.Square, new Square(target.Rank, target.File)))
            {
                return false;
            }

            return true;
        }

        private static bool QueenCanTake(BoardState state, Piece piece, Square target)
        {
            var drank = Math.Abs(piece.Square.Rank - target.Rank);
            var dfile = Math.Abs(piece.Square.File - target.File);

            if (drank != 0 && dfile != 0 && drank != dfile)
            {
                return false;
            }

            if (!state.PathIsClear(piece.Square, new Square(target.Rank, target.File)))
            {
                return false;
            }

            return true;
        }

        private static bool KingCanTake(Piece piece, Square target)
        {
            var drank = Math.Abs(piece.Square.Rank - target.Rank);
            var dfile = Math.Abs(piece.Square.File - target.File);

            if (drank > 1 || dfile > 1)
            {
                return false;
            }

            if (drank == 0 && dfile == 0)
            {
                return false;
            }

            return true;
        }
    }
}
