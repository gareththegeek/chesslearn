using System.IO;
using CommandLine;

namespace Chess.Featuriser
{
    public class Options: IConfigurationOptions
    {
        [Option('h', "help", DefaultValue = false, HelpText = "Display this help")]
        public bool Help { get; set; }

        [Option('j', "config", HelpText = "Json filename containing configuration settings, if specified these replace all options specified at the command line")]
        public string ConfigurationFile { get; set; }

        [Option('f', "pgnfile", HelpText = "PGN filename containing one or more chess games to convert to feature representation")]
        public string PgnFile { get; set; }

        [Option('o', "output", HelpText = "Write features to csv file specified by output path")]
        public bool Output { get; set; }

        [Option('p', "outputfile", HelpText = "Output csv file to write feature to if output mode is specified")]
        public string OutputFile { get; set; }

        [Option('d', "debug", HelpText = "Run debug mode for specific game")]
        public bool Debug { get; set; }

        public bool Validate()
        {
            if (string.IsNullOrEmpty(PgnFile) || !File.Exists(PgnFile))
            {
                ConsoleHelper.PrintError("Must specify a PGN file for the Featuriser to process");
                return false;
            }

            if(!Debug && !Output)
            {
                ConsoleHelper.PrintError("Must specify either Debug or Output mode");
                return false;
            }

            if(Output && string.IsNullOrEmpty(OutputFile))
            {
                ConsoleHelper.PrintError("Must specify output path for output mode");
                return false;
            }

            return true;
        }
    }
}
