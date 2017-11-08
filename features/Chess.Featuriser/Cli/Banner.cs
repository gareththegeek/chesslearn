using System;
using System.Reflection;
using System.Threading;

namespace Chess.Featuriser.Cli
{
    public static class Banner
    {
        public static void Print(string productName)
        {
            //const int space = 23;
            var thisApp = Assembly.GetExecutingAssembly();
            var name = new AssemblyName(thisApp.FullName);
            var version = "v" + name.Version;
            //var versionWithPadding = PadToLength(version, space);
            //var productNameWithPadding = PadToLength(productName, space);

            var start = Console.CursorTop;

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine(@"                                                                               ");
            Console.WriteLine(@"                .8DNNNNNNNNNNNNNDNDNNDZ=.                                      ");
            Console.WriteLine(@"                 .MDDDDDDDDDDDDDDDDDDDDDDDDM,                                  ");
            Console.WriteLine(@"                   NDDDDDDDDDDDDDDDDDDDDDDDDDDD:                               ");
            Console.WriteLine(@"                   DDDDDDDDDDDDDDDDDDDDDDDDDDDDDN.                             ");
            Console.WriteLine(@"                   NDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD,                            ");
            Console.WriteLine(@"                   NDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDI                           ");
            Console.WriteLine(@"                   DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD$                          ");
            Console.WriteLine(@"                   DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD                          ");
            Console.WriteLine(@"                  .DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDM                         ");
            Console.WriteLine(@"                  ?DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD.                        ");
            Console.WriteLine(@"                  8DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD,                        ");
            Console.WriteLine(@"                  MDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD+                        ");
            Console.WriteLine(@"                  NDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD7                        ");
            Console.WriteLine(@"                  DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDI                        ");
            Console.WriteLine(@"                  DDDDDDDDDDDDDDDDZDDDDDDDDDDDDDDDDDDD~                        ");
            Console.WriteLine(@"                 .DDDDDDDDDDDDDDD+ DDDDDDDDDDDDDDDDDDD                         ");
            Console.WriteLine(@"                 IDDDDDDDDDDDDDDN  DDDDDDDDDDDDDDDDDDD                         ");
            Console.WriteLine(@"                 DDDDDDDDDDDDDDM   DDDDDDDDDDDDDDDDDDM                         ");
            Console.WriteLine(@"                 MDDDDDDDDDDDD8    NDDDDDDDDDDDDDDDDD:                         ");
            Console.WriteLine(@"                  NDDDDDDDDDDM    .DDDDDDDDDDDDDDDDDN                          ");
            Console.WriteLine(@"                     7DDDDDDM.    MDDDDDDDDDDDDDDDDD8                          ");
            Console.WriteLine(@"                       .MDDN     .DDDDDDDDDDDDDDDDDD+                          ");
            Console.WriteLine(@"                                 NDDDDDDDDDDDDDDDDDD.                          ");
            Console.WriteLine(@"                                8DDDDDDDDDDDDDDDDDDD                           ");
            Console.WriteLine(@"                               ODDDDDDDDDDDDDDDDDDDD.                          ");
            Console.WriteLine(@"                             .DDDDDDDDDDDDDDDDDDDDDD7                          ");
            Console.WriteLine(@"                             DDDDDDDDDDDDDDDDDDDDDDDD                          ");
            Console.WriteLine(@"                          .NDDDDDDDDDDDDDDDDDDDDDDDDD+                         ");
            Console.WriteLine(@"                        .MDDDDDDDDDDDDDDDDDDDDDDDDDDDD                         ");
            Console.WriteLine(@"                        7DDDDDDDDDDDDDDDDDDDDDDDDDDDDD7                        ");
            Console.WriteLine(@"                        .                                                      ");
            Console.WriteLine(@"                                                                               ");
            Console.WriteLine(@"                    NDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDN                    ");
            Console.WriteLine(@"                   7DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD$                   ");
            Console.WriteLine(@"                   7DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD$                   ");
            Console.WriteLine(@"                   7DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD$                   ");
            Console.WriteLine(@"                   7DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD$                   ");
            Console.WriteLine(@"                    NDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD.                   ");
            Console.WriteLine(@"                                                                               ");

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine($"{productName} {version}");

            //for (int i = 0; i < 30; i++)
            //{
            //    Console.SetCursorPosition(0, start + i);
            //    Console.Write(" ");
            //}
            //Console.SetCursorPosition(0, Console.CursorTop + 2);

            Thread.Sleep(1000);
        }

        private static string PadToLength(string version, int space)
        {
            var versionWithPadding = version;
            for (var i = 0; i < space - version.Length; i++)
            {
                versionWithPadding += " ";
            }
            return versionWithPadding;
        }
    }
}
