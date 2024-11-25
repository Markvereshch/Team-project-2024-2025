using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    private Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>();
    [SerializeField] private ResourceManagerConfig resourceConfig;
    [SerializeField] private float weaponDropChance = 0.2f;

    [Header("Key-Value of droppableResources dictionary")]
    [SerializeField] List<ResourceType> droppableResources = new List<ResourceType>();
    [SerializeField] List<GameObject> prefabs = new List<GameObject>();

    public GameObject WeaponToDrop { get; set; }
    private Dictionary<ResourceType, GameObject> typePrefabDictionary = new Dictionary<ResourceType, GameObject>();

    private int capacityBonus;

    private void Awake()
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

    private void Start()
    {
        GetComponent<VehicleHealth>().OnDie += DropResource;

        int min = Mathf.Min(droppableResources.Count, prefabs.Count);
        for (int i = 0; i < min; i++)
        {
            typePrefabDictionary.TryAdd(droppableResources[i], prefabs[i]);
        }
        FetchCapacityBonus();
    }

    public void FetchCapacityBonus()
    {
        if (TryGetComponent<UpgradeManager>(out var updateManager))
        {
            capacityBonus = updateManager.CapacityBonus;
        }
    }

    public void ChangeResourceAmount(int amount, ResourceType resourceType)
    {
        int maxAmount = GetMaxResourceAmount(resourceType) + capacityBonus;
        
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

    private void DropResource()
    {
        var notEmpty = resources.Where((key, value) => value > 0).ToList();
        int index = Random.Range(0, notEmpty.Count);
        ResourceType type = notEmpty[index].Key;
        if (Random.value <= weaponDropChance)
        {
            Instantiate(WeaponToDrop, transform.position, WeaponToDrop.transform.rotation);
        }
        else if (typePrefabDictionary.TryGetValue(type, out var itemToDrop))
        {
            Instantiate(itemToDrop, transform.position, itemToDrop.transform.rotation);
        }
    }
}
