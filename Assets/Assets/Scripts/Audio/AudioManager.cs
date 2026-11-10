using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }
        
        [SerializeField] private GameObject audioSFXInstancePrefab;
        [SerializeField] private AudioSFX[] audioSFXs;
        
        private readonly Dictionary<AudioId, AudioSFXInstance> _audioInstances = new();
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }
        
        public bool Play(AudioId id)
        {
            bool alreadyPlaying = _audioInstances.TryGetValue(id, out AudioSFXInstance audioSFXInstance);
            if (!alreadyPlaying)
            {
                AudioSFX audioSFX = audioSFXs.FirstOrDefault(sfx => sfx.Id == id);
                if (!audioSFX) return false;
                
                audioSFXInstance = CreateInstance(audioSFX);
                _audioInstances.Add(id, audioSFXInstance);
            }
            
            audioSFXInstance.Play();
            return true;
        }

        private AudioSFXInstance CreateInstance(AudioSFX sfx)
        {
            GameObject audioSFXInstanceObject = Instantiate(audioSFXInstancePrefab);
            AudioSFXInstance audioSFXInstance = audioSFXInstanceObject.GetComponent<AudioSFXInstance>();
            audioSFXInstance.Init(sfx, () => _audioInstances.Remove(sfx.Id));
            
            return audioSFXInstance;
        }

        public void FadeOutLoop(AudioId id, float fadeDuration = 0.4f)
        {
            if (!_audioInstances.TryGetValue(id, out AudioSFXInstance loopingAudio))
                return;
            
            loopingAudio.FadeOutLoop(fadeDuration);
        }
    }
}
