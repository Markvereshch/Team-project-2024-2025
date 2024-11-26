using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{

    [SerializeField] private AudioMixer GameAudioMixer;
    [SerializeField] private Slider PickupSlider;

    public void SetPickupVolume()
    {
        float volume = PickupSlider.value;
        GameAudioMixer.SetFloat("PickupVolume", Mathf.Log10(volume)*20);
    }

}
