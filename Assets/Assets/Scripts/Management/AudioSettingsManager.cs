
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsManager : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Volume Sliders")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    private void Start()
    {
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    private void SetVolume(string parameter, float value)
    {
        audioMixer.SetFloat(parameter, Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f);
    }

    private void SetMasterVolume(float value)
    {
        SetVolume("MasterVolume", value);
    }

    private void SetMusicVolume(float value)
    {
        SetVolume("MusicVolume", value);
    }

    private void SetSFXVolume(float value)
    {
        SetVolume("SFXVolume", value);
    }
}
