using Chess.Featuriser.Cli;
using Chess.Featuriser.Fen;
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

        public IEnumerable<string> Deserialise(Options options)
        {
            Console.WriteLine("");
            Console.WriteLine("DESERIALISING INPUT");

            var extension = Path.GetExtension(options.Input).ToLower();
            IEnumerable<string> fens;
            switch (extension)
            {
                case ".pgn":
                    fens = DeserialisePgn(options);
                    break;
                case ".csv":
                    fens = DeserialiseCsv(options);
                    break;
                default:
                    throw new InvalidOperationException($"Unknown input file extension {extension}");
            }

            if (options.Unique)
            {
                Console.WriteLine();
                Console.WriteLine("REMOVING DUPLICATES");
                Console.WriteLine($"{fens.Count():n0} fens");
                fens = fens.Distinct().ToList();
                Console.WriteLine($"{fens.Count():n0} unique fens");
            }

            return fens;
        }

        private IEnumerable<string> DeserialisePgn(Options options)
        {
            var startTime = DateTime.Now;

            Console.WriteLine("Scanning pgn text");
            IEnumerable<PgnToken> tokens;
            using (var stream = new FileStream(options.Input, FileMode.Open))
            {
                var scanner = new PgnScanner();
                tokens = scanner.Scan(stream).ToList();
            }
            Console.WriteLine($"Scanned {tokens.Count():n0} tokens in {(DateTime.Now - startTime).TotalSeconds:f2}s");
            startTime = DateTime.Now;

            Console.WriteLine("Parsing pgn games");
            var parser = new PgnParser();
            var fens = parser.Parse(tokens);
            Console.WriteLine($"Parsed {fens.Count():n0} fens in {(DateTime.Now - startTime).TotalSeconds:f2}s");
            startTime = DateTime.Now;

            return fens;
        }

        private IEnumerable<string> DeserialiseCsv(Options options)
        {
            var startTime = DateTime.Now;

            Console.WriteLine("Scanning csv text");
            IEnumerable<string> fens;
            using (var stream = new FileStream(options.Input, FileMode.Open, FileAccess.Read))
            {
                var scanner = new CsvScanner();
                fens = scanner.Scan(stream);
            }
            Console.WriteLine($"Read {fens.Count():n0} fens in {(DateTime.Now - startTime).TotalSeconds:f2}s");

            return fens;
        }
    }
}
