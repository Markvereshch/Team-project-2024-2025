using UnityEngine;

public class ExplosiveProjectile : MonoBehaviour
{
    [SerializeField] protected LayerMask damagableLayer;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionForce = 1000f;
    [SerializeField] private float maxLifeTime = 2f;

    protected GameObject source;
    private AudioSource explosionAudio;
    [SerializeField] private ParticleSystem explosionParticles;
    private float damage;

    virtual protected void Start()
    {
        explosionAudio = GetComponentInChildren<AudioSource>();
        explosionParticles.gameObject.SetActive(false);

        Destroy(gameObject, maxLifeTime);
    }

    private void OnCollisionEnter(Collision other)
    {
        //Debug.Log(other.gameObject);
        if (other.collider.isTrigger)
        {
            return;
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, damagableLayer, QueryTriggerInteraction.Ignore);
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

        Explode();
    }

    public void Explode()
    {
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
