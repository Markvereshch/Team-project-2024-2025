using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class VehicleUpgradeUI : MonoBehaviour
{
    [Header("Details tab")]
    [SerializeField] private GameObject detailsTab;
    [SerializeField] private TMP_Text vehicleName;
    [SerializeField] private Button upgradeButton;

    [Header("Upgrade level visuals")]
    [SerializeField] private Slider[] upgradeSliders;

    [Header("Upgrade buttons")]
    [SerializeField] private Button[] upgradeButtons;

    [Header("Upgrade prices")]
    [SerializeField] private TMP_Text coins;
    [SerializeField] private TMP_Text gasoline;
    [SerializeField] private TMP_Text scrap;
    [SerializeField] private TMP_Text wood;
    [SerializeField] private TMP_Text electronics;

    [Header("Upgrade characteristics")]
    [SerializeField] private TMP_Text health;
    [SerializeField] private TMP_Text capacity;
    [SerializeField] private TMP_Text fuel;
    [SerializeField] private TMP_Text steeringAngle;
    [SerializeField] private TMP_Text brake;
    [SerializeField] private TMP_Text acceleration;

    private Dictionary<VehiclePart, Slider> upgradeToSlider = new Dictionary<VehiclePart, Slider>();
    private Dictionary<VehiclePart, Button> upgradeToButton = new Dictionary<VehiclePart, Button>();
    private UpgradeManager upgradeManager;
    [SerializeField] HangarManager hangarManager;

    public void Initialize(UpgradeManager manager, string carName)
    {
        upgradeManager = manager;
        vehicleName.text = new LocalizedString("HangarLocalizationTable", $"Vehicle_{carName}").GetLocalizedString();

        MapUIElements();
        RefreshLevels();
        RefreshButtons();
    }

    public void SetUpgradeButtonInteractivity(bool isInteractable)
    {
        upgradeButton.interactable = isInteractable;
    }

    public void RefreshButtons()
    {
        foreach (var pair in upgradeToButton)
        {
            var part = pair.Key;
            var button = pair.Value;

            if (upgradeManager == null || !upgradeManager.Upgrades.TryGetValue(part, out var upgradeList))
            {
                button.interactable = false;
                continue;
            }

            int currentLevel = upgradeManager.UpgradeInfo.typeLevelPair[part];

            if (currentLevel >= upgradeList.Count)
            {
                button.interactable = false;
            }
            else
            {
                button.interactable = true;
            }
        }
    }

    private void MapUIElements()
    {
        for (int i = 0; i < upgradeSliders.Length; i++)
        {
            var part = (VehiclePart)i;
            upgradeToSlider[part] = upgradeSliders[i];
            upgradeToButton[part] = upgradeButtons[i];
        }
    }

    public void RefreshLevel(VehiclePart part, int? level)
    {
        if (upgradeToSlider.TryGetValue(part, out var slider))
        {
            slider.value = level.HasValue ? (float)level / upgradeManager.Upgrades[part].Count : 1;

            if (slider.value == 1)
            {
                DisableUpgradeButton(part);
            }
        }
    }

    public void RefreshLevels()
    {
        foreach (var pair in upgradeToSlider)
        {
            var part = pair.Key;
            RefreshLevel(part, upgradeManager != null ? upgradeManager.UpgradeInfo.typeLevelPair[part] : null);
        }
    }

    public void OpenDetailsTab(int part)
    {
        detailsTab.SetActive(true);
        ShowUpgradeDetails((VehiclePart) part);
        ShowUpgradeCosts((VehiclePart) part);
    }

    public void CloseDetailsTab()
    {
        detailsTab.SetActive(false);
    }

    public void DisableUpgradeButton(VehiclePart part)
    {
        if (upgradeToButton.TryGetValue(part, out var button))
        {
            button.interactable = false;
        }
    }

    private void ShowUpgradeDetails(VehiclePart part)
    {
        HideAllCharacteristics();

        if (!upgradeManager.Upgrades.TryGetValue(part, out var upgrades))
            return;

        var currentLevel = upgradeManager.UpgradeInfo.typeLevelPair[part];
        if (currentLevel == upgrades.Count)
            return;

        foreach (var upgrade in upgrades[currentLevel].upgradeDetails)
        {
            SetUiText(GetCharacteristicText(upgrade.type), upgrade.value, true);
        }
    }

    private TMP_Text GetCharacteristicText(UpgradeType type) =>
        type switch
        {
            UpgradeType.Acceleration => acceleration,
            UpgradeType.SteeringAngle => steeringAngle,
            UpgradeType.FuelPoints => fuel,
            UpgradeType.CarriageCapacity => capacity,
            UpgradeType.HealthPoints => health,
            UpgradeType.BrakeSensitivity => brake,
            _ => null,
        };

    private void HideAllCharacteristics()
    {
        health.transform.parent.gameObject.SetActive(false);
        capacity.transform.parent.gameObject.SetActive(false);
        fuel.transform.parent.gameObject.SetActive(false);
        steeringAngle.transform.parent.gameObject.SetActive(false);
        brake.transform.parent.gameObject.SetActive(false);
        acceleration.transform.parent.gameObject.SetActive(false);
    }

    private void SetUiText(TMP_Text textElement, int value, bool showSign = false)
    {
        if (value > 0)
        {
            textElement.transform.parent.gameObject.SetActive(true);

            if (showSign)
                textElement.text = value.ToString("+0");
            else
                textElement.text = value.ToString();
        }
        else
        {
            textElement.transform.parent.gameObject.SetActive(false);
        }
    }

    private void ShowUpgradeCosts(VehiclePart part)
    {
        if (!upgradeManager.Upgrades.TryGetValue(part, out var upgrades))
            return;

        var currentLevel = upgradeManager.UpgradeInfo.typeLevelPair[part];


        if (currentLevel == upgrades.Count)
        {
            upgradeToButton[part].interactable = false;
            return;
        }

        var cost = upgrades[currentLevel].upgradeCost;
        upgradeButton.interactable = CanAffordUpgrade(cost, hangarManager.CurrentResources);

        SetUiText(coins, cost.Coins);
        SetUiText(gasoline, cost.Gasoline);
        SetUiText(scrap, cost.Scrap);
        SetUiText(wood, cost.Wood);
        SetUiText(electronics, cost.Electronic);
    }

    private bool CanAffordUpgrade(ResourcesData cost, ResourcesData availableResources)
    {
        return availableResources.Wood >= cost.Wood &&
               availableResources.Scrap >= cost.Scrap &&
               availableResources.Electronic >= cost.Electronic &&
               availableResources.Gasoline >= cost.Gasoline &&
               availableResources.Coins >= cost.Coins;
    }
}
