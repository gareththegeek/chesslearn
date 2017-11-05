using System.Collections.Generic;
using System.IO;
using Chess.Featuriser.Pgn;
using System.Text;

namespace Chess.Featuriser
{
    public class StateSerialiser
    {
        private const string Delimeter = ",";

        public void Serialise(IEnumerable<PgnGame> games, Stream stream)
        {
            var stateGenerator = new PgnStateGenerator();
            var featureGenerator = new FeatureGenerator();

            using (var sw = new StreamWriter(stream))
            {
                WriteHeadings(sw);

                foreach (var game in games)
                {
                    foreach (var state in stateGenerator.GenerateStates(game))
                    {
                        featureGenerator.PopulateFeatures(state);
                        Serialise(state, sw);
                    }
                }
            }
        }

        private void WriteHeadings(StreamWriter sw)
        {
            var builder = new StringBuilder();

            builder.Append("Move").Append(Delimeter);
            builder.Append("Fen").Append(Delimeter);
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

        private void Serialise(BoardState state, StreamWriter sw)
        {
            var builder = new StringBuilder();

            builder.Append(state.Move?.ToString()).Append(Delimeter);

            var fenSerialiser = new FenSerialiser();
            var fen = fenSerialiser.Serialise(state);

            builder.Append(fen).Append(Delimeter);

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

            builder.Remove(builder.Length-1, 1);

            sw.WriteLine(builder.ToString());
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
