using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chess.Featuriser.Pgn;
using Chess.Featuriser.State;
using Chess.Featuriser.Cl;

namespace Chess.Featuriser
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Banner.Print("Chess Featuriser");

                var options = new Configurator<Options>().BuildOptions(args);

                if (options == null)
                {
                    return;
                }

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

                if (options.Debug)
                {
                    DebugOutput.Debug(games);
                }

                if (!string.IsNullOrEmpty(options.Output))
                {
                    using (var stream = new FileStream(options.Output, FileMode.Create, FileAccess.Write))
                    {
                        var stateSerialiser = new StateSerialiser();
                        stateSerialiser.Serialise(games, stream, options.Features, options.Fen);
                        Console.WriteLine($"Wrote output file: {options.Output}");
                    }
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintError($"Unexpected error: {ex.Message}");
            }

            Console.WriteLine("Press enter to quit");
            Console.ReadLine();
        }
    }
}
