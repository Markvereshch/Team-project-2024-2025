using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class RaycastGunScript : GunBaseScript, IShootable, IRayShootable
{
    [SerializeField] GameObject[] impacts;
    [SerializeField] Transform[] muzzleTransforms;
    [SerializeField] TrailRenderer bulletTrail;
    [SerializeField] AudioSource audioSource;
    private IReloadable reloadable;
    private ImpactManager impactManager;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioConfig.GetStartPitch(audioSource);
        reloadable = GetComponent<IReloadable>();
        impactManager = FindObjectOfType<ImpactManager>();

        var controller = gameObject.GetComponentInParent<IVehicleController>();
        if (controller != null)
        {
            controller.Weapon = this;
        }
    }

    public void Shoot(bool isShooting)
    {
        if (isShooting && !isReloading)
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
        if(Time.time > weaponConfig.fireRate + lastShootTime && currentClip > 0)
        {
            lastShootTime = Time.time;
            currentMuzzle = (currentMuzzle + 1) % muzzleTransforms.Length;
            for(int i = 0; i < weaponConfig.bulletsInOneShoot; i++)
            {
                Shoot(muzzleTransforms[currentMuzzle]);
            }
            audioConfig.PlayShootingClipOnce(audioSource);
            audioConfig.PlayTailClip(audioSource);
            currentClip--;
            OnShoot?.Invoke();
        }
    }

    public IEnumerator CreateTrail(TrailRenderer trail, Vector3 hit)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;
        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hit, time);
            time += Time.deltaTime / trail.time;
            yield return null;
        }
        trail.transform.position = hit;
        Destroy(trail.gameObject, trail.time);
    }

    public Vector3 GenerateRecoil()
    {
        Vector3 recoil = new Vector3(
        Random.Range(-weaponConfig.spread.x, weaponConfig.spread.x),
        Random.Range(-weaponConfig.spread.y, weaponConfig.spread.y),
        Random.Range(-weaponConfig.spread.z, weaponConfig.spread.z)
        );
        return recoil;
    }

    private void Shoot(Transform muzzleTransform)
    {
        RaycastHit hit;
        TrailRenderer trail = Instantiate(bulletTrail, muzzleTransform.position, Quaternion.identity);
        if (Physics.Raycast(muzzleTransform.position, muzzleTransform.forward + GenerateRecoil(), out hit, weaponConfig.range, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            //Debug.Log(hit.collider.gameObject);
            if(hit.collider != null)
            {
                impactManager.CreateImpact(hit);
                IDamagable damagable = hit.collider.GetComponentInParent<IDamagable>();
                if(damagable != null)
                {
                    float distance = Vector3.Distance(muzzleTransform.position, hit.point);
                    damagable.TakeDamage(damageConfig.GetDamage(distance), gameObject);
                }
            }
            StartCoroutine(CreateTrail(trail, hit.point));
        }
        else
        {
            StartCoroutine(CreateTrail(trail, muzzleTransform.position + (muzzleTransform.forward + GenerateRecoil()) * weaponConfig.range));
        }
    }
}
