namespace Chess.Featuriser
{
    public class PieceListEntry
    {
        public bool IsPresent { get; set; }
        public Square Square { get; set; }
        public PieceType LowestValueAttacker { get; set; }
        public PieceType LowestValueDefender { get; set; }
    }
}