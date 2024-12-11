using System.Collections.Generic;
using UnityEngine;

public class ReachLocationObjective : TimerObjective
{
    [Header("Reach Location Objective parameters")]
    [Tooltip("Min. approach distance to complete the objective")]
    [SerializeField] private float completeDistance = 20f;
    [Tooltip("All possible locations of 'ReachLocation' objective")]
    [SerializeField] private List<GameObject> reachLocationDestinations = new List<GameObject>();
    public Transform DestinationPoint { get; set; }
    public Transform PlayerTransform { get; set; }

    private void FixedUpdate()
    {
        if (IsCompleted || PlayerTransform == null)
            return;

        if (Vector3.Distance(PlayerTransform.position, DestinationPoint.position) < completeDistance)
        {
            IsCompleted = true;
        }

        CalculateTime();
    }

    public void SetDestinationPoint()
    {
        int destinationIndex = Random.Range(0, reachLocationDestinations.Count);
        DestinationPoint = reachLocationDestinations[destinationIndex].transform;
    }

    private void CalculateFinalReward()
    {
        int modifier = IsCompleted ? 1 : 0;

        for (int i = 0; i < rewardResources.Count; i++)
        {
            RewardResources[i].amount *= modifier;
        }
    }

    protected override void CalculateTime()
    {
        currentTime += Time.deltaTime;
        if (IsCompleted || currentTime > completeTime)
        {
            CalculateFinalReward();
            OnObjectiveCompleted?.Invoke(this);
        }
    }
}
