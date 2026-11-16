using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Audio.Music
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicPlayer : MonoBehaviour
    {
        private AudioSource _player;
        private Coroutine _fadeCoroutine;

        private void Awake() => _player = GetComponent<AudioSource>();

        public void PlayTrack(AudioBGM bgm, float fadeTime)
        {
            CancelFade();
            
            if (_player.clip != bgm.Track || !_player.isPlaying)
            {
                _player.Stop();
                _player.clip = bgm.Track;
                _player.volume = 0f;
                _player.Play();
            }

            FadeTo(bgm.Volume, fadeTime, false);
        }

        public void StopTrack(float fadeTime)
        {
            if (!_player.isPlaying) return;

            CancelFade();
            FadeTo(0f, fadeTime, true);
        }

        private void CancelFade()
        {
            if (_fadeCoroutine == null) return;

            StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = null;
        }

        private void FadeTo(float targetVolume, float fadeTime, bool stopWhenDone)
        {
            if (fadeTime <= 0f)
            {
                _player.volume = targetVolume;

                if (stopWhenDone)
                    _player.Stop();

                return;
            }

            _fadeCoroutine = StartCoroutine(
                FadeRoutine(targetVolume, fadeTime, stopWhenDone)
            );
        }

        private IEnumerator FadeRoutine(
            float targetVolume,
            float fadeTime,
            bool stopWhenDone)
        {
            float startVolume = _player.volume;
            float elapsed = 0f;

            while (elapsed < fadeTime)
            {
                elapsed += Time.unscaledDeltaTime;

                float t = Mathf.Clamp01(elapsed / fadeTime);
                _player.volume = Mathf.Lerp(startVolume, targetVolume, t);

                if (t < 1f)
                    yield return null;
            }

            _player.volume = targetVolume;

            if (stopWhenDone)
                _player.Stop();

            _fadeCoroutine = null;
        }
    }
}