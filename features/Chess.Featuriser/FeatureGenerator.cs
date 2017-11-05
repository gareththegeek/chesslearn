using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess.Featuriser
{
    public class FeatureGenerator
    {
        public void PopulateFeatures(BoardState state)
        {
            InstantiateProperties(state);
            PopulateMap(state, state.WhiteAttackMap, true);
            PopulateMap(state, state.BlackAttackMap, false);
            PopulateMaterialCount(state, state.WhiteMaterialCount, true);
            PopulateMaterialCount(state, state.BlackMaterialCount, false);
            PopulatePieceList(state, state.WhitePieceList, state.BlackAttackMap, state.WhiteAttackMap, true);
            PopulatePieceList(state, state.BlackPieceList, state.WhiteAttackMap, state.BlackAttackMap, false);
            PopulateSlidingPieceMobility(state, state.WhiteSlidingPieceMobility, state.WhitePieceList);
            PopulateSlidingPieceMobility(state, state.BlackSlidingPieceMobility, state.BlackPieceList);
        }

        private void InstantiateProperties(BoardState state)
        {
            state.WhiteMaterialCount = new Dictionary<PieceType, int>();
            state.BlackMaterialCount = new Dictionary<PieceType, int>();
            state.WhitePieceList = new PieceListEntry[(int)PieceListIndex.Count];
            state.BlackPieceList = new PieceListEntry[(int)PieceListIndex.Count];
            state.WhiteSlidingPieceMobility = new List<int>[(int)PieceListIndex.SlidingPieceCount];
            state.BlackSlidingPieceMobility = new List<int>[(int)PieceListIndex.SlidingPieceCount];
            state.WhiteAttackMap = new PieceType[8, 8];
            state.BlackAttackMap = new PieceType[8, 8];
        }

        private void PopulateMaterialCount(BoardState state, Dictionary<PieceType, int> materialCount, bool isWhite)
        {
            foreach (PieceType pieceType in Enum.GetValues(typeof(PieceType))
                .Cast<PieceType>()
                .Where(x => x != PieceType.Count && x != PieceType.NoPiece))
            {
                materialCount[pieceType] = state
                    .Flatten()
                    .Count(x => x.IsWhite == isWhite && x.PieceType == pieceType);
            }
        }

        private void PopulatePieceList(
            BoardState state,
            PieceListEntry[] pieceList,
            PieceType[,] attackMap,
            PieceType[,] defendMap,
            bool isWhite)
        {
            for (var i = 0; i < (int)PieceListIndex.Count; i++)
            {
                var piece = state.Flatten().SingleOrDefault(x => x.IsWhite == isWhite && (int)x.PieceListIndex == i);

                if (piece == null)
                {
                    pieceList[i] = new PieceListEntry
                    {
                        IsPresent = false
                    };
                }
                else
                {
                    pieceList[i] = new PieceListEntry
                    {
                        IsPresent = true,
                        Square = piece.Square,
                        LowestValueAttacker = attackMap[piece.Square.Rank, piece.Square.File],
                        LowestValueDefender = defendMap[piece.Square.Rank, piece.Square.File]
                    };
                }
            }
        }

        private void PopulateSlidingPieceMobility(BoardState state, List<int>[] slidingPieceMobility, PieceListEntry[] pieceList)
        {
            for (var i = 0; i < (int)PieceListIndex.SlidingPieceCount; i++)
            {
                slidingPieceMobility[i] = new List<int>();

                var entry = pieceList[i];

                if (IsDiagonalSlider((PieceListIndex)i))
                {
                    if (entry.IsPresent)
                    {
                        slidingPieceMobility[i].Add(GetSlidingMobility(state, entry.Square, -1, -1));
                        slidingPieceMobility[i].Add(GetSlidingMobility(state, entry.Square, 1, -1));
                        slidingPieceMobility[i].Add(GetSlidingMobility(state, entry.Square, 1, 1));
                        slidingPieceMobility[i].Add(GetSlidingMobility(state, entry.Square, -1, 1));
                    }
                    else
                    {
                        slidingPieceMobility[i].AddRange(new[] { -1, -1, -1, -1 });
                    }
                }
                if (IsOrthogonalSlider((PieceListIndex)i))
                {
                    if (entry.IsPresent)
                    {
                        slidingPieceMobility[i].Add(GetSlidingMobility(state, entry.Square, 1, 0));
                        slidingPieceMobility[i].Add(GetSlidingMobility(state, entry.Square, 0, 1));
                        slidingPieceMobility[i].Add(GetSlidingMobility(state, entry.Square, 0, -1));
                        slidingPieceMobility[i].Add(GetSlidingMobility(state, entry.Square, -1, 0));
                    }
                    else
                    {
                        slidingPieceMobility[i].AddRange(new[] { -1, -1, -1, -1 });
                    }
                }
            }
        }

        private int GetSlidingMobility(BoardState state, Square origin, int rankDirection, int fileDirection)
        {
            var rank = origin.Rank + rankDirection;
            var file = origin.File + fileDirection;
            var mobility = 0;

            while (rank >= 0 && file >= 0 && rank < 8 && file < 8 && state.Squares[rank, file] == null)
            {
                mobility += 1;
                rank += rankDirection;
                file += fileDirection;
            }

            return mobility;
        }

        private bool IsDiagonalSlider(PieceListIndex index)
        {
            return index == PieceListIndex.Queen
                   || index == PieceListIndex.Bishop1
                   || index == PieceListIndex.Bishop2;
        }

        private bool IsOrthogonalSlider(PieceListIndex index)
        {
            return index == PieceListIndex.Queen
                   || index == PieceListIndex.Rook1
                   || index == PieceListIndex.Rook2;
        }

        private void PopulateMap(BoardState state, PieceType[,] map, bool isWhite)
        {
            for (var rank = 0; rank < 8; rank++)
            {
                for (var file = 0; file < 8; file++)
                {
                    var considerCheck = true;
                    var target = state.Squares[rank, file];
                    if (target?.PieceType == PieceType.King && target.IsWhite == isWhite)
                    {
                        considerCheck = false;
                    }

                    map[rank, file] = state
                        .Flatten()
                        .Where(x => x.IsWhite == isWhite)
                        .Where(x => state.CanTake(x, new Square(rank, file), considerCheck))
                        .Select(x => x.PieceType)
                        .DefaultIfEmpty(PieceType.NoPiece)
                        .Min();
                }
            }
        }
    }
}
