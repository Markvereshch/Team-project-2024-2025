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

    [Header("Panels")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject upgradesPanel;
    [SerializeField] private GameObject vehiclesPanel;

    private ResourcesData currentResources;
    private VehiclePurchaseData lastAvailableVehicle;

    private void Awake()
    {
        LoadGameData();
        ShowStartPanel();
    }

    private void Start()
    {
        OnResourcesChanged.Invoke(currentResources);        
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
        if (CanPurchaseVehicle(VehicleToPurchase))
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
