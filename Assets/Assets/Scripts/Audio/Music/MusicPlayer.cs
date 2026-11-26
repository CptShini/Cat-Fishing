using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Audio.Music
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource _playerA, _playerB;

        private AudioClip _targetTrack;
        private Coroutine _transitionCoroutine;

        public void PlayTrack(AudioBGM bgm, float fadeTime)
        {
            if (!bgm || !bgm.Track)
                return;
            
            AudioSource playing = FindPlayingSource(bgm.Track);
            if (_targetTrack == bgm.Track && playing) return;
            
            _targetTrack = bgm.Track;
            CancelTransition();

            // Check whether the requested track is already playing.
            AudioSource incoming = playing;
            AudioSource outgoing;

            if (incoming)
            {
                // Reuse the source already playing the requested track.
                outgoing = GetOtherSource(incoming);

                if (!outgoing.isPlaying)
                    outgoing = null;
            }
            else
            {
                // Keep the currently loudest source as the outgoing track.
                outgoing = GetLoudestPlayingSource();

                // Use the other source for the incoming track.
                incoming = !outgoing
                    ? _playerA
                    : GetOtherSource(outgoing);

                incoming.Stop();
                incoming.clip = bgm.Track;
                incoming.volume = 0f;
                incoming.Play();
            }

            Crossfade(outgoing, incoming, bgm.Volume, fadeTime);
        }

        public void StopTrack(float fadeTime)
        {
            CancelTransition();

            if (!_playerA.isPlaying && !_playerB.isPlaying)
                return;

            if (fadeTime <= 0f)
            {
                StopBoth();
                return;
            }

            _targetTrack = null;
            _transitionCoroutine = StartCoroutine(FadeOutRoutine(fadeTime));
        }

        private void Crossfade(
            AudioSource outgoing,
            AudioSource incoming,
            float targetVolume,
            float fadeTime)
        {
            if (fadeTime <= 0f)
            {
                if (outgoing)
                {
                    outgoing.Stop();
                    outgoing.volume = 0f;
                }

                incoming.volume = targetVolume;
                return;
            }

            _transitionCoroutine = StartCoroutine(
                CrossfadeRoutine(outgoing, incoming, targetVolume, fadeTime)
            );
        }

        private IEnumerator CrossfadeRoutine(
            AudioSource outgoing,
            AudioSource incoming,
            float targetVolume,
            float fadeTime)
        {
            float outgoingStart = outgoing ? outgoing.volume : 0f;
            float incomingStart = incoming.volume;
            float elapsed = 0f;

            while (elapsed < fadeTime)
            {
                elapsed += Time.unscaledDeltaTime;

                float t = Mathf.Clamp01(elapsed / fadeTime);
                float outgoingT = 1f - Mathf.Pow(1f - t, 2f);
                float incomingT = t * t;

                if (outgoing)
                    outgoing.volume = Mathf.Lerp(outgoingStart, 0f, outgoingT);

                incoming.volume = Mathf.Lerp(incomingStart, targetVolume, incomingT);

                if (t < 1f)
                    yield return null;
            }

            // Ensure the final volumes are exact.
            if (outgoing)
            {
                outgoing.Stop();
                outgoing.volume = 0f;
            }

            incoming.volume = targetVolume;

            _transitionCoroutine = null;
        }

        private IEnumerator FadeOutRoutine(float fadeTime)
        {
            float startA = _playerA.volume;
            float startB = _playerB.volume;
            float elapsed = 0f;

            while (elapsed < fadeTime)
            {
                elapsed += Time.unscaledDeltaTime;

                float t = Mathf.Clamp01(elapsed / fadeTime);

                _playerA.volume = Mathf.Lerp(startA, 0f, t);
                _playerB.volume = Mathf.Lerp(startB, 0f, t);

                if (t < 1f)
                    yield return null;
            }

            StopBoth();
            _transitionCoroutine = null;
        }

        private AudioSource FindPlayingSource(AudioClip clip)
        {
            if (_playerA.isPlaying && _playerA.clip == clip)
                return _playerA;

            if (_playerB.isPlaying && _playerB.clip == clip)
                return _playerB;

            return null;
        }

        private AudioSource GetLoudestPlayingSource()
        {
            if (!_playerA.isPlaying)
                return _playerB.isPlaying ? _playerB : null;

            if (!_playerB.isPlaying)
                return _playerA;

            return _playerA.volume >= _playerB.volume
                ? _playerA
                : _playerB;
        }

        private AudioSource GetOtherSource(AudioSource source) => 
            source == _playerA ? _playerB : _playerA;

        private void StopBoth()
        {
            _playerA.Stop();
            _playerB.Stop();

            _playerA.volume = 0f;
            _playerB.volume = 0f;
        }

        private void CancelTransition()
        {
            if (_transitionCoroutine == null)
                return;

            StopCoroutine(_transitionCoroutine);
            _transitionCoroutine = null;
        }
    }
}