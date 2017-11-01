using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Chess.Featuriser.Pgn
{
    public class PgnScanner
    {
        private ICollection<PgnToken> tokens;

        public IEnumerable<PgnToken> Scan(Stream stream)
        {
            tokens = new List<PgnToken>();

            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine()?.Trim();

                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    ScanLine(line);

                }
            }

            return tokens;
        }

        private void Emit(PgnToken token)
        {
            tokens.Add(token);
        }

        private void ScanLine(string line)
        {
            if (line.StartsWith("["))
            {
                ScanInfoLine(line);
            }
            else
            {
                ScanMoveTextLine(line);
            }
        }

        private void ScanInfoLine(string line)
        {
            Emit(new PgnToken
            {
                Text = line,
                Type = PgnTokenType.Info
            });
        }

        private void ScanMoveTextLine(string line)
        {
            var fragments = line.Split(' ');
            
            foreach (var fragment in fragments)
            {
                ScanMoveText(fragment);
            }
        }

        private static readonly Regex ResultRegex = new Regex(@"[0-1]{1}\-[0-1]{1}");
        private void ScanMoveText(string fragment)
        {
            if (fragment.Contains("."))
            {
                ScanMoveNumber(fragment);
            }
            else if (ResultRegex.IsMatch(fragment) || fragment == "1/2-1/2" || fragment == "*")
            {
                ScanResult(fragment);
            }
            else
            {
                ScanMove(fragment);
            }
        }

        private void ScanMoveNumber(string fragment)
        {
            var parts = fragment
                .Split('.')
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();

            Emit(new PgnToken
            {
                Text = parts.First(),
                Type = PgnTokenType.TurnNumber
            });

            if (parts.Count> 1)
            {
                foreach (var part in parts.Skip(1))
                {
                    ScanMoveText(part);
                }
            }
        }

        private void ScanMove(string fragment)
        {
            Emit(new PgnToken
            {
                Text = fragment,
                Type = PgnTokenType.Move
            });
        }

        private void ScanResult(string fragment)
        {
            Emit(new PgnToken
            {
                Text = fragment,
                Type = PgnTokenType.Result
            });
        }
    }
}