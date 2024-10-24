using UnityEngine;

public class AmmoManager : MonoBehaviour
{
    [SerializeField] private int machinegunAmmo = 50;
    [SerializeField] private int shotgunAmmo = 15;
    [SerializeField] private int explosiveAmmo = 5;
    [SerializeField] private AmmoManagerConfig ammoConfig;


    public void ChangeAmmo(int amount, GunType gunType)
    {
        switch (gunType)
        {
            case GunType.Shotgun:
                shotgunAmmo = Mathf.Clamp(shotgunAmmo + amount, 0, ammoConfig.maxShotgunAmmo);
                break;
            case GunType.MachineGun:
                machinegunAmmo = Mathf.Clamp(machinegunAmmo + amount, 0, ammoConfig.maxMachinegunAmmo);
                break;
            case GunType.Explosive:
                explosiveAmmo = Mathf.Clamp(explosiveAmmo + amount, 0, ammoConfig.maxExplosiveAmmo);
                break;
        }
    }

    public int GetAmmo(GunType gunType)
    {
        switch (gunType)
        {
            case GunType.Shotgun:
                return shotgunAmmo;
            case GunType.MachineGun:
                return machinegunAmmo;
            case GunType.Explosive:
                return explosiveAmmo;
            default:
                return 0;
        }
    }
}
