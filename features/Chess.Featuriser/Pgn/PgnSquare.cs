namespace Chess.Featuriser.Pgn
{
    public struct PgnSquare
    {
        public int? Rank { get; set; }
        public int? File { get; set; }

        public override string ToString()
        {
            var result = string.Empty;
            if (File != null)
            {
                result += (char)(File + 'a');
            }
            if (Rank != null)
            {
                result += (Rank + 1).ToString();
            }
            return result;
        }
    }
}