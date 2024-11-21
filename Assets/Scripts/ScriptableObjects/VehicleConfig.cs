using UnityEngine;

[CreateAssetMenu(fileName = "VehicleConfig", menuName = "VehicleConfigs/VehicleConfig", order = 1)]
public class VehicleConfig : ScriptableObject
{
    [Header("Characteristics")]
    public float health;
    public float gasoline;
}
