using UnityEngine;

namespace Assets.Scripts.Audio
{
    [CreateAssetMenu(fileName = "New Audio SFX", menuName = "ScriptableObjects/Audio SFX")]
    public class AudioSFX : ScriptableObject
    {
        [field: SerializeField]
        public AudioId Id { get; private set; }
        
        [field: SerializeField, Space]
        public AudioClip Clip { get; private set; }
        
        [field: SerializeField, Range(0f, 1f)]
        public float Volume { get; private set; }
        
        [field: SerializeField]
        public bool Looping { get; private set; }

        public override string ToString() => 
            $"{Id} ({(Looping ? "Loop" : "SFX")})";
    }
}