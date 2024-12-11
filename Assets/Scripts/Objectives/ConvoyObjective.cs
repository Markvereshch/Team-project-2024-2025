using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvoyObjective : Objective
{
    [Header("Common Kill Enemy Objective parameters")]
    [Tooltip("All possible transport prefabs")]
    [SerializeField] private List<GameObject> transportPrefabs = new List<GameObject>();
    [Tooltip("All possible path start points of moving targets to be killed")]
    [SerializeField] private List<Waypoint> pathStartPositions = new List<Waypoint>();

    private GameObject transport;
    public GameObject Player { get; set; }

    public override void Reset()
    {
        base.Reset();

        if (transport != null)
        {
            Destroy(transport);
            transport = null;
        }
    }

    public void PrepareTransport()
    {
        int spawnIndex;
        Waypoint spawnPosition;

        spawnIndex = Random.Range(0, pathStartPositions.Count);
        spawnPosition = pathStartPositions[spawnIndex];

        transport = SpawnVehicle(spawnPosition);
    }

    private GameObject SpawnVehicle(Waypoint startWaypoint)
    {
        int index = Random.Range(0, transportPrefabs.Count);
        var vehicleToSpawn = transportPrefabs[index];

        var instantiated = Instantiate(vehicleToSpawn, startWaypoint.transform.position, startWaypoint.transform.rotation);

        CarInitializer initialized = instantiated.GetComponent<CarInitializer>();
        initialized.SetCarPlayability(false);

        VehicleHealth targetHealth = instantiated.GetComponent<VehicleHealth>();
        targetHealth.Fraction = Fraction.Ally;
        targetHealth.OnKilled += HandleTargetDead;

        AICarControl carControl = instantiated.GetComponent<AICarControl>();
        carControl.IsTransport = true;

        AICarMovement ai = instantiated.GetComponent<AICarMovement>();
        ai.Activate();

        var transportState = new TransportCarState();
        carControl.SetState(transportState);
        transportState.OnDestinationReached += HandleDestinationReaching;
        transportState.SetInitialTarget(startWaypoint);

        var transport = instantiated.AddComponent<Transport>();
        transport.Player = Player;

        return instantiated;
    }

    private void HandleDestinationReaching()
    {
        IsCompleted = true;
        CalculateFinalReward();
        OnObjectiveCompleted?.Invoke(this);
    }

    private void HandleTargetDead(VehicleHealth enemyHealth)
    {
        if (!IsCompleted)
        {
            CalculateFinalReward();
            OnObjectiveCompleted?.Invoke(this);
        }
    }

    private void CalculateFinalReward()
    {
        int modifier = IsCompleted? 1 : 0;

        for (int i = 0; i < rewardResources.Count; i++)
        {
            RewardResources[i].amount *= modifier;
        }
    }
}
