using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class TimerObjective : Objective
{
    [SerializeField] protected float completeTime = 120f;
    public float CompleteTime { get { return completeTime; } }
    protected float currentTime;

    public override void Reset()
    {
        base.Reset();
        GenerateReward();
        currentTime = 0;
    }

    virtual protected void CalculateTime()
    {
        currentTime += Time.deltaTime;
        if (currentTime > completeTime)
        {
            OnObjectiveCompleted?.Invoke(this);
        }
    }
}

public abstract class Objective : MonoBehaviour, IObjective
{
    [SerializeField] protected string title;
    [SerializeField] protected string description;
    [SerializeField] protected List<RewardResource> possibleRewards = new List<RewardResource>();
    [SerializeField] protected int numOfRewardResources = 3;
    protected List<RewardResource> rewardResources = new List<RewardResource>();
    public string Title { get { return title; } }
    public string Description { get { return description; } }
    public List<RewardResource> RewardResources { get { return rewardResources; } }
    public bool IsCompleted { get; protected set; }
    public UnityAction<IObjective> OnObjectiveCompleted { get; set; }

    public virtual void Reset()
    {
        IsCompleted = false;
    }

    public void GenerateReward()
    {
        rewardResources.Clear();

        if (possibleRewards.Count <= numOfRewardResources)
        {
            rewardResources.AddRange(possibleRewards);
        }
        else
        {
            var selectedRewards = new HashSet<int>();
            while (selectedRewards.Count < numOfRewardResources)
            {
                int randomIndex = Random.Range(0, possibleRewards.Count);
                if (!selectedRewards.Contains(randomIndex))
                {
                    selectedRewards.Add(randomIndex);
                    rewardResources.Add(possibleRewards[randomIndex]);
                }
            }
        }
    }
}

public interface IObjective
{
    string Title { get;}
    string Description { get; }
    List<RewardResource> RewardResources { get; }
    bool IsCompleted { get; }
    UnityAction<IObjective> OnObjectiveCompleted { get; set; }
    void Reset();
}

[System.Serializable]
public record RewardResource
{
    public ResourceType resourceType;
    public int amount;
}