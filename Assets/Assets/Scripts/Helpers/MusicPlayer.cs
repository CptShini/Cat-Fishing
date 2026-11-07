using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClip[] music;
        
        private bool _playing;
        private AudioSource _player;
    
        private void Awake() => _player = GetComponent<AudioSource>();

        public void PlayTrack(int index, float fadeTime)
        {
            if (_playing || index < 0 || index >= music.Length) return;
            
            _playing = true;
            _player.clip = music[index];
            StartCoroutine(_player.FadeIn(fadeTime));
        }

        public void StopTrack(float fadeTime)
        {
            if (!_playing) return;
            
            _playing = false;
            StartCoroutine(_player.FadeOut(fadeTime));
        }
    }
    
    public static class AudioFadeOut
    {
        public static IEnumerator FadeIn(this AudioSource audioSource, float fadeTime)
        {
            float endVolume = audioSource.volume;
            audioSource.volume = 0;
            audioSource.Play();

            while (audioSource.volume < endVolume)
            {
                audioSource.volume += endVolume * Time.deltaTime / fadeTime;
                
                yield return null;
            }
        }
        
        public static IEnumerator FadeOut(this AudioSource audioSource, float fadeTime)
        {
            float startVolume = audioSource.volume;

            while (audioSource.volume > 0)
            {
                audioSource.volume -= startVolume * Time.deltaTime / fadeTime;

                yield return null;
            }

            audioSource.Stop();
            audioSource.volume = startVolume;
        }
    }
}
