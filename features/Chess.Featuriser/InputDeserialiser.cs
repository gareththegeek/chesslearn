using Chess.Featuriser.Cl;
using Chess.Featuriser.Pgn;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Chess.Featuriser
{
    public class InputDeserialiser
    {
        public IEnumerable<PgnGame> Deserialise(Options options)
        {
            var startTime = DateTime.Now;

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

            return games;
        }
    }
}
