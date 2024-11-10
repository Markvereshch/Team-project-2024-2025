using UnityEngine;

[CreateAssetMenu(fileName = "EntityConfig", menuName = "VehicleConfigs/EntityConfig", order = 1)]
public class EntityConfig : ScriptableObject
{
    public float health;
    public float gasoline;
    public GameObject agentAI;
}
