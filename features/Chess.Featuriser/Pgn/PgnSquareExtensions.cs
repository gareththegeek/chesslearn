using System;

namespace Chess.Featuriser.Pgn
{
    public static class PgnSquareExtensions
    {
        public static Square ToSquare(this PgnSquare square)
        {
            if (square.Rank == null) throw new ArgumentNullException(nameof(square.Rank));
            if (square.File == null) throw new ArgumentNullException(nameof(square.File));

            return new Square(square.Rank.Value, square.File.Value);
        }
    }
}
