using System.Collections.Generic;

namespace Chess.Featuriser.State
{
    public static class PieceExtensions
    {
        private static readonly Dictionary<PieceType, string> PieceNameLookup = new Dictionary<PieceType, string>
        {
            { PieceType.Pawn, string.Empty },
            { PieceType.Knight, "N" },
            { PieceType.Bishop, "B" },
            { PieceType.Rook, "R" },
            { PieceType.Queen, "Q" },
            { PieceType.King, "K" }
        };

        public static string GetAbbreviation(this PieceType pieceType)
        {
            return PieceNameLookup[pieceType];
        }
    }
}
