using System.Collections.Generic;
using UnityEngine;

public class HuntSeasonObjective : TimerObjective
{
    private int enemiesKilled;
    public VehicleHealth PlayerHealth { get; set; }

    public void RegisterEnemy(VehicleHealth enemyHealth)
    {
        enemyHealth.OnKilled += CheckKiller;
    }

    private void CheckKiller(VehicleHealth killerHealth)
    {
        if (killerHealth.Equals(PlayerHealth))
        {
            enemiesKilled++;
            Debug.Log($"Player killed an enemy. Total kills: {enemiesKilled}");
        }
    }

    private void FixedUpdate()
    {
        if (IsCompleted)
            return;
        CalculateTime();
    }

    override public void Reset()
    {
        base.Reset();
        enemiesKilled = 0;
    }

    private void CalculateFinalReward()
    {
        for (int i = 0; i < rewardResources.Count; i++)
        {
            RewardResources[i].amount *= enemiesKilled;
        }
    }

    protected override void CalculateTime()
    {
        currentTime += Time.deltaTime;
        if (currentTime > completeTime)
        {
            IsCompleted = true;
            CalculateFinalReward();
            OnObjectiveCompleted?.Invoke(this);
        }
    }
}
