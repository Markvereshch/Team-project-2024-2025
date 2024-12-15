using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private float spawnInterval = 30.0f;
    private float lastSpawnTime = -30f;
    [SerializeField] private List<EnemyGroup> enemyGroups = new List<EnemyGroup>();

    public bool CanSpawn()
    {
        return Time.time >= lastSpawnTime + spawnInterval;
    }

    public void SetLastSpawnTime()
    {
        lastSpawnTime = Time.time;
    }

    public void AddEnemyGroup(EnemyGroup group)
    {
        enemyGroups.Add(group);
        group.OnGroupDestroyed.AddListener(RemoveEnemyGroup);
    }

    private void RemoveEnemyGroup(EnemyGroup group)
    {
        enemyGroups.Remove(group);
    }
}
