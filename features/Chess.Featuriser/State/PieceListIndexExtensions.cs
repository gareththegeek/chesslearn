using System.Collections.Generic;

namespace Chess.Featuriser.State
{
    public static class PieceListIndexExtensions
    {
        private static readonly Dictionary<PieceListIndex, PieceType> PieceTypeLookup = new Dictionary<PieceListIndex, PieceType>
        {
            {PieceListIndex.Queen, PieceType.Queen},
            {PieceListIndex.Rook1, PieceType.Rook},
            {PieceListIndex.Rook2, PieceType.Rook},
            {PieceListIndex.Bishop1, PieceType.Bishop},
            {PieceListIndex.Bishop2, PieceType.Bishop},
            {PieceListIndex.King, PieceType.King},
            {PieceListIndex.Knight1, PieceType.Knight},
            {PieceListIndex.Knight2, PieceType.Knight},
            {PieceListIndex.PawnA, PieceType.Pawn},
            {PieceListIndex.PawnB, PieceType.Pawn},
            {PieceListIndex.PawnC, PieceType.Pawn},
            {PieceListIndex.PawnD, PieceType.Pawn},
            {PieceListIndex.PawnE, PieceType.Pawn},
            {PieceListIndex.PawnF, PieceType.Pawn},
            {PieceListIndex.PawnG, PieceType.Pawn},
            {PieceListIndex.PawnH, PieceType.Pawn}
        };

        public static PieceType ToPieceType(this PieceListIndex index)
        {
            return PieceTypeLookup[index];
        }
    }
}
