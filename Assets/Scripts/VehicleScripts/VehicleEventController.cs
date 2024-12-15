using System.Collections.Generic;
using UnityEngine;

public class VehicleEventController : MonoBehaviour
{
    private VehicleHealth entityHealth;
    [SerializeField] private ParticleSystem onDeathParticle;
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private List<AudioClip> damageSounds = new List<AudioClip>();
    private AudioSource vehicleAudio;
    [SerializeField] private float timeToDestroy = 15f;
    private float turretMass = 500f;
    private float explosionForce = 30f;

    private void Start()
    {
        entityHealth = GetComponent<VehicleHealth>();
        vehicleAudio = GetComponent<AudioSource>();
        entityHealth.OnDamaged += OnDamage;
        entityHealth.OnDie += OnDie;
    }

    private void OnDamage(float damageAmount, GameObject source)
    {
        int index = Random.Range(0, damageSounds.Count);
        vehicleAudio.PlayOneShot(damageSounds[index]);
    }

    private void OnDie()
    {
        var vfx = Instantiate(onDeathParticle.gameObject, transform.position, Quaternion.identity);
        Destroy(vfx, 2f);
        vehicleAudio.PlayOneShot(explosionSound);

        var wheels = GetComponentsInChildren<WheelCollider>();
        var turret = GetComponentInChildren<TurretControl>();

        foreach (var wheel in wheels)
        {
            wheel.gameObject.SetActive(false);
        }

        if (turret != null)
        {
            turret.gameObject.transform.parent = null;
            var rb = turret.gameObject.AddComponent<Rigidbody>();
            rb.mass = turretMass;
            turret.enabled = false;
            rb.AddForce(Vector3.up * explosionForce, ForceMode.Impulse);
            Destroy(turret.gameObject, timeToDestroy);
        }

        Destroy(gameObject, timeToDestroy);
    }
}
