using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Fishies.CatchMinigame
{
    [CreateAssetMenu(fileName = "Fish Catch Pattern", menuName = "ScriptableObjects/Fish/Fish Catch Pattern")]
    public class FishCatchPattern : ScriptableObject
    {
        public CatchPatternMove[] UpMoves;
        public CatchPatternMove[] UpRightMoves;
        public CatchPatternMove[] RightMoves;
        public CatchPatternMove[] DownRightMoves; 
        public CatchPatternMove[] DownMoves;
        public CatchPatternMove[] DownLeftMoves;
        public CatchPatternMove[] LeftMoves;
        public CatchPatternMove[] UpLeftMoves;
        
        public CatchPatternMove[] GetMoves(FishMove move)
        {
            return move switch
            {
                FishMove.Up => UpMoves,
                FishMove.UpRight => UpRightMoves,
                FishMove.Right => RightMoves,
                FishMove.DownRight => DownRightMoves,
                FishMove.Down => DownMoves,
                FishMove.DownLeft => DownLeftMoves,
                FishMove.Left => LeftMoves,
                FishMove.UpLeft => UpLeftMoves,
                _ => throw new ArgumentOutOfRangeException(nameof(move), move, null)
            };
        }

        public List<FishMove> GetUsedMoves()
        {
            List<FishMove> moves = new();
            if (UpMoves.Length > 0) moves.Add(FishMove.Up);
            if (UpRightMoves.Length > 0) moves.Add(FishMove.UpRight);
            if (RightMoves.Length > 0) moves.Add(FishMove.Right);
            if (DownRightMoves.Length > 0) moves.Add(FishMove.DownRight);
            if (DownMoves.Length > 0) moves.Add(FishMove.Down);
            if (DownLeftMoves.Length > 0) moves.Add(FishMove.DownLeft);
            if (LeftMoves.Length > 0) moves.Add(FishMove.Left);
            if (UpLeftMoves.Length > 0) moves.Add(FishMove.UpLeft);
            return moves;
        }
    }
}