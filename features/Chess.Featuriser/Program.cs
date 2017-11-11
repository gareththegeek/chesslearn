using System;
using Chess.Featuriser.Cli;
using Chess.Featuriser.Fen;
using System.Linq;
using Chess.Featuriser.State;

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
                var states = inputDeserialiser.Deserialise(options);

                if (options.Unique || options.Fen)
                {
                    var fenSerialiser = new FenSerialiser();
                    Console.WriteLine();
                    Console.WriteLine("POPULATING FENS");
                    foreach(var state in states)
                    {
                        state.Fen = fenSerialiser.Serialise(state);
                    }
                    Console.WriteLine("Fens populated.");
                }

                if (options.Unique)
                {
                    Console.WriteLine();
                    Console.WriteLine("REMOVING DUPLICATES");
                    Console.WriteLine($"{states.Count():n0} states");
                    states = states.Distinct(new BoardStateEqualityComparer()).ToList();
                    Console.WriteLine($"{states.Count():n0} unique states");
                }

                if (options.Debug)
                {
                    DebugOutput.Debug(states);
                }

                if (!string.IsNullOrEmpty(options.Output))
                {
                    var outputSerialiser = new OutputSerialiser();
                    outputSerialiser.Serialise(states, options);
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
