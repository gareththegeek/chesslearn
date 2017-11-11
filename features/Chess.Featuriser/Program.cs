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
                var fens = inputDeserialiser.Deserialise(options);
                inputDeserialiser = null;
                GC.Collect();

                if (!string.IsNullOrEmpty(options.Output))
                {
                    var outputSerialiser = new OutputSerialiser();
                    outputSerialiser.Serialise(fens, options);
                }

                if (options.Debug)
                {
                    DebugOutput.Debug(fens);
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
