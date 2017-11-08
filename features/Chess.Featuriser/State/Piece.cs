namespace Chess.Featuriser.State
{
    public class Piece
    {
        public Piece(PieceType pieceType, PieceListIndex pieceListIndex, bool isWhite)
        {
            PieceType = pieceType;
            PieceListIndex = pieceListIndex;
            IsWhite = isWhite;
        }

        protected Piece(Piece original)
        {
            PieceType = original.PieceType;
            PieceListIndex = original.PieceListIndex;
            IsWhite = original.IsWhite;
            Square = new Square(original.Square.Rank, original.Square.File);
        }

        public bool IsWhite { get; set; }
        public PieceType PieceType { get; set; }
        public Square Square { get; set; }
        public PieceListIndex PieceListIndex { get; set; }

        public Piece Clone()
        {
            return new Piece(this);
        }

        public override string ToString()
        {
            var pieceName = PieceType.GetAbbreviation();
            if (!IsWhite)
            {
                pieceName = pieceName.ToLower();
            }
            return pieceName + Square;
        }
    }
}
