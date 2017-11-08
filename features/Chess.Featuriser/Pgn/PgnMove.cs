using Chess.Featuriser.State;
using System.Text;

namespace Chess.Featuriser.Pgn
{
    public class PgnMove
    {
        public PieceType PieceType { get; set; }
        public PgnSquare Square { get; set; }
        public PgnSquare Origin { get; set; }
        public byte Flags { get; set; }
        public PieceType? Promotion { get; set; }

        public override string ToString()
        {
            var result = new StringBuilder();

            if ((Flags & (int)PgnMoveFlags.CastleShort) != 0)
            {
                return "O-O";
            }

            if ((Flags & (int)PgnMoveFlags.CastleLong) != 0)
            {
                return "O-O-O";
            }

            result.Append(PieceType.GetAbbreviation());
            result.Append(Origin);
            if ((Flags & (int)PgnMoveFlags.Take) != 0)
            {
                result.Append("x");
            }
            result.Append(Square);

            if ((Flags & (int) PgnMoveFlags.Promote) != 0)
            {
                result.Append("=");
                result.Append(Promotion?.GetAbbreviation());
            }

            return result.ToString();
        }
    }
}