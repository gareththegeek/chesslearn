using System.IO;
using CommandLine;

namespace Chess.Featuriser.Cli
{
    public class Options : IConfigurationOptions
    {
        [Option('h', "help", HelpText = "Display this help")]
        public bool Help { get; set; }

        [Option('j', "config", HelpText = "Configuration filename - .json filename containing configuration settings. If specified these replace all options specified at the command line.")]
        public string ConfigurationFile { get; set; }

        [Option('i', "input", HelpText = "Input filename - Can be one of the following: .pgn file containing one or more chess games, .csv file containing one fen per row.")]
        public string Input { get; set; }

        [Option('s', "scores", HelpText = "Input scores - Indicated that the input csv contains position scores in the second column, which should be preserved in the output file as labels.")]
        public bool Scores { get; set; }

        [Option('o', "output", HelpText = "Output filename - Specifies the .csv filename to which data is output. If specified either /e, /f or both must also be true. Either output filename or debug mode is required.")]
        public string Output { get; set; }

        [Option('e', "features", HelpText = "Output features - Write features to the specified output .csv file.")]
        public bool Features { get; set; }

        [Option('f', "fen", HelpText = "Output fen - Write fen to the specified output .csv file.")]
        public bool Fen { get; set; }

        //TODO support uci output

        [Option('u', "unique", HelpText = "Unique output - output only unique board positions.")]
        public bool Unique { get; set; }

        [Option('d', "debug", HelpText = "Debug mode - Run debug mode for specific game. Either output filename or debug mode is required.")]
        public bool Debug { get; set; }

        public bool Validate()
        {
            if (string.IsNullOrEmpty(Input) || !File.Exists(Input))
            {
                ConsoleHelper.PrintError("Must specify an input file for the Featuriser to process");
                return false;
            }

            if (!string.IsNullOrEmpty(ConfigurationFile) && !File.Exists(ConfigurationFile))
            {
                ConsoleHelper.PrintWarning($"Configuration file not found '{ConfigurationFile}'. Using command line parameters");
                ConfigurationFile = null;
            }

            if (Path.GetExtension(Input).ToLower() == ".pgn" && Scores)
            {
                ConsoleHelper.PrintError("Cannot import scores from pgn file. Either specify csv input file or do not set scores parameter");
                return false;
            }

            if (!Debug && string.IsNullOrEmpty(Output))
            {
                ConsoleHelper.PrintError("Must specify either Debug or Output mode");
                return false;
            }

            if (!string.IsNullOrEmpty(Output) && !Features && !Fen)
            {
                ConsoleHelper.PrintError("One or more output formats must be specified (fen/features) when using Output mode");
                return false;
            }

            return true;
        }
    }
}
