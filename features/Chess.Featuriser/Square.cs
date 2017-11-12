namespace Chess.Featuriser
{
    public class Square
    {
        public Square(int rank, int file)
        {
            Rank = rank;
            File = file;
        }

        public int Rank { get; set; }
        public int File { get; set; }

        public override string ToString()
        {
            return (char)(File + 'a') + (Rank + 1).ToString();
        }

        public override bool Equals(object obj)
        {
            var other = obj as Square;

            if (other == null)
            {
                return base.Equals(obj);
            }

            return Rank == other.Rank && File == other.File;
        }
    }
}
