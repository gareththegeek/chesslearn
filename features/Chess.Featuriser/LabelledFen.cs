namespace Chess.Featuriser
{
    public struct LabelledFen
    {
        public LabelledFen(string fen)
        {
            Fen = fen;
            Score = null;

            if (fen.IndexOf(",") == -1)
            {
                return;
            }

            var tokens = fen.Split(',');
            Fen = tokens[0];
            Score = float.Parse(tokens[1]);
        }

        public string Fen { get; set; }
        public float? Score { get; set; }
    }
}
