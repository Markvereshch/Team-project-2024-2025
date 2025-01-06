using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource environmentSource;

    [Header("Default sounds")]
    [SerializeField] public AudioClip DefaultImpact;

    [Header("Soundtracks")]
    [SerializeField] private List<MusicGroup> musicGroups = new List<MusicGroup>();
    [Header("GeigerSounds")]
    [SerializeField] private AudioClip geigerLow;
    [SerializeField] private AudioClip geigerHight;

    [Header("Death soundtracks")]
    [SerializeField] private AudioClip unknown;
    [SerializeField] private AudioClip irradiated;
    [SerializeField] private AudioClip killedInAction;
    [SerializeField] private AudioClip noGasoline;

    [Header("Death sfx")]
    [SerializeField] private AudioClip morseCode;
    [SerializeField] private AudioClip flames;

    [Header("Music coroutine settings")]
    [SerializeField] private float secondsBeforeFade = 10f;
    [SerializeField] private float fadeDuration = 2f;

    private MusicGroup currentMusicGroup;
    private Coroutine musicCheckCoroutine;
    private VehicleHealth playerHealth;
    private float enemiesNearby;
    private float initialVolume;

    public VehicleHealth PlayerHealth 
    {
        get
        {
            return playerHealth;
        }
        set
        {
            playerHealth = value;
            playerHealth.OnDamaged += HandleDamage;
            playerHealth.OnDie += HandleDeath;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            ChooseMusicGroup();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChooseMusicGroup()
    {
        currentMusicGroup = musicGroups[Random.Range(0, musicGroups.Count)];
        initialVolume = musicSource.volume;
        PlayMusic(currentMusicGroup.DrivingTheme);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == musicSource.clip)
            return;

        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == musicSource.clip)
            return;

        if (sfxSource.isPlaying)
        {
            sfxSource.Stop();
        }
        sfxSource.clip = clip;
        sfxSource.Play();
    }

    public void RemoveNearbyEnemy()
    {
        enemiesNearby--;
        if (enemiesNearby > 0 && musicCheckCoroutine == null)
        {
            musicCheckCoroutine = StartCoroutine(WaitForMusicChange(secondsBeforeFade, currentMusicGroup.DrivingTheme));
        }
    }

    public void AddNearbyEnemy()
    {
        enemiesNearby++;
        if (musicSource.clip != currentMusicGroup.AlertTheme)
        {
            PlayMusic(currentMusicGroup.AlertTheme);
        }
    }


    private void HandleDamage(float damage, GameObject damageSource)
    {
        if (damageSource != null)
        {
            if (damageSource.GetComponent<DamageZone>())
                HandleDamage(geigerLow, geigerHight);
            else
                HandleDamage(currentMusicGroup.EasyFightTheme, currentMusicGroup.HardFightTheme);
        }

        if (musicCheckCoroutine != null)
            StopCoroutine(musicCheckCoroutine);

        musicCheckCoroutine = StartCoroutine(WaitForMusicChange(secondsBeforeFade, currentMusicGroup.DrivingTheme));
    }

    private void HandleDamage(AudioClip easyTheme, AudioClip hardTheme)
    {
        if (PlayerHealth.IsHalfDead())
            PlayMusic(hardTheme);
        else
            PlayMusic(easyTheme);
    }

    private void HandleDeath()
    {
        if (musicCheckCoroutine != null)
            StopCoroutine(musicCheckCoroutine);

        //musicSource.volume = initialVolume;
        switch (GameManager.Instance.GetCauseOfDeath())
        {
            case (DeathCause.NoGasoline):
                PlayMusic(noGasoline);
                PlaySFX(morseCode);
                break;
            case (DeathCause.KIA):
                PlayMusic(killedInAction);
                PlaySFX(flames);
                break;
            case (DeathCause.Unknown):
                PlayMusic(unknown);
                break;
            case (DeathCause.Radiation):
                PlayMusic(irradiated);
                PlaySFX(geigerLow);
                break;
        }
    }

    private IEnumerator WaitForMusicChange(float beforeFade, AudioClip nextClip)
    {
        float elapsedTime = 0f;

        yield return new WaitForSeconds(beforeFade);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(initialVolume, 0, elapsedTime / fadeDuration);
            yield return null;
        }

        musicSource.Stop();

        PlayMusic(nextClip);

        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0, initialVolume, elapsedTime / fadeDuration);
            yield return null;
        }
    }
}

[System.Serializable]
public struct MusicGroup
{
    public AudioClip DrivingTheme;
    public AudioClip AlertTheme;
    public AudioClip EasyFightTheme;
    public AudioClip HardFightTheme;
    public AudioClip FightEndTheme;
}
