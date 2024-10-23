using UnityEngine;
using UnityEngine.Events;

public abstract class GunBaseScript : MonoBehaviour
{
    public WeaponConfig weaponConfig;
    public ReloadingConfig reloadConfig;
    public DamageConfig damageConfig;
    public AudioConfigScriptableObject audioConfig;

    public AmmoManager ammoManager;
    public float lastShootTime;
    public int currentMuzzle;
    public int currentClip;
    public bool isReloading;

    public UnityEvent onShoot;

    private void Start()
    {
        ammoManager = GetComponent<AmmoManager>();
        onShoot ??= new UnityEvent();
    }
}
