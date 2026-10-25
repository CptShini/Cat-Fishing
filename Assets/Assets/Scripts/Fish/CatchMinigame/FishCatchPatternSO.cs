using System;
using UnityEngine;

namespace Assets.Scripts.Fish.CatchMinigame
{
    [CreateAssetMenu(fileName = "Fish Catch Pattern", menuName = "ScriptableObjects/Fish Catch Pattern")]
    public class FishCatchPatternSO : ScriptableObject
    {
        public CatchPatternMove[] UpMoves;
        public CatchPatternMove[] DownMoves;
        public CatchPatternMove[] LeftMoves;
        public CatchPatternMove[] RightMoves;

        public CatchPatternMove[] GetMoves(FishMove move)
        {
            return move switch
            {
                FishMove.Up => UpMoves,
                FishMove.Down => DownMoves,
                FishMove.Left => LeftMoves,
                FishMove.Right => RightMoves,
                _ => throw new ArgumentOutOfRangeException(nameof(move), move, null)
            };
        }
    }
}