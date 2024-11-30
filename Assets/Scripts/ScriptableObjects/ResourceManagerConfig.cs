using UnityEngine;

[CreateAssetMenu(fileName = "ResourceManagerConfig", menuName = "VehicleConfigs/ResourceManagerConfig", order = 0)]
public class ResourceManagerConfig : ScriptableObject
{
    [Header("Ammunition")]
    public int maxMachinegunAmmo = 500;
    public int startMachinegunAmmo = 50;
    public int maxShotgunAmmo = 300;
    public int startShotgunAmmo = 15;
    public int maxExplosiveAmmo = 20;
    public int startExplosiveAmmo = 5;

    [Header("Resources")]
    public int maxWood = 120;
    public int startWood = 0;
    public int maxScrap = 80;
    public int startScrap = 0;
    public int maxElectronics = 20;
    public int startElectronics = 0;
    public int maxGasoline = 100;
    public int startGasoline = 0;
    public int maxCoins = 500000;
    public int startCoins = 0;
}