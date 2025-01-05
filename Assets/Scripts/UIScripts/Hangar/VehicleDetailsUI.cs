using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class VehicleDetailsUI : MonoBehaviour
{
    [Header("About vehicle")]
    [SerializeField] TMP_Text vehicleName;
    [SerializeField] TMP_Text vehicleDescription;

    [Header("Vehicle Characteristics")]
    [Header("Survivability")]
    [SerializeField] TMP_Text health;
    [SerializeField] TMP_Text fuel;
    
    [Header("Mobility")]
    [SerializeField] TMP_Text motorTorque;
    [SerializeField] TMP_Text brake;
    [SerializeField] TMP_Text steeringRange;
    [SerializeField] TMP_Text maxSpeed;

    [Header("Carriage capacity")]
    [SerializeField] TMP_Text mgAmmo;
    [SerializeField] TMP_Text shotgunAmmo;
    [SerializeField] TMP_Text explosiveAmmo;
    [SerializeField] TMP_Text wood;
    [SerializeField] TMP_Text scrap;
    [SerializeField] TMP_Text electronics;
    [SerializeField] TMP_Text gasoline;
    [SerializeField] TMP_Text coins;

    [Header("Buying options")]
    [SerializeField] TMP_Text storeButtonText;
    [SerializeField] Button buyButton;
    [SerializeField] TMP_Text priceInCoins;

    [Header("HangarManager")]
    [SerializeField] HangarManager hangarManager;

    public void UpdateVehicleDetailsUi()
    {
        if (hangarManager.IsVehicleSelected())
        {
            priceInCoins.transform.parent.gameObject.SetActive(false);
            buyButton.interactable = false;
            storeButtonText.text = new LocalizedString("HangarLocalizationTable", "Hangar_Selected").GetLocalizedString();
        }
        else if (hangarManager.IsVehicleBought())
        {
            priceInCoins.transform.parent.gameObject.SetActive(false);
            buyButton.interactable = true;
            storeButtonText.text = new LocalizedString("HangarLocalizationTable", "Hangar_Select").GetLocalizedString();
        }
        else if (!hangarManager.CanPurchaseVehicle())
        {
            priceInCoins.transform.parent.gameObject.SetActive(true);
            buyButton.interactable = false;
            priceInCoins.text = hangarManager.VehicleToPurchase.BasePrice.ToString();
            storeButtonText.text = new LocalizedString("HangarLocalizationTable", "Hangar_UnableToBuy").GetLocalizedString();
        }
        else
        {
            priceInCoins.transform.parent.gameObject.SetActive(true);
            buyButton.interactable = true;
            priceInCoins.text = hangarManager.VehicleToPurchase.BasePrice.ToString();
            storeButtonText.text = new LocalizedString("HangarLocalizationTable", "Hangar_Buy").GetLocalizedString();
        }

        UpdateVehicleStats(hangarManager.CurrentVehicle.GetComponent<VehicleStats>(), hangarManager.CurrentVehicle.GetComponent<ResourceManager>());
        UpdateVehicleDescription(hangarManager.VehicleToPurchase.CarName);
    }

    private void UpdateVehicleStats(VehicleStats stats, ResourceManager resourceManager)
    {
        resourceManager.FetchCapacityBonus();

        health.text = stats.maxHealth.ToString();
        fuel.text = stats.maxGasoline.ToString();

        motorTorque.text = stats.motorTorque.ToString();
        brake.text = stats.brakeAcceleration.ToString();
        steeringRange.text = stats.steeringRange.ToString();
        maxSpeed.text = stats.maxSpeed.ToString();

        mgAmmo.text = resourceManager.GetMaxResourceAmount(ResourceType.MachineGunAmmo).ToString();
        shotgunAmmo.text = resourceManager.GetMaxResourceAmount(ResourceType.ShotgunAmmo).ToString();
        explosiveAmmo.text = resourceManager.GetMaxResourceAmount(ResourceType.ExplosiveAmmo).ToString();

        wood.text = resourceManager.GetMaxResourceAmount(ResourceType.Wood).ToString();
        scrap.text = resourceManager.GetMaxResourceAmount(ResourceType.Scrap).ToString();
        electronics.text = resourceManager.GetMaxResourceAmount(ResourceType.Electronics).ToString();
        gasoline.text = resourceManager.GetMaxResourceAmount(ResourceType.Gasoline).ToString();
        coins.text = resourceManager.GetMaxResourceAmount(ResourceType.Coins).ToString();
    }

    private void UpdateVehicleDescription(string techName)
    {

        vehicleName.text = new LocalizedString("HangarLocalizationTable", $"Vehicle_{techName}").GetLocalizedString();
        vehicleDescription.text = new LocalizedString("HangarLocalizationTable", $"VehicleStory_{techName}").GetLocalizedString();
    }


}
