using System.Collections;
using UnityEngine;

public interface IShootable
{
    public void Shoot();
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
    public IEnumerator PerformReloading();
    public bool IsAbleToReload();
}
public interface IDamagable
{
    public int CurrentHealth { get; }
    public int MaxHealth { get; }
    public delegate void TakeDamageEvent(int damage);
    public event TakeDamageEvent OnTakeDamage;
    public delegate void DeathEvent(Vector3 position);
    public event DeathEvent OnDeath;
    public void TakeDamage(int damage);
}
public interface IEquipable<GameObject> 
{
    GameObject WeaponPrefab {  get; }
}