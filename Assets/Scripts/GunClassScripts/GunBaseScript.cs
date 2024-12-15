using UnityEngine;
using UnityEngine.Events;

public abstract class GunBaseScript : MonoBehaviour
{
    public WeaponConfig weaponConfig;
    public ReloadingConfig reloadConfig;
    public DamageConfig damageConfig;
    public AudioConfigScriptableObject audioConfig;
    public ResourceManager resourceManager;

    public float lastShootTime;
    public int currentMuzzle;
    public int currentClip;
    public bool isReloading;

    public UnityEvent OnShoot { get; set; }

    private void Awake()
    {
        OnShoot ??= new UnityEvent();
        resourceManager = GetComponentInParent<ResourceManager>();
    }
}
