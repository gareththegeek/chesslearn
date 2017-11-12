using System.Collections.Generic;
using System.IO;
using System.Text;
using Chess.Featuriser.Features;
using Chess.Featuriser.Fen;
using System;
using Chess.Featuriser.State;
using Chess.Featuriser.Cli;
using System.Linq;

namespace Chess.Featuriser
{
    public class OutputSerialiser
    {
        private const string Delimeter = ",";
        private const int ReportEvery = 50000;

        public void Serialise(IEnumerable<string> fens, Options options)
        {
            if (!options.Features && !options.Fen) throw new ArgumentException("Must specify features or fen to seriliase");

            var startTime = DateTime.Now;
            Console.WriteLine();
            Console.WriteLine("SERIALISING OUTPUT");

            var fenStateGenerator = new FenStateGenerator();
            var featureGenerator = new FeatureGenerator();

            var i = 0;

            using (var stream = new FileStream(options.Output, FileMode.Create, FileAccess.Write))
            {
                using (var sw = new StreamWriter(stream))
                {
                    WriteHeadings(sw, options);

                    var total = fens.Count();
                    foreach (var fen in fens)
                    {
                        var fenValue = fen;
                        float? score = null;
                        if (options.Scores)
                        {
                            var tokens = fen.Split(',');
                            fenValue = tokens[0];
                            score = float.Parse(tokens[1]);
                        }

                        var state = fenStateGenerator.Generate(fenValue);
                        state.Score = score;

                        if (options.Features)
                        {
                            featureGenerator.PopulateFeatures(state);
                        }
                        Serialise(state, sw, options);

                        if (i++ % ReportEvery == 0)
                        {
                            Console.WriteLine($"Serialised {i:n0} states ({i / (float)total * 100.0:f2}%) in {(DateTime.Now - startTime).TotalSeconds:f2}s");
                            sw.Flush();
                        }
                    }
                }
            }

            Console.WriteLine($"Wrote output file: {options.Output} - {i:n0} states in {(DateTime.Now - startTime).TotalSeconds:f2}s");
        }

        private void WriteHeadings(StreamWriter sw, Options options)
        {
            var builder = new StringBuilder();

            //TODO support uci output
            //builder.Append("Move").Append(Delimeter);
            if (options.Fen)
            {
                builder.Append("Fen").Append(Delimeter);
            }

            if (options.Features)
            {
                builder.Append("IsW").Append(Delimeter);
                builder.Append("W0-0").Append(Delimeter);
                builder.Append("W0-0-0").Append(Delimeter);
                builder.Append("B0-0").Append(Delimeter);
                builder.Append("B0-0-0").Append(Delimeter);
                builder.Append("Move").Append(Delimeter);
                builder.Append("HalfMove").Append(Delimeter);
                builder.Append("EpFile").Append(Delimeter);
                builder.Append("EpRank").Append(Delimeter);

                WritePieceCountHeadings("W", builder);
                WritePieceCountHeadings("B", builder);

                WritePieceListHeadings("W", builder);
                WritePieceListHeadings("B", builder);

                WriteSlidingHeadings("W", builder);
                WriteSlidingHeadings("B", builder);

                WriteAttackMapHeadings("W", builder);
                WriteAttackMapHeadings("B", builder);
            }

            if (options.Scores)
            {
                builder.Append("SCORE").Append(Delimeter);
            }

            builder.Remove(builder.Length - 1, 1);

            sw.WriteLine(builder.ToString());
        }

        private void WriteAttackMapHeadings(string colour, StringBuilder builder)
        {
            var ranks = new[] { "1", "2", "3", "4", "5", "6", "7", "8" };
            var files = new[] { "a", "b", "c", "d", "e", "f", "g", "h" };

            foreach (var rank in ranks)
            {
                foreach (var file in files)
                {
                    builder.Append($"{colour}A{file}{rank}").Append(Delimeter);
                }
            }
        }

        private static void WriteSlidingHeadings(string colour, StringBuilder builder)
        {
            string[] pieces;
            var length = 8;
            pieces = new[] { "Q", "KR", "QR", "KB", "QB" };
            foreach (var piece in pieces)
            {
                for (var i = 0; i < length; i++)
                {
                    builder.Append($"{colour}{piece}S{i}").Append(Delimeter);
                }

                length = 4;
            }
        }

