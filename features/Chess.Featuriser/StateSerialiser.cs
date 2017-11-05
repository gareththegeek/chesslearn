using System;
using System.Collections.Generic;
using System.IO;
using Chess.Featuriser.Pgn;
using System.Text;

namespace Chess.Featuriser
{
    public class StateSerialiser
    {
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

            builder.Append("Fen").Append(", ");
            builder.Append("IsW").Append(", ");
            builder.Append("W0-0").Append(", ");
            builder.Append("W0-0-0").Append(", ");
            builder.Append("B0-0").Append(", ");
            builder.Append("B0-0-0").Append(", ");
            builder.Append("Move").Append(", ");
            builder.Append("HalfMove").Append(", ");
            builder.Append("EpFile").Append(", ");
            builder.Append("EpRank").Append(", ");

            string[] pieces;

            WritePieceCountHeadings("W", builder);
            WritePieceCountHeadings("B", builder);

            WritePieceListHeadings("W", builder);
            WritePieceListHeadings("B", builder);

            WriteSlidingHeadings("W", builder);
            WriteSlidingHeadings("B", builder);

            WriteAttackMapHeadings("W", builder);
            WriteAttackMapHeadings("B", builder);

            sw.WriteLine(builder.ToString());
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
                    builder.Append($"{colour}{piece}S{i}").Append(", ");
                }

                length = 4;
            }
        }

        private static void WritePieceListHeadings(string colour, StringBuilder builder)
        {
            var pieces = new[] { "Pa", "Pb", "Pc", "Pd", "Pe", "Pf", "Pg", "Ph", "QR", "QN", "QB", "Q", "K", "KB", "KN", "KR" };
            foreach (var piece in pieces)
            {
                builder.Append($"{colour}{piece}Present").Append(", ");
                builder.Append($"{colour}{piece}File").Append(", ");
                builder.Append($"{colour}{piece}Rank").Append(", ");
                builder.Append($"{colour}{piece}LVA").Append(", ");
                builder.Append($"{colour}{piece}LVD").Append(", ");
            }
        }

        private static void WritePieceCountHeadings(string colour, StringBuilder builder)
        {
            var pieces = new[] { "P", "N", "B", "R", "Q", "K" };

            foreach (var piece in pieces)
            {
                builder.Append($"{colour}{piece}Count").Append(", ");
            }
        }

        private void Serialise(BoardState state, StreamWriter sw)
        {
            var builder = new StringBuilder();

            var fenSerialiser = new FenSerialiser();
            var fen = fenSerialiser.Serialise(state);

            builder.Append(fen).Append(", ");

            builder
                .Append(FormatBoolean(state.IsWhite)).Append(", ")
                .Append(FormatBoolean(state.WhiteCastleShort)).Append(", ")
                .Append(FormatBoolean(state.WhiteCastleLong)).Append(", ")
                .Append(FormatBoolean(state.BlackCastleShort)).Append(", ")
                .Append(FormatBoolean(state.BlackCastleLong)).Append(", ")
                .Append(state.MoveNumber).Append(", ")
                .Append(state.HalfMoveClock).Append(", ")
                .Append(FormatSquare(state.EnPassantTarget)).Append(", ");

            foreach (var key in state.WhiteMaterialCount.Keys)
            {
                builder.Append(state.WhiteMaterialCount[key]).Append(", ");
            }

            foreach (var key in state.BlackMaterialCount.Keys)
            {
                builder.Append(state.BlackMaterialCount[key]).Append(", ");
            }
            // TODO There is a problem with the piece list, for example queen square's lowest value defender is 4(queen) but should be 5(king)
            // TODO Confirm order of piece export - csv ordering is confusing..
            foreach (var entry in state.WhitePieceList)
            {
                builder.Append(FormatEntry(entry)).Append(", ");
            }

            foreach (var entry in state.BlackPieceList)
            {
                builder.Append(FormatEntry(entry)).Append(", ");
            }

            foreach (var value in state.WhiteSlidingPieceMobility)
            {
                builder.Append(value).Append(", ");
            }

            foreach (var value in state.BlackSlidingPieceMobility)
            {
                builder.Append(value).Append(", ");
            }

            foreach (var pieceType in state.WhiteAttackMap)
            {
                builder.Append(FormatPieceType(pieceType)).Append(", ");
            }

            foreach (var pieceType in state.BlackAttackMap)
            {
                builder.Append(FormatPieceType(pieceType)).Append(", ");
            }

            builder.Remove(builder.Length - 2, 2);

            sw.WriteLine(builder.ToString());
        }

        private string FormatBoolean(bool value) => value ? "1" : "0";
        private string FormatSquare(Square square) => square == null ? "8, 8" : square.File + ", " + square.Rank;
        private string FormatPieceType(PieceType pieceType) => ((int)pieceType).ToString();
        private string FormatEntry(PieceListEntry entry) =>
            FormatBoolean(entry.IsPresent) + ", " +
            FormatSquare(entry.Square) + ", " +
            FormatPieceType(entry.LowestValueAttacker) + ", " +
            FormatPieceType(entry.LowestValueDefender);
    }
}
