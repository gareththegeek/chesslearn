namespace Chess.Featuriser.Pgn
{
    public enum PgnMoveFlags
    {
        Take = 1,
        IsWhite = 2,
        CastleLong = 4,
        CastleShort = 8,
        Castle = CastleShort | CastleLong,
        Promote = 16
    }
}