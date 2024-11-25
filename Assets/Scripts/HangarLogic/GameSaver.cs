using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class GameSaver
{
    private static readonly string filePath = Application.persistentDataPath + "/gameSave.json";

    private static readonly ResourcesData defaultResources = 
        new ResourcesData(0, 0, 0, 100000, 100000);
    private static readonly VehicleData defaultCar = 
        new VehicleData("Toyota", 0, 0, 0, 0);
    private static readonly VehiclesData defaultCarList = 
        new VehiclesData(new List<VehicleData>() { defaultCar });

    private static GameSaveData bufferedSave;

    public static void Save(GameSaveData saveData)
    {
        string saveAsJson = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(filePath, saveAsJson);
        bufferedSave = saveData;
        Debug.Log("Game saved to: " + filePath);
    }

    public static GameSaveData Load()
    {
        if (bufferedSave == null)
        {
            if (File.Exists(filePath))
            {
                string saveAsJson = File.ReadAllText(filePath);
                GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(saveAsJson);
                Debug.Log("Game loaded from " + filePath);
                bufferedSave = saveData;
            }
            else
            {
                bufferedSave = new GameSaveData(defaultResources, defaultCarList);
            }
        }
        return bufferedSave;
    }
}

[System.Serializable]
public class VehicleData
{
    public string CarName;
    public uint HullLevel;
    public uint WheelsLevel;
    public uint CarriageLevel;
    public uint FuelTankLevel;

    public VehicleData(string carName, uint hullLevel, uint wheelsLevel, uint carriageLevel, uint fuelTankLevel)
    {
        CarName = carName;
        HullLevel = hullLevel;
        WheelsLevel = wheelsLevel;
        CarriageLevel = carriageLevel;
        FuelTankLevel = fuelTankLevel;
    }

    public void FromUpgradeInfo(UpgradeInfo upgradeInfo)
    {
        HullLevel = upgradeInfo.HullLevel;
        WheelsLevel = upgradeInfo.WheelsLevel;
        CarriageLevel = upgradeInfo.CarriageLevel;
        FuelTankLevel = upgradeInfo.FuelTankLevel;
    }
}

[System.Serializable]
public class ResourcesData
{
    public uint Wood;
    public uint Scrap;
    public uint Electronic;
    public uint Gasoline;
    public uint Coins;

    public ResourcesData(uint wood, uint scrap, uint electronic, uint gasoline, uint coins)
    {
        Wood = wood;
        Scrap = scrap;
        Electronic = electronic;
        Gasoline = gasoline;
        Coins = coins;
    }

    public bool CanAfford(ResourcesData currentResources)
    {
        return currentResources.Coins >= Coins &&
               currentResources.Wood >= Wood &&
               currentResources.Scrap >= Scrap &&
               currentResources.Gasoline >= Gasoline &&
               currentResources.Electronic >= Electronic;
    }

    public void DeductFrom(ResourcesData currentResources)
    {
        currentResources.Coins -= Coins;
        currentResources.Wood -= Wood;
        currentResources.Scrap -= Scrap;
        currentResources.Gasoline -= Gasoline;
        currentResources.Coins -= Coins;
    }
}

[System.Serializable]
public class VehiclesData
{
    public List<VehicleData> vehicles;

    public VehiclesData(List<VehicleData> vehicles)
    {
        this.vehicles = vehicles;
    }
}

[System.Serializable]
public class GameSaveData
{
    public ResourcesData resourcesSaveData;
    public VehiclesData vehiclesSaveData;

    public GameSaveData(ResourcesData resourcesSaveData, VehiclesData vehiclesSaveData)
    {
        this.resourcesSaveData = resourcesSaveData;
        this.vehiclesSaveData = vehiclesSaveData;
    }
}