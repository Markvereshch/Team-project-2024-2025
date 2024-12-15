using UnityEngine;

[CreateAssetMenu(fileName = "VehicleMovementConfig", menuName = "VehicleConfigs/VehicleMovementConfig", order = 2)]
public class VehicleMovementConfig : ScriptableObject
{
    [Header("Movement")]
    public float motorTorque;
    public float brakeTorque;
    public float brakeAcceleration;
    public float steeringRange;
    public float steeringRangeAtMaxSpeed;
}
