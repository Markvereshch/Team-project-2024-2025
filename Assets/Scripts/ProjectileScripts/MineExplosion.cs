using UnityEngine;

public class MineExplosion : MonoBehaviour
{
    [SerializeField] private LayerMask damagableLayer;
    [SerializeField] private LayerMask pickUpLayer;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionForce = 1000f;
    [SerializeField] private float maxLifeTime = 2f;

    private AudioSource explosionAudio;
    private ParticleSystem explosionParticles;
    private GameObject source;
    private float damage;

    private void Start()
    {
        explosionAudio = GetComponentInChildren<AudioSource>();
        explosionParticles = GetComponentInChildren<ParticleSystem>();
        explosionParticles.gameObject.SetActive(false);

        Destroy(gameObject, maxLifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject);
        if (other.isTrigger)
        {
            return;
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, damagableLayer);
        for (int i = 0; i < colliders.Length; i++)
        {
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();
            if (targetRigidbody)
                targetRigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            
            var damagable = colliders[i].gameObject.GetComponentInParent<IDamagable>();
            if (damagable == null)
                continue;
            damagable.TakeDamage(damage, source);
        }
        explosionParticles.gameObject.SetActive(true);
        explosionParticles.transform.parent = null;
        explosionParticles.Play();
        explosionAudio.Play();
        Destroy(explosionParticles.gameObject, explosionParticles.main.duration);
        Destroy(gameObject);
    }

    public void SetCharacteristics(GameObject source, float damage)
    {
        this.source = source;
        this.damage = damage;
    }
}
