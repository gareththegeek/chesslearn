using System;
using System.IO;
using CommandLine;
using Newtonsoft.Json;

namespace Chess.Featuriser.Cli
{
    public class Configurator<T> where T : class, IConfigurationOptions, new()
    {
        public T BuildOptions(string[] args)
        {
            var options = new T();

            if (!ParseArgs(options, args))
            {
                ConsoleHelper.PrintHelp<T>();
                return null;
            }

            if (options == null)
            {
                throw new InvalidOperationException("Unable to create options");
            }

            if (options.Help)
            {
                ConsoleHelper.PrintHelp<T>();
                return null;
            }

            if (!string.IsNullOrEmpty(options.ConfigurationFile))
            {
                options = ParseConfigurationFile(options);
            }

            if (options == null || !options.Validate())
            {
                ConsoleHelper.PrintHelp<T>();
                return null;
            }

            return options;
        }

        private bool ParseArgs(T options, string[] args)
        {
            return Parser.Default.ParseArguments(args, options);
        }

        private T ParseConfigurationFile(T options)
        {
            var configurationFile = options.ConfigurationFile;

            if (!File.Exists(configurationFile))
            {
                ConsoleHelper.PrintError($"Configuration file not found {configurationFile}");
                return null;
            }

            try
            {
                using (var fs0 = new FileStream(configurationFile, FileMode.Open, FileAccess.Read))
                {
                    using (var sr0 = new StreamReader(fs0))
                    {
                        var json = sr0.ReadToEnd();

                        options = JsonConvert.DeserializeObject<T>(json);
                    }
                }
            }
            catch (Exception)
            {
                ConsoleHelper.PrintError("Unable to parse configuration file");
                return null;
            }

            if (options == null)
            {
                ConsoleHelper.PrintError("Unable to parse configuration file");
                return null;
            }

            return options;
        }
    }
}
