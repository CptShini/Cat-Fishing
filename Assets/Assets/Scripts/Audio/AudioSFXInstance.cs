using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Audio
{
    public class AudioSFXInstance : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;

        public void Play(AudioSFX sfx)
        {
            audioSource.clip = sfx.Clip;
            audioSource.volume = sfx.Volume;
            audioSource.loop = sfx.Looping;
            
            audioSource.Play();

            if (sfx.Looping) return;
            
            Destroy(gameObject, audioSource.clip.length + 1);
        }
        
        public void FadeOutLoop(float fadeDuration, Action onComplete) => 
            StartCoroutine(FadeOutCoroutine(fadeDuration, onComplete));

        private IEnumerator FadeOutCoroutine(float duration, Action onComplete)
        {
            float startVolume = audioSource.volume;
            
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
                yield return null;
            }

            onComplete.Invoke();
            audioSource.Stop();
            Destroy(gameObject);
        }
    }
}