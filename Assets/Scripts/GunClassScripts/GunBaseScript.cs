using UnityEngine;

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

    private void Start()
    {
        ammoManager = GetComponent<AmmoManager>();
    }
}
