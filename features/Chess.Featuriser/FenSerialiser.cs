using System.Collections.Generic;
using System.Text;

namespace Chess.Featuriser
{
    public class FenSerialiser
    {
        public string Serialise(BoardState boardState)
        {
            var result = new StringBuilder();

            AppendPiecePlacement(boardState, result);
            AppendFlags(boardState, result);

            return result.ToString();
        }

        private void AppendPiecePlacement(BoardState boardState, StringBuilder result)
        {
            for (var rank = 7; rank >= 0; rank--)
            {
                if (rank < 7)
                {
                    result.Append("/");
                }

                var emptyCount = 0;
                for (var file = 0; file < 8; file++)
                {
                    var piece = boardState.Squares[rank, file];

                    if (piece == null)
                    {
                        emptyCount++;
                        continue;
                    }

                    if (emptyCount > 0)
                    {
                        result.Append(emptyCount.ToString());
                        emptyCount = 0;
                    }
                    result.Append(ToFenString(piece));
                }

                if (emptyCount > 0)
                {
                    result.Append(emptyCount.ToString());
                }
            }
        }

        private static readonly Dictionary<PieceType, string> PieceAbbreviationLookup = new Dictionary
            <PieceType, string>
        {
            {PieceType.Pawn, "p"},
            {PieceType.Knight, "n"},
            {PieceType.Bishop, "b"},
            {PieceType.Rook, "r"},
            {PieceType.Queen, "q"},
            {PieceType.King, "k"}
        };

        private string ToFenString(Piece piece)
        {
            var abbreviation = PieceAbbreviationLookup[piece.PieceType];
            if (piece.IsWhite)
            {
                abbreviation = abbreviation.ToUpper();
            }
            return abbreviation;
        }

        private void AppendFlags(BoardState boardState, StringBuilder result)
        {
            result.Append(boardState.IsWhite ? " w " : " b ");

            result.Append(boardState.WhiteCastleShort ? "K" : string.Empty);
            result.Append(boardState.WhiteCastleLong ? "Q" : string.Empty);
            result.Append(boardState.BlackCastleShort ? "k" : string.Empty);
            result.Append(boardState.BlackCastleLong ? "q" : string.Empty);

            var castlingRights = boardState.WhiteCastleShort ||
                                 boardState.WhiteCastleLong ||
                                 boardState.BlackCastleShort ||
                                 boardState.BlackCastleLong;

            result.Append(castlingRights ? " " : "- ");

            var ep = boardState.EnPassantTarget?.ToString();
            if (string.IsNullOrEmpty(ep))
            {
                ep = "-";
            }
            result.Append($"{ep} {boardState.HalfMoveClock} {boardState.MoveNumber}");
        }
    }
}
