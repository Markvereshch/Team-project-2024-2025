using System.Collections.Generic;
using UnityEngine;

public class VehicleEventController : MonoBehaviour
{
    private EntityHealth entityHealth;
    [SerializeField] private ParticleSystem onDeathParticle;
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private List<AudioClip> damageSounds = new List<AudioClip>();
    private AudioSource vehicleAudio;
    private void Start()
    {
        entityHealth = GetComponent<EntityHealth>();
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

        foreach (var child in wheels)
        {
            child.gameObject.SetActive(false);
        }

        if (turret != null)
        {
            turret.gameObject.transform.parent = null;
            var rb = turret.gameObject.AddComponent<Rigidbody>();
            turret.enabled = false;
            rb.AddForce(Vector3.up * 30f, ForceMode.Impulse);
        }
    }
}
