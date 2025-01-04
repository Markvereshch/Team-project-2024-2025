using UnityEngine;

public class VehicleUpgradeController : MonoBehaviour
{
    [SerializeField] private UpgradeUI upgradeUI;

    private HangarManager hangarManager;
    private UpgradeManager upgradeManager;
    public int CurrentPart { get; set; }

    private void Start()
    {
        hangarManager = GetComponent<HangarManager>();
        hangarManager.OnVehicleSelected.AddListener(OnVehicleSelected);
        OnVehicleSelected();
    }

    private void OnVehicleSelected()
    {
        upgradeManager = hangarManager.CurrentVehicle.GetComponent<UpgradeManager>();
        upgradeUI.Initialize(upgradeManager, hangarManager.VehicleToPurchase.CarName);
    }

    public void Upgrade()
    {
        if (!ValidateUpgrade(out var nextUpgrade, out var currentLevel))
            return;

        if (nextUpgrade.upgradeCost.CanAfford(hangarManager.CurrentResources))
        {
            ApplyUpgrade(nextUpgrade);
            Debug.Log($"Upgraded {(VehiclePart)CurrentPart} to level {currentLevel + 1}.");
        }
        else
        {
            Debug.Log("Not enough resources.");
        }
    }

    private bool ValidateUpgrade(out Upgrade nextUpgrade, out int currentLevel)
    {
        nextUpgrade = null;
        currentLevel = 0;

        if (upgradeManager == null || !upgradeManager.Upgrades.TryGetValue((VehiclePart)CurrentPart, out var upgradeList))
        {
            Debug.Log("Upgrade manager is not set");
            return false;
        }

        currentLevel = upgradeManager.UpgradeInfo.typeLevelPair[(VehiclePart)CurrentPart];
        if (currentLevel >= upgradeList.Count)
        {
            Debug.Log("Max level reached.");
            return false;
        }

        nextUpgrade = upgradeList[currentLevel];
        return true;
    }

    private void ApplyUpgrade(Upgrade nextUpgrade)
    {
        hangarManager.CurrentResources -= nextUpgrade.upgradeCost;

        UpgradeInfo newInfo = new UpgradeInfo(upgradeManager.UpgradeInfo);
        newInfo.typeLevelPair[(VehiclePart)CurrentPart]++;
        upgradeManager.UpgradeInfo = newInfo;

        SaveUpgrades(newInfo);

        upgradeUI.RefreshLevel((VehiclePart)CurrentPart, newInfo.typeLevelPair[(VehiclePart)CurrentPart]);

        if (newInfo.typeLevelPair[(VehiclePart)CurrentPart] >=
            upgradeManager.Upgrades[(VehiclePart)CurrentPart].Count)
        {
            upgradeUI.DisableUpgradeButton((VehiclePart)CurrentPart);
            upgradeUI.CloseDetailsTab();
        }
    }

    private void SaveUpgrades(UpgradeInfo info)
    {
        var upgraded = hangarManager.AvailableVehicles.vehicles.Find(car =>
            car.CarName == hangarManager.LastAvailableVehicle.CarName);

        upgraded.FromUpgradeInfo(info);
        hangarManager.SaveGameData();
    }
}
