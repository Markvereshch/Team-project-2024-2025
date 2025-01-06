using UnityEngine;
using UnityEngine.UI;

public class AudioMixerUI : MonoBehaviour
{
    [Header("Audio sliders")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider environmentVolumeSlider;

    [SerializeField] private AudioMixerManager audioMixerManager;

    private void Start()
    {
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", audioMixerManager.DefaultValue);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", audioMixerManager.DefaultValue);
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", audioMixerManager.DefaultValue);
        environmentVolumeSlider.value = PlayerPrefs.GetFloat("EnvironmentVolume", audioMixerManager.DefaultValue);

        masterVolumeSlider.onValueChanged.AddListener(audioMixerManager.SetMasterVolume);
        sfxVolumeSlider.onValueChanged.AddListener(audioMixerManager.SetSFXVolume);
        musicVolumeSlider.onValueChanged.AddListener(audioMixerManager.SetMusicVolume);
        environmentVolumeSlider.onValueChanged.AddListener(audioMixerManager.SetEnvironmentVolume);
    }
}