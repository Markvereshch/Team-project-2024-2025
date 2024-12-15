using UnityEngine;

public class Surface : MonoBehaviour
{
    [SerializeField] private SurfaceImpactConfig surfaceConfig;
    public SurfaceImpactConfig SurfaceConfig { get { return surfaceConfig; } }
}