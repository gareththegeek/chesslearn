using System.Collections.Generic;
using System.Linq;

namespace Chess.Featuriser.Pgn
{
    public class PgnGame
    {
        public PgnGame()
        {
            Moves = new List<PgnMove>();
        }

        public ICollection<PgnMove> Moves { get; set; }

        public override string ToString()
        {
            return Moves.Aggregate(string.Empty, (x, y) => x + " " + y.ToString());
        }
    }
}