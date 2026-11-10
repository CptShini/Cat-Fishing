using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Audio
{
    public class AudioSFXInstance : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;

        private float _lifetime, _clipLifetime;
        private Action _onComplete;

        public void Init(AudioSFX sfx, Action onComplete)
        {
            name = sfx.ToString();
            audioSource.clip = sfx.Clip;
            audioSource.volume = sfx.Volume;
            audioSource.loop = sfx.Looping;
            
            _clipLifetime = sfx.Clip.length;
            _onComplete = onComplete;
        }

        public void Play()
        {
            if (audioSource.isPlaying)
            {
                if (audioSource.loop) return;
                
                audioSource.Stop();
            }
            
            _lifetime = _clipLifetime;
            audioSource.Play();
        }
        
        private void Update()
        {
            if (audioSource.loop) return;
            
            _lifetime -= Time.deltaTime;
            if (_lifetime > 0f) return;
            
            Destroy(gameObject, 1f);
        }
        
        public void FadeOutLoop(float fadeDuration) => 
            StartCoroutine(FadeOutCoroutine(fadeDuration));

        private IEnumerator FadeOutCoroutine(float duration)
        {
            float startVolume = audioSource.volume;
            
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
                yield return null;
            }
            
            audioSource.Stop();
            Destroy(gameObject);
        }

        private void OnDestroy() => _onComplete.Invoke();
    }
}