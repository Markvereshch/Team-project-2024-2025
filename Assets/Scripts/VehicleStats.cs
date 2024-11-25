using UnityEngine;

public class VehicleStats : MonoBehaviour
{
    [Header("Durability")]
    public float maxHealth;
    [Header("Gasoline and capacity")]
    public float maxGasoline;
    [Header("Movement")]
    public float motorTorque;
    public float brakeTorque;
    public float brakeAcceleration;
    public float steeringRange;
    public float steeringRangeAtMaxSpeed;
    [Header("Configuration")]

    [SerializeField] private VehicleConfig config;
    private void Start()
    {
        maxHealth = config.health;
        maxGasoline = config.gasoline;
        if (TryGetComponent<UpgradeManager>(out var updateManager))
        {
            maxHealth += updateManager.HealthBonus;
            maxGasoline += updateManager.FuelBonus;
        }
    }
}
