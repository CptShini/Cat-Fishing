using System;
using UnityEngine;

namespace Assets.Scripts.Fishies.CatchMinigame
{
    public enum FishMove
    {
        Up,
        UpRight,
        Right,
        DownRight,
        Down,
        DownLeft,
        Left,
        UpLeft
    }

    public static class FishMoveExtensions
    {
        public static Vector2 ToVector2(this FishMove move) =>
            move switch
            {
                FishMove.Up => Vector2.up,
                FishMove.UpRight => Vector2.up + Vector2.right,
                FishMove.Right => Vector2.right,
                FishMove.DownRight => Vector2.down + Vector2.right,
                FishMove.Down => Vector2.down,
                FishMove.DownLeft => Vector2.down + Vector2.left,
                FishMove.Left => Vector2.left,
                FishMove.UpLeft => Vector2.up + Vector2.left,
                _ => throw new ArgumentOutOfRangeException(nameof(move), move, null)
            };
    }
}