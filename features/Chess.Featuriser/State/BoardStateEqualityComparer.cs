using System.Collections.Generic;

namespace Chess.Featuriser.State
{
    public class BoardStateEqualityComparer : IEqualityComparer<BoardState>
    {
        public bool Equals(BoardState x, BoardState y)
        {
            return x.Fen == y.Fen;
        }

        public int GetHashCode(BoardState obj)
        {
            return obj.Fen.GetHashCode();
        }
    }
}
