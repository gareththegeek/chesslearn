using Chess.Featuriser.Cli;
using Chess.Featuriser.State;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess.Featuriser.Pgn
{
    public class PgnStateGenerator
    {
        public IEnumerable<BoardState> GenerateStates(PgnGame game)
        {
            var results = new List<BoardState>();
            var boardState = BoardState.Initial();

            var i = 1;
            foreach (var move in game.Moves)
            {
                try
                {
                    results.Add(boardState.Clone());

                    AdvanceBoardState(boardState, move);

                    i += 1;
                }
                catch(Exception)
                {
                    ConsoleHelper.PrintWarning($"Error on move {i}");
                    throw;
                }
            }

            results.Add(boardState);

            return results;
        }

        private void AdvanceBoardState(BoardState boardState, PgnMove move)
        {
            ApplyMove(boardState, move);
            AdvanceCounters(boardState);
        }

        private void ApplyMove(BoardState boardState, PgnMove move)
        {
            boardState.Move = move;
            boardState.EnPassantTarget = null;

            if ((move.Flags & (int)PgnMoveFlags.Castle) != 0)
            {
                Castle(boardState, move);
                return;
            }

            if (move.Square.Rank == null || move.Square.File == null)
                throw new InvalidOperationException("Move must have destination square");

            var origin = FindOrigin(boardState, move);
            var destination = move.Square.ToSquare();

            Move(boardState, origin, destination);

            if ((move.Flags & (int)PgnMoveFlags.Promote) != 0)
            {
                if (move.Promotion == null)
                    throw new InvalidOperationException("When promoting piece, must specify new piece type");

                boardState.Squares[destination.Rank, destination.File].PieceType = move.Promotion.Value;
            }
        }

        private void Castle(BoardState boardState, PgnMove move)
        {
            var white = boardState.IsWhite;
            var castleShort = (move.Flags & (int)PgnMoveFlags.CastleShort) != 0;

            int rank;

            if (white)
            {
                boardState.WhiteCastleShort = false;
                boardState.WhiteCastleLong = false;

                rank = 0;
            }
            else
            {
                boardState.BlackCastleShort = false;
                boardState.BlackCastleLong = false;

                rank = 7;
            }

            if (castleShort)
            {
                Move(boardState, new Square(rank, 4), new Square(rank, 6));
                Move(boardState, new Square(rank, 7), new Square(rank, 5));
            }
            else
            {
                Move(boardState, new Square(rank, 4), new Square(rank, 2));
                Move(boardState, new Square(rank, 0), new Square(rank, 3));
            }
        }

        private void Move(BoardState boardState, Square origin, Square destination)
        {
            var piece = boardState.Squares[origin.Rank, origin.File];
            boardState.Squares[destination.Rank, destination.File] = piece;
            boardState.Squares[origin.Rank, origin.File] = null;
            piece.Square = destination;
        }

        private Square FindOrigin(BoardState boardState, PgnMove move)
        {
            switch (move.PieceType)
            {
                case PieceType.Pawn:
                    return FindPawnOrigin(boardState, move);
                case PieceType.Knight:
                    return FindKnightOrigin(boardState, move);
                case PieceType.Bishop:
                    return FindBishopOrigin(boardState, move);
                case PieceType.Rook:
                    return FindRookOrigin(boardState, move);
                case PieceType.Queen:
                    return FindQueenOrigin(boardState, move);
                case PieceType.King:
                    return FindKingOrigin(boardState, move);
                default:
                    throw new InvalidOperationException($"Unexpected piece type {move.PieceType}");
            }
        }

        private Square FindPawnOrigin(BoardState boardState, PgnMove move)
        {
            if ((move.Flags & (int)PgnMoveFlags.Take) != 0)
            {
                return FindPawnTakeOrigin(boardState, move);
            }
            return FindPawnMoveOrigin(boardState, move);
        }

        private Square FindPawnMoveOrigin(BoardState boardState, PgnMove move)
        {
            var direction = boardState.IsWhite ? 1 : -1;

            var pawns = boardState
                .Flatten()
                .Where(x => x.PieceType == PieceType.Pawn &&
                            x.IsWhite == boardState.IsWhite)
                .Where(x => x.Square.File == move.Square.File)
                .ToList();

            var singleMovePawns = pawns.Where(x => x.Square.Rank + direction == move.Square.Rank).ToList();
            var doubleMovePawns = pawns.Where(x => x.Square.Rank + direction * 2 == move.Square.Rank).ToList();

            var doubleMove = !singleMovePawns.Any();

            pawns = doubleMove ? doubleMovePawns : singleMovePawns;

            var square = pawns.Single().Square;

            if (doubleMove)
            {
                boardState.EnPassantTarget = new Square(square.Rank + direction, square.File);
            }

            return pawns.Single().Square;
        }

        private Square FindPawnTakeOrigin(BoardState boardState, PgnMove move)
        {
            if (move.Square.Rank == null || move.Square.File == null) throw new InvalidOperationException("Move must have destination square");

            var direction = boardState.IsWhite ? 1 : -1;

            var pawns = boardState
                .Flatten()
                .Where(x => x.PieceType == PieceType.Pawn &&
                            x.IsWhite == boardState.IsWhite)
                .Where(x => Math.Abs(x.Square.File - move.Square.File.Value) == 1 &&
                                x.Square.Rank + direction == move.Square.Rank.Value)
                .ToList();

            return Disambiguate(move, pawns).Square;
        }

        private Square FindKnightOrigin(BoardState boardState, PgnMove move)
        {
            if (move.Square.Rank == null || move.Square.File == null) throw new InvalidOperationException("Move must have destination square");

            var knights = boardState
                .Flatten()
                .Where(x => x.PieceType == PieceType.Knight &&
                            x.IsWhite == boardState.IsWhite)
                .Where(x => (Math.Abs(x.Square.Rank - move.Square.Rank.Value) == 2 &&
                             Math.Abs(x.Square.File - move.Square.File.Value) == 1)
                            ||
                            (Math.Abs(x.Square.Rank - move.Square.Rank.Value) == 1 &&
                             Math.Abs(x.Square.File - move.Square.File.Value) == 2))
                .ToList();

            return Disambiguate(move, knights).Square;
        }

        private Square FindBishopOrigin(BoardState boardState, PgnMove move)
        {
            if (move.Square.Rank == null || move.Square.File == null) throw new InvalidOperationException("Move must have destination square");

            var shops = boardState
                .Flatten()
                .Where(x => x.PieceType == PieceType.Bishop &&
                            x.IsWhite == boardState.IsWhite)
                .Where(x => Math.Abs(x.Square.Rank - move.Square.Rank.Value) == Math.Abs(x.Square.File - move.Square.File.Value))
                .Where(x => boardState.PathIsClear(x.Square, move.Square.ToSquare()))
                .ToList();

            return Disambiguate(move, shops).Square;
        }

        private Square FindRookOrigin(BoardState boardState, PgnMove move)
        {
            if (move.Square.Rank == null || move.Square.File == null) throw new InvalidOperationException("Move must have destination square");

            var rooks = boardState
                .Flatten()
                .Where(x => x.PieceType == PieceType.Rook &&
                            x.IsWhite == boardState.IsWhite)
                .Where(x => x.Square.Rank == move.Square.Rank ||
                            x.Square.File == move.Square.File)
                .Where(x => boardState.PathIsClear(x.Square, move.Square.ToSquare()))
                .ToList();

            var origin = Disambiguate(move, rooks).Square;

            if (origin.File == 0)
            {
                if (origin.Rank == 0)
                {
                    boardState.WhiteCastleLong = false;
                }
                else if (origin.Rank == 7)
                {
                    boardState.BlackCastleLong = false;
                }
            }
            else if (origin.File == 7)
            {
                if (origin.Rank == 0)
                {
                    boardState.WhiteCastleShort = false;
                }
                else if (origin.Rank == 7)
                {
                    boardState.BlackCastleShort = false;
                }
            }

            return origin;
        }

        private Square FindQueenOrigin(BoardState boardState, PgnMove move)
        {
            if (move.Square.Rank == null || move.Square.File == null) throw new InvalidOperationException("Move must have destination square");

            var queen = boardState
                .Flatten()
                .Single(x => x.PieceType == PieceType.Queen &&
                             x.IsWhite == boardState.IsWhite);

            return queen.Square;
        }

        private Square FindKingOrigin(BoardState boardState, PgnMove move)
        {
            var king = boardState
                .Flatten()
                .Single(x => x.PieceType == PieceType.King &&
                             x.IsWhite == boardState.IsWhite);

            if (boardState.IsWhite)
            {
                boardState.WhiteCastleShort = false;
                boardState.WhiteCastleLong = false;
            }
            else
            {
                boardState.BlackCastleShort = false;
                boardState.BlackCastleLong = false;
            }

            return king.Square;
        }

        private static Piece Disambiguate(PgnMove move, List<Piece> pieces)
        {
            if (pieces.Count == 2)
            {
                pieces = pieces
                    .Where(x => x.Square.Rank == move.Origin.Rank ||
                                x.Square.File == move.Origin.File)
                    .ToList();
            }

            //1.b3 e5 2.Bb2 Nc6 3.e3 g6 4.f4 Bg7 5.Nf3 d6 6.Bb5 Ne7 7.fxe5 O - O 8.O - O dxe5 9.Bxc6 Nxc6 10.e4 f5 11.exf5 e4 12.Bxg7 Kxg7 0 - 1
            // Problem false positive ambiguous move: 6 ..Ne7
            // The system thinks it is ambiguous but it isn't.
            // The Nc6 cannot move as he is pinned :(


            return pieces.Single();
        }

        private static void AdvanceCounters(BoardState boardState)
        {
            boardState.IsWhite = !boardState.IsWhite;
            boardState.MoveNumber += boardState.IsWhite ? 1 : 0;
            boardState.HalfMoveClock = boardState.IsWhite ? 0 : 1;
        }
    }
}
