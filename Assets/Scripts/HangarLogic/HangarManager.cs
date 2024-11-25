using UnityEngine;
using UnityEngine.Events;

public class HangarManager : MonoBehaviour
{
    public GameObject CurrentVehicle { get; set; }
    public VehiclePurchaseData LastAvailableVehicle
    { 
        get { return lastAvailableVehicle; } 
        set 
        { 
            lastAvailableVehicle = value;
            OnVehicleSelected?.Invoke(); 
        } 
    }
    public VehiclePurchaseData VehicleToPurchase { get; private set; }

    public UnityEvent OnVehicleSelected = new UnityEvent();

    [Header("Panels")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject upgradesPanel;
    [SerializeField] private GameObject vehiclesPanel;

    public ResourcesData CurrentResources { get; private set; }
    public VehiclesData AvailableVehicles { get; set; }

    private VehiclePurchaseData lastAvailableVehicle;

    private void Start()
    {
        LoadGameData();
        ShowStartPanel();
    }

    public void ShowStartPanel()
    {
        startPanel.SetActive(true);
        upgradesPanel.SetActive(false);
        vehiclesPanel.SetActive(false);
    }

    public void LoadGameData()
    {
        var gameData = GameSaver.Load();
        SetResources(gameData.resourcesSaveData);
        SetVehicles(gameData.vehiclesSaveData);
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

    private void SetResources(ResourcesData resources)
    {
        CurrentResources = new ResourcesData
        (
            resources.Wood,
            resources.Scrap,
            resources.Electronic,
            resources.Gasoline,
            resources.Coins
        );
    }

    private void SetVehicles(VehiclesData vehicles)
    {
        AvailableVehicles = vehicles;
    }

    public VehicleData GetVehicleSaveData(string carName)
    {
        return AvailableVehicles.vehicles.Find(vehicle => vehicle.CarName == carName);
    }

    public void SetSelectedVehicle(VehiclePurchaseData vehicleData)
    {
        VehicleToPurchase = vehicleData;
    }

    public void PurchaseVehicle()
    {
        if (CanPurchaseVehicle(VehicleToPurchase))
        {
            AvailableVehicles.vehicles.Add(new VehicleData(VehicleToPurchase.CarName, 0, 0, 0, 0));
            CurrentResources.Coins -= VehicleToPurchase.BasePrice;
            SaveGameData();
            Debug.Log("Vehicle purchased!");
        }
        else
        {
            Debug.Log("Unable to purchase vehicle.");
        }

        if (IsVehicleBought(VehicleToPurchase)) //ZAMIENIT!!!
        {
            LastAvailableVehicle = VehicleToPurchase;
        }

        Debug.Log(LastAvailableVehicle);
    }

    private bool CanPurchaseVehicle(VehiclePurchaseData vehicleData)
    {
        return CurrentResources.Coins >= vehicleData.BasePrice &&
               !IsVehicleBought(vehicleData);
    }

    public bool IsVehicleBought(VehiclePurchaseData vehicleData)
    {
        return AvailableVehicles.vehicles.Exists(v => v.CarName == vehicleData.CarName);
    }

}
