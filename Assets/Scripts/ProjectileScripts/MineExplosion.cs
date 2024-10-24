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
    private Rigidbody bulletRb;

    private Vector3 onCollisionPosition;

    private void Start()
    {
        bulletRb = this.GetComponent<Rigidbody>();
        explosionAudio = GetComponentInChildren<AudioSource>();
        explosionParticles = GetComponentInChildren<ParticleSystem>();
        explosionParticles.gameObject.SetActive(false);

        Destroy(gameObject, maxLifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, damagableLayer);
        for (int i = 0; i < colliders.Length; i++)
        {
            //damage logic goes here
        }
        explosionParticles.gameObject.SetActive(true);
        explosionParticles.transform.parent = null;
        explosionParticles.Play();
        explosionAudio.Play();
        Destroy(explosionParticles.gameObject, explosionParticles.main.duration);
        Destroy(gameObject);
    }
}
