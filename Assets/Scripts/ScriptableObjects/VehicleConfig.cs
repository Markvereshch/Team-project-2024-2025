using UnityEngine;

[CreateAssetMenu(fileName = "VehicleConfig", menuName = "VehicleConfigs/VehicleConfig", order = 1)]
public class VehicleConfig : ScriptableObject
{
    [Header("Characteristics")]
    public float health;
    public float gasoline;
    [Header("Movement")]
    public float motorTorque;
    public float brakeTorque;
    public float brakeAcceleration;
    public float steeringRange;
    public float steeringRangeAtMaxSpeed;
    public float maxSpeed;
}
