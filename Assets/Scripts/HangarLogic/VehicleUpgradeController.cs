using UnityEngine;

public class VehicleUpgradeController : MonoBehaviour
{
    private HangarManager hangarManager;
    private UpgradeManager upgradeManager;

    private void Start()
    {
        hangarManager = GetComponent<HangarManager>();
        hangarManager.OnVehicleSelected.AddListener(SetCar);
        SetCar();
    }

    public void SetCar()
    {
        upgradeManager = hangarManager.CurrentVehicle.GetComponent<UpgradeManager>();
    }

    public void Upgrade(int part)
    {
        if (!upgradeManager || !upgradeManager.Upgrades.TryGetValue((VehiclePart) part, out var upgradeList))
        {
            Debug.Log(upgradeManager == null);
            return;
        }

        uint currentLevel = upgradeManager.UpgradeInfo.typeLevelPair[(VehiclePart)part];
        if (currentLevel >= upgradeList.Count)
        {
            Debug.Log("Max level reached.");
            return;
        }

        var nextUpgrade = upgradeList[(int)currentLevel];
        if (nextUpgrade.upgradeCost.CanAfford(hangarManager.CurrentResources))
        {
            nextUpgrade.upgradeCost.DeductFrom(hangarManager.CurrentResources);
            //ÏÎÔÈÊÑÈÒÜ ÎÁÍÎÂËÅÍÈÅ ÀÂÒÎÌÎÁÈËß
            UpgradeInfo newInfo = new UpgradeInfo(upgradeManager.UpgradeInfo);
            newInfo.typeLevelPair[(VehiclePart)part]++;
            upgradeManager.UpgradeInfo = newInfo;
            //
            Debug.Log($"Upgraded {(VehiclePart)part} to level {upgradeManager.UpgradeInfo.typeLevelPair[(VehiclePart)part]}.");
            SaveUpgrades(newInfo);
        }
        else
        {
            Debug.Log("Not enough resources.");
        }
    }

    private void SaveUpgrades(UpgradeInfo info)
    {
        var upgraded = hangarManager.AvailableVehicles.vehicles.Find(car => car.CarName == hangarManager.LastAvailableVehicle.CarName);
        upgraded.FromUpgradeInfo(info);
        hangarManager.SaveGameData();
    }
}
