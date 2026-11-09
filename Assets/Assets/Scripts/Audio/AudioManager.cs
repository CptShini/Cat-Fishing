using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }
        
        [SerializeField] private GameObject AudioSFXInstancePrefab;
        [SerializeField] private AudioSFX[] audioSFXs;
        
        private readonly Dictionary<AudioId, AudioSFXInstance> _loopingAudio = new();
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }
        
        public AudioSFXInstance Play(AudioId id)
        {
            AudioSFX audioSFX = audioSFXs.FirstOrDefault(sfx => sfx.Id == id);
            if (!audioSFX) return null;
            
            GameObject audioSFXInstanceObject = Instantiate(AudioSFXInstancePrefab);
            AudioSFXInstance audioSFXInstance = audioSFXInstanceObject.GetComponent<AudioSFXInstance>();
            audioSFXInstance.Play(audioSFX);

            return audioSFXInstance;
        }
        
        public void PlayLoop(AudioId id)
        {
            if (_loopingAudio.ContainsKey(id))
                return;
            
            AudioSFXInstance loopingAudio = Play(id);
            if (!loopingAudio) return;
            
            _loopingAudio.Add(id, loopingAudio);
        }

        public void FadeOutLoop(AudioId id, float fadeDuration = 0.4f)
        {
            if (!_loopingAudio.TryGetValue(id, out AudioSFXInstance loopingAudio))
                return;
            
            loopingAudio.FadeOutLoop(fadeDuration, () => _loopingAudio.Remove(id));
        }
    }
}
