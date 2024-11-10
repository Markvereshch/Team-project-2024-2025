using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private float spawnInterval = 30.0f;
    private float lastSpawnTime = -30f;

    public bool CanSpawn()
    {
        return Time.time >= lastSpawnTime + spawnInterval;
    }

    public void SetLastSpawnTime()
    {
        lastSpawnTime = Time.time;
    }
}
