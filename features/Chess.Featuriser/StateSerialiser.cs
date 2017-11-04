using System.Collections.Generic;
using System.IO;
using Chess.Featuriser.Pgn;

namespace Chess.Featuriser
{
    public class StateSerialiser
    {
        public void Serialise(List<PgnGame> games, Stream stream)
        {
            var stateGenerator = new PgnStateGenerator();

            using (var sw = new StreamWriter(stream))
            {
                foreach (var game in games)
                {
                    foreach (var state in stateGenerator.GenerateStates(game))
                    {
                        Serialise(state, sw);
                    }
                }
            }
        }

        private void Serialise(BoardState state, StreamWriter sw)
        {
            
        }
    }
}
