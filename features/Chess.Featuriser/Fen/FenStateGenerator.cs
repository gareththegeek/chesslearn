using Chess.Featuriser.State;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Chess.Featuriser.Fen
{
    public class FenStateGenerator
    {
        private static string[] PieceLookup = { "p", "n", "b", "r", "q", "k" };
        private Dictionary<ColourPieceType, int> pieceListIndexDictionary;

        private struct ColourPieceType
        {
            public bool IsWhite { get; set; }
            public PieceType PieceType { get; set; }
        }

        private enum FenTokens
        {
            Squares = 0,
            IsWhite,
            CastlingRights,
            EnPassant,
            HalfMoveClock,
            MoveNumber
        }
        
        public BoardState Generate(string fen)
        {
            pieceListIndexDictionary = new Dictionary<ColourPieceType, int>();

            return ParseFen(fen);
        }

        private BoardState ParseFen(string fen)
        {
            var tokens = fen.Split(' ');

            var result = new BoardState();

            ParseSquares(tokens[(int)FenTokens.Squares], result);

            ParseState(tokens, result);

            return result;
        }

        private void ParseSquares(string squares, BoardState result)
        {
            var tokens = squares.Split('/');

            for (var rank = 0; rank < 8; rank++)
            {
                var rankToken = tokens[rank];

                var file = 0;
                var index = 0;
                while (file < 8)
                {
                    var current = rankToken.Substring(index++, 1);
                    if (int.TryParse(current, out int number))
                    {
                        file += number;
                    }
                    else
                    {
                        var piece = ParsePiece(current);
                        result.Squares[7 - rank, file] = piece;
                        piece.Square = new Square(7 - rank, file);

                        file += 1;
                    }
                }
            }
        }

        private Piece ParsePiece(string current)
        {
            var pieceType = (PieceType)Array.IndexOf(PieceLookup, current.ToLower());
            var isWhite = Char.IsUpper(current[0]);

            var key = new ColourPieceType { IsWhite = isWhite, PieceType = pieceType };

            if (!pieceListIndexDictionary.TryGetValue(key, out int count))
            {
                count = 0;
            }

            pieceListIndexDictionary[key] = count + 1;

            PieceListIndex pieceListIndex;
            switch (pieceType)
            {
                case PieceType.Pawn:
                    //TODO more intelligent pawn assignment
                    pieceListIndex = PieceListIndex.PawnA + count;
                    break;
                case PieceType.Knight:
                    pieceListIndex = PieceListIndex.Knight1 + count;
                    break;
                case PieceType.Bishop:
                    pieceListIndex = PieceListIndex.Bishop1 + count;
                    break;
                case PieceType.Rook:
                    pieceListIndex = PieceListIndex.Rook1 + count;
                    break;
                case PieceType.Queen:
                    //TODO what about promotion :/
                    pieceListIndex = PieceListIndex.Queen;
                    break;
                case PieceType.King:
                    pieceListIndex = PieceListIndex.King;
                    break;
                default:
                    throw new InvalidOperationException($"Unknown piece type {pieceType}");
            }

            return new Piece(pieceType, pieceListIndex, isWhite);
        }

        private void ParseState(string[] tokens, BoardState result)
        {
            result.IsWhite = tokens[(int)FenTokens.IsWhite].ToLower() == "w";

            var castling = tokens[(int)FenTokens.CastlingRights];
            result.WhiteCastleShort = castling.Contains('K');
            result.WhiteCastleLong = castling.Contains('Q');
            result.BlackCastleShort = castling.Contains('k');
            result.BlackCastleLong = castling.Contains('q');

            var ep = tokens[(int)FenTokens.EnPassant].ToLower();
            if (ep != "-")
            {
                result.EnPassantTarget = new Square(ep[1] - '1', ep[0] - 'a');
            }

            result.HalfMoveClock = int.Parse(tokens[(int)FenTokens.HalfMoveClock]);
            result.MoveNumber = int.Parse(tokens[(int)FenTokens.MoveNumber]);
        }
    }
}
