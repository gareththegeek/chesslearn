﻿using Chess.Featuriser.Cli;
using Chess.Featuriser.Fen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess.Featuriser.Pgn
{
    public class PgnParser
    {
        private const int ReportEvery = 1000;

        private IEnumerator<PgnToken> enumerator;

        public IEnumerable<string> Parse(IEnumerable<PgnToken> tokens)
        {
            var stateGenerator = new PgnStateGenerator();
            var fenSerialiser = new FenSerialiser();

            var tokensList = tokens.ToList();

            enumerator = tokensList
                .Where(x => x.Type == PgnTokenType.Move || x.Type == PgnTokenType.Result)
                .GetEnumerator();

            var fens = new List<string>();

            var expectedTotal = tokensList.Count(x => x.Type == PgnTokenType.Result);
            var i = 1;
            var games = 0;
            var discarded = 0;
            var startTime = DateTime.Now;

            while (enumerator.MoveNext())
            {
                try
                {
                    games += 1;
                    var game = ParseGame();
                    var states = stateGenerator.GenerateStates(game);
                    fens.AddRange(states.Select(x => fenSerialiser.Serialise(x)));

                    if (i++ % ReportEvery == 0)
                    {
                        Console.WriteLine($"Processed {i:n0}/{expectedTotal:n0} ({i / (float)expectedTotal * 100.0:f2}%) games in {(DateTime.Now - startTime).TotalSeconds:f2}s");
                    }
                }
                catch (Exception)
                {
                    discarded += 1;
                    ConsoleHelper.PrintWarning($"Discarding game {i:n0}");
                }
            }

            Console.WriteLine($"Processed {games:n0} games, discarded {discarded:n0} ({discarded / (float)games * 100.0:f2}%) in {(DateTime.Now - startTime).TotalSeconds:f2}s");

            return fens;
        }

        private PgnGame ParseGame()
        {
            var game = new PgnGame();

            var isWhite = true;

            var i = 0;
            while (enumerator.Current.Type != PgnTokenType.Result)
            {
                try
                {
                    if (enumerator.Current.Text.Contains("#"))
                    {
                        enumerator.MoveNext();
                        continue;
                    }

                    if(enumerator.Current.Text == "{")
                    {
                        SkipRemaining();
                        break;
                    }

                    i++;
                    var move = ParseMove();
                    game.Moves.Add(move);

                    if (isWhite)
                    {
                        move.Flags |= (int)PgnMoveFlags.IsWhite;
                    }
                    isWhite = !isWhite;

                    enumerator.MoveNext();
                }
                catch (Exception)
                {
                    Console.WriteLine($"Error in half move {i}");
                }
            }

            return game;
        }

        private void SkipRemaining()
        {
            while (enumerator.Current.Type != PgnTokenType.Result)
            {
                ConsoleHelper.PrintWarning("Continuation detected ({), skipping remaining moves");
                enumerator.MoveNext();
            }
        }

        private PgnMove ParseMove()
        {
            var token = enumerator.Current;

            if (token.Type != PgnTokenType.Move) throw new InvalidOperationException();

            var text = token.Text
                .Replace("#", "")
                .Replace("+", "")
                .Replace("!", "")
                .Replace("?", "");

            if (text.StartsWith("O-O"))
            {
                return ParseCastle(text);
            }

            if (text.Contains("="))
            {
                return ParsePromotion(text);
            }

            return ParseStandardMove(text);
        }

        private PgnMove ParseCastle(string text)
        {
            return new PgnMove
            {
                Flags = (byte)(text == "O-O" ? PgnMoveFlags.CastleShort : PgnMoveFlags.CastleLong)
            };
        }

        private PgnMove ParsePromotion(string text)
        {
            var fragments = text.Split('=');
            var standardPart = fragments[0];
            var promotionPart = fragments[1];

            var move = ParseStandardMove(standardPart);

            move.Promotion = PieceLookup[promotionPart[0]];
            move.Flags |= (int)PgnMoveFlags.Promote;

            return move;
        }

        private static readonly Dictionary<char, PieceType> PieceLookup = new Dictionary<char, PieceType>
        {
            {'P', PieceType.Pawn},
            {'N', PieceType.Knight},
            {'B', PieceType.Bishop},
            {'R', PieceType.Rook},
            {'Q', PieceType.Queen},
            {'K', PieceType.King}
        };

        private PgnMove ParseStandardMove(string text)
        {
            var remaining = text;

            var piece = text.Substring(0, 1);
            if ((text[0] & 0x20) == 0x20)
            {
                piece = "P";
            }
            else
            {
                remaining = remaining.Substring(1);
            }

            var destination = remaining.Substring(remaining.Length - 2);
            remaining = remaining.Substring(0, remaining.Length - 2);

            var move = new PgnMove
            {
                Square = ParseSquare(destination),
                PieceType = PieceLookup[piece[0]]
            };

            if (remaining.EndsWith("x"))
            {
                move.Flags |= (int)PgnMoveFlags.Take;
                remaining = remaining.Substring(0, remaining.Length - 1);
            }

            if (remaining.Length > 1)
            {
                throw new InvalidOperationException($"Unrecognised move {text}");
            }

            if (!string.IsNullOrEmpty(remaining))
            {
                var reference = remaining[0];

                if (reference >= '0' && reference <= '9')
                {
                    move.Origin = new PgnSquare
                    {
                        Rank = ParseRank(reference)
                    };
                }
                else
                {
                    move.Origin = new PgnSquare
                    {
                        File = ParseFile(reference)
                    };
                }
            }

            return move;
        }

        private PgnSquare ParseSquare(string destination)
        {
            return new PgnSquare
            {
                Rank = ParseRank(destination[1]),
                File = ParseFile(destination[0])
            };
        }

        private static int ParseFile(char file)
        {
            return file - 'a';
        }

        private static int ParseRank(char rank)
        {
            return int.Parse(rank.ToString()) - 1;
        }
    }
}
