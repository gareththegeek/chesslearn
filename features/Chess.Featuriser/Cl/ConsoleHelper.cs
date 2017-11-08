using System;
using CommandLine.Text;

namespace Chess.Featuriser.Cl
{
    public static class ConsoleHelper
    {
        public static void PrintColoured(string error, ConsoleColor colour)
        {
            var currentColour = Console.ForegroundColor;
            Console.ForegroundColor = colour;
            Console.WriteLine(error);
            Console.ForegroundColor = currentColour;
        }

        public static void PrintError(string error)
        {
            PrintColoured(error, ConsoleColor.Red);
        }

        public static void PrintWarning(string warning)
        {
            PrintColoured(warning, ConsoleColor.Yellow);   
        }

        public static void PrintHelp<T>() where T: new()
        {
            Console.WriteLine();
            Console.WriteLine(HelpText.AutoBuild(new T()));
            Console.ReadLine();
        }
    }
}
