using System.Collections.Generic;
using UnityEngine;

public class KillEnemyObjective : Objective
{
    [Header("Common Kill Enemy Objective parameters")]
    [Tooltip("All possible target prefabs")]
    [SerializeField] private List<GameObject> targetPrefabs = new List<GameObject>();
    [Tooltip("Determines whether target follows some path or not")]
    [SerializeField] private bool isTargetConvoy;
    [Tooltip("Amount of 'targets' per hideout to be killed")]
    [SerializeField] private int enemiesToKill = 1;
    [Tooltip("All possible locations of stationary targets to be killed")]
    [SerializeField] private List<Transform> targetHideouts = new List<Transform>();
    [Tooltip("All possible path start points of moving targets to be killed")]
    [SerializeField] private List<Transform> pathStartPositions = new List<Transform>();

    private int killedEnemies = 0;

    public override void Reset()
    {
        base.Reset();
        killedEnemies = 0;
    }

    public void PrepareEnemy()
    {
        int spawnIndex;
        Transform spawnPosition;

        if (isTargetConvoy)
        {
            spawnIndex = Random.Range(0, pathStartPositions.Count);
            spawnPosition = pathStartPositions[spawnIndex];
        }
        else
        {
            spawnIndex = Random.Range(0, targetHideouts.Count);
            spawnPosition = targetHideouts[spawnIndex];
        }

        var vehicle = SpawnVehicle(spawnPosition);
    }

    private GameObject SpawnVehicle(Transform spawnPosition)
    {
        int index = Random.Range(0, targetPrefabs.Count);
        var vehicleToSpawn = targetPrefabs[index];

        var instantiated = Instantiate(vehicleToSpawn, spawnPosition.transform.position, spawnPosition.transform.rotation);

        CarInitializer initialized = instantiated.GetComponent<CarInitializer>();
        initialized.SetCarPlayability(false);
        

        VehicleHealth targetHealth = instantiated.GetComponent<VehicleHealth>();
        targetHealth.Fraction = Fraction.Enemy;

        AICarControl carControl = instantiated.GetComponent<AICarControl>();
        carControl.IsTransport = isTargetConvoy;

        AICarMovement ai = instantiated.GetComponent<AICarMovement>();
        ai.Activate();

        targetHealth.OnKilled += HandleTargetDead;

        return instantiated;
    }

    private void HandleTargetDead(VehicleHealth enemyHealth)
    {
        killedEnemies++;

        if (killedEnemies == enemiesToKill)
        {
            IsCompleted = true;
            OnObjectiveCompleted?.Invoke(this);
        }
    }
}
