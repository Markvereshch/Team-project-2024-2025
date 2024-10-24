using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Configuration", menuName = "Guns/AmmoManagerConfig", order = 5)]
public class AmmoManagerConfig : ScriptableObject
{
    public int maxMachinegunAmmo = 500;
    public int maxShotgunAmmo = 300;
    public int maxExplosiveAmmo = 20;
}
