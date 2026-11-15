using UnityEngine;

namespace Assets.Scripts.Audio
{
    [CreateAssetMenu(fileName = "New Audio BGM", menuName = "ScriptableObjects/Audio BGM")]
    public class AudioBGM : ScriptableObject
    {
        [field: SerializeField]
        public AudioClip Track { get; private set; }
        
        [field: SerializeField, Range(0f, 1f)]
        public float Volume { get; private set; }
    }
}