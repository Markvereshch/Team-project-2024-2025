using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UpgradeManager : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private List<Upgrade> hullUpgrades = new List<Upgrade>();
    [SerializeField] private List<Upgrade> wheelUpgrades = new List<Upgrade>();
    [SerializeField] private List<Upgrade> carriegeUpgrades = new List<Upgrade>();
    [SerializeField] private List<Upgrade> fuelTankUpgrades = new List<Upgrade>();
    public Dictionary<VehiclePart, List<Upgrade>> Upgrades { get; private set; } = new Dictionary<VehiclePart, List<Upgrade>>();
    public UnityEvent OnStatsChanged = new UnityEvent();

    private UpgradeInfo upgradeInfo = new UpgradeInfo(0, 0, 0, 0);
    public UpgradeInfo UpgradeInfo
    {
        get => upgradeInfo;
        set
        {
            upgradeInfo = value;
            CalculateModifiers();
            UpdateVisuals();
            OnStatsChanged.Invoke();
        }
    }

    public int FuelBonus { get; private set; }
    public int HealthBonus { get; private set; }
    public int CapacityBonus { get; private set; }
    public int AccelerationBonus { get; private set; }
    public int BrakeBonus { get; private set; }
    public int SteerAngleBonus { get; private set; }

    private void Awake()
    {
        Upgrades.Add(VehiclePart.Hull, hullUpgrades);
        Upgrades.Add(VehiclePart.Wheels, wheelUpgrades);
        Upgrades.Add(VehiclePart.Carriage, carriegeUpgrades);
        Upgrades.Add(VehiclePart.FuelTank, fuelTankUpgrades);
    }

    private void Start()
    {
        UpdateVisuals();
        CalculateModifiers();
    }

    private void UpdateVisuals()
    {
        GetComponent<UpgradeVisualManager>().ShowVisuals(UpgradeInfo);
    }

    private void CalculateModifiers()
    {
        ResetModifiers();

        foreach (var pair in Upgrades)
        {
            int level = UpgradeInfo.typeLevelPair[pair.Key];
            var upgradesToApply = pair.Value;

            for (int i = 0; i < Mathf.Min(level, (uint)upgradesToApply.Count); i++)
            {
                foreach (var details in upgradesToApply[i].upgradeDetails)
                {
                    ApplyModifier(details);
                }
            }
        }
    }

    private void ResetModifiers()
    {
        FuelBonus = 0;
        HealthBonus = 0;
        CapacityBonus = 0;
        AccelerationBonus = 0;
        BrakeBonus = 0;
        SteerAngleBonus = 0;
    }

    private void ApplyModifier(UpgradeTypeValuePair details)
    {
        switch (details.type)
        {
            case UpgradeType.HealthPoints:
                HealthBonus += details.value;
                break;
            case UpgradeType.FuelPoints:
                FuelBonus += details.value;
                break;
            case UpgradeType.CarriageCapacity:
                CapacityBonus += details.value;
                break;
            case UpgradeType.BrakeSensitivity:
                BrakeBonus += details.value;
                break;
            case UpgradeType.Acceleration:
                AccelerationBonus += details.value;
                break;
            case UpgradeType.SteeringAngle:
                SteerAngleBonus += details.value;
                break;
        }
    }
}
