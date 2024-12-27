using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [SerializeField] private ResourceManagerConfig resourceConfig;
    [SerializeField] private InformationUIConfig informationUIConfig;
    [SerializeField] private float weaponDropChance = 0.2f;

    [Header("Key-Value of droppableResources dictionary")]
    [SerializeField] List<ResourceType> droppableResources = new List<ResourceType>();
    [SerializeField] List<GameObject> prefabs = new List<GameObject>();

    public GameObject WeaponToDrop { get; set; }
    private Dictionary<ResourceType, GameObject> typePrefabDictionary = new Dictionary<ResourceType, GameObject>();
    private Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>();

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

    public void ChangeResourceAmount(int amount, ResourceType resourceType, bool showInfoTab = false)
    {
        int maxAmount = GetMaxResourceAmount(resourceType);

        if (resources.ContainsKey(resourceType))
        {
            var newAmount = Mathf.Clamp(resources[resourceType] + amount, 0, maxAmount);
            resources[resourceType] = newAmount;
            //Debug.Log($"Added {resourceType}: {amount}.");

            if (showInfoTab)
                InGameUIManager.Instance.InformationList.AddInformation($"{resourceType}: {amount}", informationUIConfig.GetIconByResourceType(resourceType));
        }
    }

    public int GetResourceAmount(ResourceType resourceType)
    {
        return resources.ContainsKey(resourceType) ? resources[resourceType] : 0;
    }

    public int GetMaxResourceAmount(ResourceType resourceType)
    {
        int maxAmount = 0;
        switch (resourceType)
        {
            case ResourceType.ShotgunAmmo:
                maxAmount = resourceConfig.maxShotgunAmmo;
                break;
            case ResourceType.MachineGunAmmo:
                maxAmount = resourceConfig.maxMachinegunAmmo;
                break;
            case ResourceType.ExplosiveAmmo:
                maxAmount = resourceConfig.maxExplosiveAmmo;
                break;
            case ResourceType.Wood:
                maxAmount = resourceConfig.maxWood;
                break;
            case ResourceType.Scrap:
                maxAmount = resourceConfig.maxScrap;
                break;
            case ResourceType.Electronics:
                maxAmount = resourceConfig.maxElectronics;
                break;
            case ResourceType.Gasoline:
                maxAmount = resourceConfig.maxGasoline;
                break;
            case ResourceType.Coins:
                maxAmount = resourceConfig.maxCoins;
                break;
            default:
                maxAmount = int.MaxValue;
                break;
        }
        return maxAmount + capacityBonus > int.MaxValue ? int.MaxValue : maxAmount + capacityBonus;
    }

    private void DropResource()
    {
        var notEmpty = resources.Where((key, value) => value > 0).ToList();
        int index = Random.Range(0, notEmpty.Count);
        ResourceType type = notEmpty[index].Key;
        if (Random.value <= weaponDropChance && WeaponToDrop != null)
        {
            Instantiate(WeaponToDrop, transform.position, WeaponToDrop.transform.rotation);
        }
        else if (typePrefabDictionary.TryGetValue(type, out var itemToDrop))
        {
            Instantiate(itemToDrop, transform.position, itemToDrop.transform.rotation);
        }
    }

    public void PrepareSaveData(ResourcesData resourceSaveData)
    {
        resourceSaveData.Scrap += GetResourceAmount(ResourceType.Scrap);
        resourceSaveData.Coins += GetResourceAmount(ResourceType.Coins);
        resourceSaveData.Electronic += GetResourceAmount(ResourceType.Electronics);
        resourceSaveData.Gasoline += GetResourceAmount(ResourceType.Gasoline);
        resourceSaveData.Wood += GetResourceAmount(ResourceType.Wood);
    }
}
