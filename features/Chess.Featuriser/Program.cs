using System;
using Chess.Featuriser.Cli;

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
