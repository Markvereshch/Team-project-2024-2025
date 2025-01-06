using UnityEngine;
using UnityEngine.Audio;

public class AudioMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    private const float curveConst = 20f;

    private const string MasterVolumeKey = "MasterVolume";
    private const string SFXVolumeKey = "SFXVolume";
    private const string MusicVolumeKey = "MusicVolume";
    private const string EnvironmentVolumeKey = "EnvironmentVolume";

    public float DefaultValue { get; private set; } = 0f;

    private void Start()
    {
        LoadVolumes();
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat(MasterVolumeKey, Mathf.Log10(volume) * curveConst);
        PlayerPrefs.SetFloat(MasterVolumeKey, volume);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat(SFXVolumeKey, Mathf.Log10(volume) * curveConst);
        PlayerPrefs.SetFloat(SFXVolumeKey, volume);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat(MusicVolumeKey, Mathf.Log10(volume) * curveConst);
        PlayerPrefs.SetFloat(MusicVolumeKey, volume);
    }

    public void SetEnvironmentVolume(float volume)
    {
        audioMixer.SetFloat(EnvironmentVolumeKey, Mathf.Log10(volume) * curveConst);
        PlayerPrefs.SetFloat(EnvironmentVolumeKey, volume);
    }

    private void LoadVolumes()
    {
        float masterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, DefaultValue);
        float sfxVolume = PlayerPrefs.GetFloat(SFXVolumeKey, DefaultValue);
        float musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, DefaultValue);
        float environmentVolume = PlayerPrefs.GetFloat(EnvironmentVolumeKey, DefaultValue);

        audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolume) * curveConst);
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolume) * curveConst);
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolume) * curveConst);
        audioMixer.SetFloat("EnvironmentVolume", Mathf.Log10(environmentVolume) * curveConst);
    }
}
