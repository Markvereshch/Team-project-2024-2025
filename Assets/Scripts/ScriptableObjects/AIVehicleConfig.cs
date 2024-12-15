using UnityEngine;

[CreateAssetMenu(fileName = "AIVehicleConfig", menuName = "VehicleConfigs/AIVehicleConfig", order = 2)]
public class AIVehicleConfig : ScriptableObject
{
    [Header("AI-controlled")]
    public GameObject agentAI;
    public Vector3 frontSensorPosition;
    public Vector3 backSensorPosition;
    public float spaceBetweenSensors;
}
