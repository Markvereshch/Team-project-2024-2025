using UnityEngine;

public class VehicleManager : MonoBehaviour
{
    [Header("VehicleSpawnPoint")]
    [SerializeField] private GameObject spawnpoint;
    [Header("Available vehicles")]
    [SerializeField] private VehicleStore vehicleStore;

    private HangarManager hangarManager;
    private int index;

    private int previousIndex = -1;

    private int Index
    {
        get => index;
        set
        {
            if (value < 0)
                index = 0;
            else if (value > vehicleStore.vehicles.Count - 1)
                index = vehicleStore.vehicles.Count - 1;
            else
                index = value;
        }
    }

    private void Start()
    {
        Index = 0;
        hangarManager = GetComponent<HangarManager>();
        HandleSelection();
        hangarManager.LastAvailableVehicle = vehicleStore.GetVehicleById(Index);
    }

    public void SetVehicleShop()
    {
        Index = hangarManager.LastAvailableVehicle.Id;
        previousIndex = Index;
        //Debug.Log(Index + " " + previousIndex);
    }

    public void Select(int step)
    {
        Index += step;
        HandleSelection();
    }

    private void HandleSelection()
    {
        if (previousIndex != Index)
        {
            var selectedVehicle = vehicleStore.GetVehicleById(Index);
            hangarManager.VehicleToPurchase = selectedVehicle;
            hangarManager.CurrentVehicle = SpawnVehicle(selectedVehicle);
            previousIndex = Index;
        }
    }

    public GameObject SpawnVehicle(VehiclePurchaseData vehicleToSpawn)
    {
        if (hangarManager.CurrentVehicle)
            Destroy(hangarManager.CurrentVehicle);

        var instantiated = Instantiate(vehicleToSpawn.Prefab, spawnpoint.transform.position, spawnpoint.transform.rotation);
        var upgradeManager = instantiated.GetComponent<UpgradeManager>();
        if (upgradeManager != null)
        {
            var vehicleData = hangarManager.GetVehicleSaveData(vehicleToSpawn.CarName);
            if (vehicleData != null)
            {
                UpgradeInfo carUpgrades = new(vehicleData.HullLevel, vehicleData.WheelsLevel, vehicleData.CarriageLevel, vehicleData.FuelTankLevel);
                upgradeManager.UpgradeInfo = carUpgrades;
            }
        }
        return instantiated;
    }

    public void SelectAvailableVehicle()
    {
        if (hangarManager.LastAvailableVehicle == hangarManager.VehicleToPurchase)
        {
            hangarManager.OnVehicleSelected.Invoke();
            return;
        }

        if (hangarManager.CurrentVehicle)
            Destroy(hangarManager.CurrentVehicle);

        hangarManager.CurrentVehicle = SpawnVehicle(hangarManager.LastAvailableVehicle);
    }
}

