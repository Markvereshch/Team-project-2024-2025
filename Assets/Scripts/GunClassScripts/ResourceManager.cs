using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    private Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>();
    [SerializeField] private ResourceManagerConfig resourceConfig;

    private void Start()
    {
        resources[ResourceType.ShotgunAmmo] = resourceConfig.startShotgunAmmo;
        resources[ResourceType.MachineGunAmmo] = resourceConfig.startMachinegunAmmo;
        resources[ResourceType.ExplosiveAmmo] = resourceConfig.startExplosiveAmmo;
        resources[ResourceType.Wood] = resourceConfig.startWood;
        resources[ResourceType.Scrap] = resourceConfig.startScrap;
        resources[ResourceType.Electronics] = resourceConfig.startElectronics;
        resources[ResourceType.Gasoline] = resourceConfig.startGasoline;
        resources[ResourceType.Coins] = resourceConfig.startCoins;
    }

    public void ChangeResourceAmount(int amount, ResourceType resourceType)
    {
        int maxAmount = GetMaxResourceAmount(resourceType);

        if (resources.ContainsKey(resourceType))
        {
            resources[resourceType] = Mathf.Clamp(resources[resourceType] + amount, 0, maxAmount);
        }
    }

    public int GetResourceAmount(ResourceType resourceType)
    {
        return resources.ContainsKey(resourceType) ? resources[resourceType] : 0;
    }

    private int GetMaxResourceAmount(ResourceType resourceType)
    {
        switch (resourceType)
        {
            case ResourceType.ShotgunAmmo:
                return resourceConfig.maxShotgunAmmo;
            case ResourceType.MachineGunAmmo:
                return resourceConfig.maxMachinegunAmmo;
            case ResourceType.ExplosiveAmmo:
                return resourceConfig.maxExplosiveAmmo;
            case ResourceType.Wood:
                return resourceConfig.maxWood;
            case ResourceType.Scrap:
                return resourceConfig.maxScrap;
            case ResourceType.Electronics:
                return resourceConfig.maxElectronics;
            case ResourceType.Gasoline:
                return resourceConfig.maxGasoline;
            case ResourceType.Coins:
                return resourceConfig.maxCoins;
            default:
                return int.MaxValue;
        }
    }
}
