using UnityEngine;

public class ProjectileGunScript : GunBaseScript, IShootable, ILaunchable
{
    [SerializeField] Transform[] muzzleTransforms;
    [SerializeField] AudioSource audioSource;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float launchForce = 30f;
    [SerializeField] private Rigidbody playerRb;

    private Rigidbody shellRb;
    private IReloadable reloadable;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioConfig.GetStartPitch(audioSource);
        reloadable = GetComponent<IReloadable>();
        shellRb = bulletPrefab.GetComponent<Rigidbody>();
        playerRb = gameObject.GetComponentInParent<Rigidbody>();

        var controller = gameObject.GetComponentInParent<IVehicleController>();
        if (controller != null)
        {
            controller.Weapon = this;
        }
    }

    public void Shoot(bool isShooting)
    {
        if (isShooting)
        {
            PerformShot();
        }
        if (reloadable.IsAbleToReload())
        {
            StartCoroutine(reloadable.PerformReloading());
        }
    }

    private void PerformShot()
    {
        if (Time.time > weaponConfig.fireRate + lastShootTime && currentClip > 0)
        {
            lastShootTime = Time.time;
            currentMuzzle = (currentMuzzle + 1) % muzzleTransforms.Length;
            for (int i = 0; i < weaponConfig.bulletsInOneShoot; i++)
            {
                CreateBullet(muzzleTransforms[currentMuzzle]);
            }
            audioConfig.PlayShootingClipOnce(audioSource);
            currentClip--;
            audioConfig.PlayTailClip(audioSource);
            onShoot?.Invoke();
        }
    }

    public void CreateBullet(Transform muzzleTransform)
    {
        Rigidbody shellInstance = Instantiate(shellRb, muzzleTransform.position, muzzleTransform.rotation) as Rigidbody;
        shellInstance.GetComponent<ExplosiveProjectile>().SetCharacteristics(gameObject, damageConfig.GetDamage());
        shellInstance.velocity = launchForce * (muzzleTransform.forward + GenerateRecoil());
        GenerateBackForce(muzzleTransform);
    }

    private Vector3 GenerateRecoil()
    {
        Vector3 recoil = new Vector3(
        Random.Range(-weaponConfig.spread.x, weaponConfig.spread.x),
        Random.Range(-weaponConfig.spread.y, weaponConfig.spread.y),
        Random.Range(-weaponConfig.spread.z, weaponConfig.spread.z)
        );
        return recoil;
    }

    private void GenerateBackForce(Transform muzzleTransform)
    {
        playerRb.AddForceAtPosition(launchForce * (muzzleTransform.forward * (-1)) * 100, transform.position);
    }
}