        private static void WritePieceListHeadings(string colour, StringBuilder builder)
        {
            var pieces = new[] { "Q", "QR", "KR", "QB", "KB", "K", "QN", "KN", "Pa", "Pb", "Pc", "Pd", "Pe", "Pf", "Pg", "Ph" };
            foreach (var piece in pieces)
            {
                builder.Append($"{colour}{piece}Present").Append(Delimeter);
                builder.Append($"{colour}{piece}File").Append(Delimeter);
                builder.Append($"{colour}{piece}Rank").Append(Delimeter);
                builder.Append($"{colour}{piece}LVA").Append(Delimeter);
                builder.Append($"{colour}{piece}LVD").Append(Delimeter);
            }
        }

        private static void WritePieceCountHeadings(string colour, StringBuilder builder)
        {
            var pieces = new[] { "P", "N", "B", "R", "Q", "K" };

            foreach (var piece in pieces)
            {
                builder.Append($"{colour}{piece}Count").Append(Delimeter);
            }
        }

        private void Serialise(BoardState state, StreamWriter sw, Options options)
        {
            var builder = new StringBuilder();

            //TODO support UCI output
            //builder.Append(state.Move?.ToString()).Append(Delimeter);

            if (options.Fen)
            {
                builder.Append(state.Fen).Append(Delimeter);
            }

            if (options.Features)
            {
                AppendFeatures(state, builder);
            }

            if (options.Scores)
            {
                throw new NotImplementedException("How can we access the score from here?");
            }

            builder.Remove(builder.Length - 1, 1);

            sw.WriteLine(builder.ToString());
        }

        private void AppendFeatures(BoardState state, StringBuilder builder)
        {
            builder
                .Append(FormatBoolean(state.IsWhite)).Append(Delimeter)
                .Append(FormatBoolean(state.WhiteCastleShort)).Append(Delimeter)
                .Append(FormatBoolean(state.WhiteCastleLong)).Append(Delimeter)
                .Append(FormatBoolean(state.BlackCastleShort)).Append(Delimeter)
                .Append(FormatBoolean(state.BlackCastleLong)).Append(Delimeter)
                .Append(state.MoveNumber).Append(Delimeter)
                .Append(state.HalfMoveClock).Append(Delimeter)
                .Append(FormatSquare(state.EnPassantTarget)).Append(Delimeter);

            foreach (var key in state.WhiteMaterialCount.Keys)
            {
                builder.Append(state.WhiteMaterialCount[key]).Append(Delimeter);
            }

            foreach (var key in state.BlackMaterialCount.Keys)
            {
                builder.Append(state.BlackMaterialCount[key]).Append(Delimeter);
            }
            // TODO There is a problem with the piece list, for example queen square's lowest value defender is 4(queen) but should be 5(king)
            foreach (var entry in state.WhitePieceList)
            {
                builder.Append(FormatEntry(entry)).Append(Delimeter);
            }

            foreach (var entry in state.BlackPieceList)
            {
                builder.Append(FormatEntry(entry)).Append(Delimeter);
            }

            foreach (var array in state.WhiteSlidingPieceMobility)
            {
                foreach (var value in array)
                {
                    builder.Append(value).Append(Delimeter);
                }
            }

            foreach (var array in state.BlackSlidingPieceMobility)
            {
                foreach (var value in array)
                {
                    builder.Append(value).Append(Delimeter);
                }
            }

            foreach (var pieceType in state.WhiteAttackMap)
            {
                builder.Append(FormatPieceType(pieceType)).Append(Delimeter);
            }

            foreach (var pieceType in state.BlackAttackMap)
            {
                builder.Append(FormatPieceType(pieceType)).Append(Delimeter);
            }
        }

        private string FormatBoolean(bool value) => value ? "1" : "0";
        private string FormatSquare(Square square) => square == null ? "8, 8" : square.File + Delimeter + square.Rank;
        private string FormatPieceType(PieceType pieceType) => ((int)pieceType).ToString();
        private string FormatEntry(PieceListEntry entry) =>
            FormatBoolean(entry.IsPresent) + Delimeter +
            FormatSquare(entry.Square) + Delimeter +
            FormatPieceType(entry.LowestValueAttacker) + Delimeter +
            FormatPieceType(entry.LowestValueDefender);
    }
}
