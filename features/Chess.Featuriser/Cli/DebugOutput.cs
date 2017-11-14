using Chess.Featuriser.Features;
using Chess.Featuriser.Fen;
using Chess.Featuriser.State;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess.Featuriser.Cli
{
    public class DebugOutput
    {
        public static void Debug(IEnumerable<string> fens)
        {
            Console.WriteLine();
            Console.WriteLine("DEBUG MODE");
            Console.WriteLine("Enter fen number (1-" + fens.Count() + ")");
            var n = int.Parse(Console.ReadLine());

            var fenStateGenerator = new FenStateGenerator();
            var featureGenerator = new FeatureGenerator();

            Console.OutputEncoding = System.Text.Encoding.Unicode;
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine();

            var movestring = "";

            var i = n - 1;
            while(i < fens.Count())
            {
                var labelledFen = new LabelledFen(fens.ElementAt(i));
                var fen = labelledFen.Fen;
                var state = fenStateGenerator.Generate(fen);

                PrintFen(fen);

                featureGenerator.PopulateFeatures(state);
                PrintMap(state.WhiteAttackMap, true);
                PrintMap(state.BlackAttackMap, false);

                PrintPieceList(state.WhitePieceList, ConsoleColor.White);
                PrintPieceList(state.BlackPieceList, ConsoleColor.Black);

                PrintSlidingList(state.WhiteSlidingPieceMobility, ConsoleColor.White);
                PrintSlidingList(state.BlackSlidingPieceMobility, ConsoleColor.Black);
                
                Console.ReadLine();
                Console.Clear();
                if (state.IsWhite)
                {
                    movestring = state.MoveNumber + ".";
                }
                else
                {
                    movestring += " ..";
                }
                movestring += state.Move;
                Console.WriteLine(movestring);
                Console.WriteLine();
                i += 1;
            }
        }

        private class ColouredValue
        {
            public string Value { get; set; }
            public ConsoleColor Colour { get; set; }
        }

        private static void PrintFen(string fen)
        {
            var lines = fen.Split('/').Take(8);

            var extendedLines = new string[8];

            var i = 0;
            foreach (var line in lines)
            {
                foreach (var character in line)
                {
                    if (character < '1' || character > '8')
                    {
                        extendedLines[i] += character;
                    }
                    else
                    {
                        extendedLines[i] += "".PadLeft(int.Parse(character.ToString()), ' ');
                    }
                }
                i++;
            }

            PrintBoard((rank, file) => new ColouredValue
            {
                Value = extendedLines[rank][file].ToString(),
                Colour = ((extendedLines[rank][file] & 0x20) != 0) ? ConsoleColor.Black : ConsoleColor.White
            });
        }

        private static void PrintMap(PieceType[,] map, bool isWhite)
        {
            PrintBoard((rank, file) => new ColouredValue
            {
                Value = map[7 - rank, file] == PieceType.NoPiece ? " " : ((int)map[7 - rank, file]).ToString(),
                Colour = isWhite ? ConsoleColor.White : ConsoleColor.Black
            });
        }

        private static void PrintBoard(Func<int, int, ColouredValue> squareValue)
        {
            var black = false;
            for (var rank = 0; rank < 8; rank++)
            {
                black = !black;
                for (var file = 0; file < 8; file++)
                {
                    black = !black;

                    var colouredValue = squareValue(rank, file);

                    Console.BackgroundColor = black ? ConsoleColor.DarkGray : ConsoleColor.Gray;
                    Console.ForegroundColor = colouredValue.Colour;
                    Console.Write(colouredValue.Value);
                }
                Console.WriteLine();
            }
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
        }

        private static void PrintPieceList(PieceListEntry[] pieceList, ConsoleColor colour)
        {
            Console.ForegroundColor = colour;
            Console.BackgroundColor = ConsoleColor.Gray;

            for (var i = 0; i < (int)PieceListIndex.Count; i++)
            {
                var pieceName = ((PieceListIndex)i).ToPieceType().ToString();
                var pieceAbbr = pieceName.Substring(0, 1);
                if (pieceName.Contains("Knight"))
                {
                    pieceAbbr = "N";
                }
                Console.Write(pieceAbbr + "  ");
            }
            Console.WriteLine();

            for (var i = 0; i < (int)PieceListIndex.Count; i++)
            {
                if (!pieceList[i].IsPresent)
                {
                    Console.Write("   ");
                }
                else
                {
                    Console.Write(pieceList[i].Square + " ");
                }
            }
            Console.WriteLine();
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
        }

        private static void PrintSlidingList(List<int>[] list, ConsoleColor colour)
        {
            Console.ForegroundColor = colour;
            Console.BackgroundColor = ConsoleColor.Gray;

            for (var i = 0; i < (int)PieceListIndex.SlidingPieceCount; i++)
            {
                Console.Write(((PieceListIndex)i).ToPieceType().ToString().PadRight(7));

                foreach (var j in list[i])
                {
                    Console.Write(j.ToString());
                    Console.Write(" ");
                }
                Console.WriteLine();
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;

            Console.WriteLine();
        }
    }
}
