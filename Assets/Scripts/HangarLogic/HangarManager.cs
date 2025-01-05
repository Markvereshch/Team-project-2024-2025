using UnityEngine;
using UnityEngine.Events;

public class HangarManager : MonoBehaviour
{
    public GameObject CurrentVehicle { get; set; }
    public VehiclesData AvailableVehicles { get; set; }
    public VehiclePurchaseData VehicleToPurchase { get; set; }
    public VehiclePurchaseData LastAvailableVehicle
    { 
        get { return lastAvailableVehicle; } 
        set 
        { 
            lastAvailableVehicle = value;
            OnVehicleSelected?.Invoke(); 
        } 
    }
    public ResourcesData CurrentResources
    {
        get => currentResources;
        set
        {
            currentResources = value;
            OnResourcesChanged?.Invoke(currentResources);
        }
    }

    public UnityEvent OnVehicleSelected = new UnityEvent();
    public UnityEvent<ResourcesData> OnResourcesChanged = new UnityEvent<ResourcesData>();


    private ResourcesData currentResources;
    private VehiclePurchaseData lastAvailableVehicle;

    private void Awake()
    {
        LoadGameData();
    }

    private void Start()
    {
        OnResourcesChanged.Invoke(currentResources);        
    }

    public void LoadGameData()
    {
        var gameData = GameSaver.Load();
        CurrentResources = gameData.resourcesSaveData;
        AvailableVehicles = gameData.vehiclesSaveData;
    }

    public void SaveGameData()
    {
        ResourcesData resourcesToSave = new ResourcesData(
            CurrentResources.Wood,
            CurrentResources.Scrap,
            CurrentResources.Electronic,
            CurrentResources.Gasoline,
            CurrentResources.Coins);
        GameSaveData dataToSave = new(resourcesToSave, AvailableVehicles);
        GameSaver.Save(dataToSave);
    }

    public VehicleData GetVehicleSaveData(string carName)
    {
        return AvailableVehicles.vehicles.Find(vehicle => vehicle.CarName == carName);
    }

    public void PurchaseVehicle()
    {
        if (CanPurchaseVehicle() && !IsVehicleBought())
        {
            AvailableVehicles.vehicles.Add(new VehicleData(VehicleToPurchase.CarName, 0, 0, 0, 0));

            CurrentResources = new ResourcesData(
                CurrentResources.Wood,
                CurrentResources.Scrap, 
                CurrentResources.Electronic, 
                CurrentResources.Gasoline, 
                CurrentResources.Coins - VehicleToPurchase.BasePrice
                );

            OnVehicleSelected.Invoke();
            SaveGameData();
            Debug.Log("Vehicle purchased!");
        }
        else
        {
            Debug.Log("Unable to purchase vehicle.");
        }

        if (IsVehicleBought()) //ZAMIENIT!!!
        {
            LastAvailableVehicle = VehicleToPurchase;
        }

        Debug.Log(LastAvailableVehicle);
    }

    public bool CanPurchaseVehicle()
    {
        return CurrentResources.Coins >= VehicleToPurchase.BasePrice;
    }

    public bool IsVehicleBought()
    {
        return AvailableVehicles.vehicles.Exists(v => v.CarName == VehicleToPurchase.CarName);
    }

    public bool IsVehicleSelected()
    {
        return lastAvailableVehicle == VehicleToPurchase;
    }
}
