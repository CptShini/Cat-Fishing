using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Assets.Scripts.Audio
{
    public class AudioSettings : MonoBehaviour
    {
        private const string
            MasterVolume = "MasterVolume",
            MusicVolume = "MusicVolume",
            SFXVolume = "SFXVolume";
    
        [Header("Audio Mixer")]
        [SerializeField] private AudioMixer audioMixer;

        [Header("Volume Sliders")]
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;

        private void Start()
        {
            float masterVolume = PlayerPrefs.GetFloat(MasterVolume, 1f);
            float musicVolume = PlayerPrefs.GetFloat(MusicVolume, 1f);
            float sfxVolume = PlayerPrefs.GetFloat(SFXVolume, 1f);
        
            masterVolumeSlider.value = masterVolume;
            musicVolumeSlider.value = musicVolume;
            sfxVolumeSlider.value = sfxVolume;

            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
            sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);

            SetMasterVolume(masterVolume);
            SetMusicVolume(musicVolume);
            SetSFXVolume(sfxVolume);
        }

        private void SetMasterVolume(float value) => SetVolume(MasterVolume, value);
        private void SetMusicVolume(float value) => SetVolume(MusicVolume, value);
        private void SetSFXVolume(float value) => SetVolume(SFXVolume, value);
    
        private void SetVolume(string parameter, float value)
        {
            PlayerPrefs.SetFloat(parameter, value);
            audioMixer.SetFloat(parameter, Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f);
        }
    }
}
