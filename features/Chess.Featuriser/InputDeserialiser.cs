using Chess.Featuriser.Cl;
using Chess.Featuriser.Pgn;
using Chess.Featuriser.State;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Chess.Featuriser
{
    public class InputDeserialiser
    {
        private const int ReportEvery = 10000;

        public IEnumerable<BoardState> Deserialise(Options options)
        {
            var startTime = DateTime.Now;

            Console.WriteLine("");
            Console.WriteLine("Deserialising input");

            Console.WriteLine("Scanning pgn text");
            IEnumerable<PgnToken> tokens;
            using (var stream = new FileStream(options.Input, FileMode.Open))
            {
                //TODO support csv fen input not just pgn
                var scanner = new PgnScanner();
                tokens = scanner.Scan(stream).ToList();
            }
            Console.WriteLine($"Scanned {tokens.Count()} tokens in {(DateTime.Now - startTime).TotalSeconds}s");
            startTime = DateTime.Now;

            Console.WriteLine("Parsing pgn games");
            var parser = new PgnParser();
            var games = parser.Parse(tokens);
            Console.WriteLine($"Parsed {games.Count()} games in {(DateTime.Now - startTime).TotalSeconds}s");
            startTime = DateTime.Now;

            Console.WriteLine("Generating state information");
            var stateGenerator = new PgnStateGenerator();
            var states = new List<BoardState>();
            var i = 1;
            foreach (var game in games)
            {
                states.AddRange(stateGenerator.GenerateStates(game));

                if (i++ % ReportEvery == 0)
                {
                    Console.WriteLine($"Processed {states.Count} in {(DateTime.Now - startTime).TotalSeconds}s");
                }
            }
            Console.WriteLine($"Processed states. {states.Count} states in {(DateTime.Now - startTime).TotalSeconds}s");

            return states;
        }
    }
}
