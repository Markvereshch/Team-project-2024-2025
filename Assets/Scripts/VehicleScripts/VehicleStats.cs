using UnityEngine;
using UnityEngine.Events;

public class VehicleStats : MonoBehaviour
{
    [Header("Durability")]
    public float maxHealth;
    [Header("Gasoline")]
    public float maxGasoline;
    [Header("Movement")]
    public float motorTorque;
    public float brakeTorque;
    public float brakeAcceleration;
    public float steeringRange;
    public float steeringRangeAtMaxSpeed;
    public float maxSpeed;

    [Header("Configuration object")]
    [SerializeField] private VehicleConfig vehicleConfig;
    private UpgradeManager upgradeManager;
    
    private void Awake()
    {
        LoadFromConfig();
        if (TryGetComponent(out upgradeManager))
        {
            upgradeManager.OnStatsChanged.AddListener(PrepareStats);
            ApplyUpgrades();
        }
    }
    private void PrepareStats()
    {
        LoadFromConfig();
        ApplyUpgrades();
    }

    private void LoadFromConfig()
    {
        if (vehicleConfig == null)
        {
            Debug.LogError("Vehicle config is not set");
            return;
        }
        maxHealth = vehicleConfig.health;
        maxGasoline = vehicleConfig.gasoline;
        motorTorque = vehicleConfig.motorTorque;
        brakeTorque = vehicleConfig.brakeTorque;
        brakeAcceleration = vehicleConfig.brakeAcceleration;
        steeringRange = vehicleConfig.steeringRange;
        steeringRangeAtMaxSpeed = vehicleConfig.steeringRangeAtMaxSpeed;
        maxSpeed = vehicleConfig.maxSpeed;
    }

    private void ApplyUpgrades()
    {
        maxHealth += upgradeManager.HealthBonus;
        maxGasoline += upgradeManager.FuelBonus;

        motorTorque += upgradeManager.AccelerationBonus;
        brakeTorque += upgradeManager.AccelerationBonus;
        brakeAcceleration += upgradeManager.BrakeBonus;
        steeringRange += upgradeManager.SteerAngleBonus;
        steeringRangeAtMaxSpeed += upgradeManager.SteerAngleBonus;
    }
}
