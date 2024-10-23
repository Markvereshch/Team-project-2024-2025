using UnityEngine;

public class MineExplosion : MonoBehaviour
{
    [SerializeField] private LayerMask damagableLayer;
    [SerializeField] private AudioSource explosionAudio;
    [SerializeField] private ParticleSystem explosionParticles;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionForce = 1000f;
    [SerializeField] private float maxLifeTime = 2f;

    private void Start()
    {
        Destroy(gameObject, maxLifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("BOom");
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, damagableLayer);
        for (int i = 0; i < colliders.Length; i++)
        {
            //damage logic goes here
        }
        explosionParticles.transform.parent = null;
        if (explosionAudio != null) explosionAudio?.Play();
        if (explosionParticles != null) {
            explosionParticles.Play();
            Destroy(explosionParticles.gameObject, explosionParticles.main.duration);
        }
        Destroy(gameObject);
    }
}
