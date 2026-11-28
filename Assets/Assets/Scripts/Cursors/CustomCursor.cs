using System;
using UnityEngine;

namespace Assets.Scripts.Cursors
{
    [Serializable]
    public class CustomCursor
    {
        public CustomCursorID id;
        public Texture2D texture;
        public Vector2 hotspot;
    }
}