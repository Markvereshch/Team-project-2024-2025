using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public interface IShootable
{
    public void Shoot(bool isShooting);
}
public interface IRayShootable
{
    public IEnumerator CreateTrail(TrailRenderer trail, Vector3 hitPoint);
    public Vector3 GenerateRecoil();
}
public interface ILaunchable
{
    public void CreateBullet(Transform muzzleTransform);
}
public interface IReloadable
{
    public UnityAction OnReloadEnd { get; set; }
    public IEnumerator PerformReloading();
    public bool IsAbleToReload();
}
public interface IDamagable
{
    public bool IsDead { get; }
    public bool Invincible { get; set; }
    public UnityAction<float, GameObject> OnDamaged { get; set; }
    public UnityAction OnDie { get; set; }
    public void TakeDamage(float damage, GameObject source);
}
public interface IVehicleController
{
    public IShootable Weapon { get; set; }
    public ReloadScript ManualReloading { get; set; }
    public TurretControl TurretControl { get; set; }
}