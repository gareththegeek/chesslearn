using System;
using System.Collections.Generic;
using Chess.Featuriser.Pgn;
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

                var inputDeserialiser = new InputDeserialiser();
                IEnumerable<PgnGame> games = inputDeserialiser.Deserialise(options);

                if (options.Debug)
                {
                    DebugOutput.Debug(games);
                }

                if (!string.IsNullOrEmpty(options.Output))
                {
                    var outputSerialiser = new OutputSerialiser();
                    outputSerialiser.Serialise(games, options);
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
