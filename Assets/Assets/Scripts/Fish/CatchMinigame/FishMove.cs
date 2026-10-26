using System;
using UnityEngine;

namespace Assets.Scripts.Fish.CatchMinigame
{
    public enum FishMove
    {
        Up,
        Down,
        Left,
        Right
    }

    public static class FishMoveExtensions
    {
        public static Vector2 ToVector2(this FishMove move) =>
            move switch
            {
                FishMove.Up => Vector2.up,
                FishMove.Down => Vector2.down,
                FishMove.Left => Vector2.left,
                FishMove.Right => Vector2.right,
                _ => throw new ArgumentOutOfRangeException(nameof(move), move, null)
            };
    }
}