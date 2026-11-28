using System;
using Assets.Scripts.Audio.Music;
using Assets.Scripts.Cursors;

namespace Assets.Scripts.Management
{
    [Serializable]
    public class SceneInfo
    {
        public int BuildIndex;
        public AudioBGM BGM;
        public CustomCursorID CursorID;
    }
}