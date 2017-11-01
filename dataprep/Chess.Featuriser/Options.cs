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

        public bool Validate()
        {
            if (string.IsNullOrEmpty(PgnFile) || !File.Exists(PgnFile))
            {
                ConsoleHelper.PrintError("Must specify a PGN file for the Featuriser to process");
                return false;
            }

            return true;
        }
    }
}
