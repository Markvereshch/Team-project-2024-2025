using UnityEngine;

public class ImpactManager : MonoBehaviour
{
    [SerializeField] private SurfaceImpactConfig defaultImpactConfig;

    public static ImpactManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CreateImpact(RaycastHit hit)
    {
        GameObject impact;

        if (hit.collider.TryGetComponent<Surface>(out var surface))
        {
            impact = Instantiate(surface.SurfaceConfig.impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            if (surface.SurfaceConfig.impactSounds.Count > 0)
                AudioSource.PlayClipAtPoint(surface.SurfaceConfig.GetRandomImpactSound(), hit.transform.position);
        } 
        else
        {
            impact = Instantiate(defaultImpactConfig.impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            if (defaultImpactConfig.impactSounds.Count > 0)
                AudioSource.PlayClipAtPoint(defaultImpactConfig.GetRandomImpactSound(), hit.transform.position);
        }
        Destroy(impact, 1f);
    }
}
