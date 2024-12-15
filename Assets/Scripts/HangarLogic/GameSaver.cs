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
    public int HullLevel;
    public int WheelsLevel;
    public int CarriageLevel;
    public int FuelTankLevel;

    public VehicleData(string carName, int hullLevel, int wheelsLevel, int carriageLevel, int fuelTankLevel)
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

    public UpgradeInfo ToUpgradeInfo()
    {
        return new UpgradeInfo(HullLevel, WheelsLevel, CarriageLevel, FuelTankLevel);
    }
}

[System.Serializable]
public class ResourcesData
{
    public int Wood;
    public int Scrap;
    public int Electronic;
    public int Gasoline;
    public int Coins;

    public ResourcesData(int wood, int scrap, int electronic, int gasoline, int coins)
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

    public static ResourcesData operator -(ResourcesData a, ResourcesData b)
    {
        return new ResourcesData(
            a.Wood - b.Wood,
            a.Scrap - b.Scrap,
            a.Electronic - b.Electronic,
            a.Gasoline - b.Gasoline,
            a.Coins - b.Coins
        );
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