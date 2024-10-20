using UnityEngine;

[CreateAssetMenu(fileName = "Audio Configuration", menuName = "Guns/AudioConfig", order = 3)]
public class AudioConfigScriptableObject : ScriptableObject
{
    [Range(0, 1f)] public float volume = 1f;
    public AudioClip shootingClip;
    public AudioClip shootingTaleClip;
    public AudioClip reloadStartClip;
    public AudioClip reloadStopClip;
    public float startPitch;
    [SerializeField] float pitchRange = 0.15f;

    public void GetStartPitch(AudioSource source)
    {
        startPitch = source.pitch;
    }

    public void PlayShootingClipOnce(AudioSource source)
    {
        if(shootingClip != null)
        {
            source.pitch = Random.Range(startPitch - pitchRange, startPitch + pitchRange);
            source.PlayOneShot(shootingClip, volume);
            if(!source.isPlaying)
                source.pitch = startPitch;
        }
    }

    public void PlayShootingClip(AudioSource source)
    {
        if (shootingClip != null)
        {
            source.pitch = Random.Range(startPitch - pitchRange, startPitch + pitchRange);
            source.clip = shootingClip;
            source.volume = volume;
            if(!source.isPlaying)
            {
                source.Play();
            }
            source.pitch = startPitch;
        }
    }
    public void PlayStartReloadClip(AudioSource source)
    {
        if (reloadStartClip != null)
        {
            source.pitch = Random.Range(startPitch - pitchRange, startPitch + pitchRange);
            source.PlayOneShot(reloadStartClip, volume);
            if (!source.isPlaying)
                source.pitch = startPitch;
        }
    }

    public void PlayStopReloadClip(AudioSource source)
    {
        if (reloadStopClip != null)
        {
            source.pitch = Random.Range(startPitch - pitchRange, startPitch + pitchRange);
            source.PlayOneShot(reloadStopClip, volume);
            if (!source.isPlaying)
                source.pitch = startPitch;
        }
    }

    public void PlayTailClip(AudioSource source)
    {
        if (shootingTaleClip != null)
        {
            source.pitch = Random.Range(startPitch - pitchRange, startPitch + pitchRange);
            source.PlayOneShot(shootingTaleClip, volume);
            if (!source.isPlaying)
                source.pitch = startPitch;
        }
    }
}
