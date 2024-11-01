using UnityEngine;

public class AICarControl : MonoBehaviour, IVehicleController
{
    private EntityHealth entityHealth;
    private AITargetSeeker seeker;

    public IShootable Weapon { get; set; }
    public ReloadScript ManualReloading { get; set; }
    public TurretControl TurretControl { get; set; }

    private void Start()
    {
        entityHealth = GetComponent<EntityHealth>();
        TurretControl = GetComponentInChildren<TurretControl>();
        Weapon = GetComponentInChildren<IShootable>();
        seeker = GetComponent<AITargetSeeker>();
    }

    private void Update()
    {
        if (entityHealth.IsDead)
        {
            return;
        }
        if (TurretControl != null)
        {
            TurretControl.Move();
        }
        if (Weapon != null && seeker.Target != null && TurretControl.IsTargetInSight(seeker.Target))
        {
            Weapon.Shoot(true);
        }
    }
}
