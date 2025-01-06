using UnityEngine;

public class VehicleAudioController : MonoBehaviour
{
    [Header("AudioSources")]
    [SerializeField] AudioSource wheelAudioSource;
    [SerializeField] AudioSource engineAudioSource;
    [SerializeField] AudioSource impactAudioSource;
    [SerializeField] AudioSource hornAudioSource;

    [Header("Sounds")]
    [SerializeField] AudioClip engineSound;
    [SerializeField] AudioClip brakeSound;
    [SerializeField] AudioClip brakeTail;
    [SerializeField] AudioClip hornSound;
 
    [Header("Pitches")]
    [SerializeField] private float minPitch;
    [SerializeField] private float maxPitch;
    [SerializeField] private float carPitch;

    [SerializeField] private float minSpeed;
    [SerializeField] private float maxSpeed;

    [Header("Speed options")]
    [SerializeField] private float speedToBrake = 10f;
    [SerializeField] private float speedToCollide = 3f;

    private float currentSpeed;
    private bool isBraking;
    private Rigidbody rb;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        maxSpeed = GetComponent<VehicleStats>().maxSpeed;
        GetComponent<VehicleHealth>().OnDie += StopAll;
        speedToBrake = maxSpeed / 3f;

        engineAudioSource.clip = engineSound;
        wheelAudioSource.clip = brakeSound;
        hornAudioSource.clip = hornSound;

        engineAudioSource.pitch = minPitch;
        engineAudioSource.Play();

        if (GameManager.Instance != null)
            GameManager.Instance.OnGameOver += StopAll;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.isTrigger && currentSpeed > speedToCollide)
        {
            if(collision.collider.TryGetComponent<Surface>(out var surface))
            {
                impactAudioSource.clip = surface.SurfaceConfig.GetRandomCollisionSound();
            }
            else
            {
                impactAudioSource.clip = SFXManager.Instance.DefaultImpact;
            }
            impactAudioSource.Play();
        }
    }

    public void PlayEngineSound()
    {
        currentSpeed = rb.velocity.magnitude;
        carPitch = rb.velocity.magnitude / maxSpeed;

        if (currentSpeed < minSpeed)
            engineAudioSource.pitch = minPitch;
        else if (currentSpeed > minSpeed && currentSpeed < maxSpeed)
            engineAudioSource.pitch = minPitch + carPitch;
        else if (currentSpeed > maxSpeed)
            engineAudioSource.pitch = maxPitch;
    }

    public void PlayBrakeSound()
    {
        if (currentSpeed > speedToBrake && !isBraking && !wheelAudioSource.isPlaying)
        {
            wheelAudioSource.clip = brakeSound;
            wheelAudioSource.Play();
            isBraking = true;
        }
        else if (currentSpeed > 0.2f && isBraking && !wheelAudioSource.isPlaying)
        {
            wheelAudioSource.Stop();
            wheelAudioSource.clip = brakeTail;
            wheelAudioSource.Play();
            isBraking = false;
        }
    }

    public void PlayHornSound()
    {
        if(!hornAudioSource.isPlaying)
        {
            hornAudioSource.Play();
        }
    }

    public void StopAll()
    {
        wheelAudioSource.Stop();
        engineAudioSource.Stop();
        hornAudioSource.Stop();
    }
}
