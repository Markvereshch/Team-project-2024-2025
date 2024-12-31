using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;

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

    public override void CreateUIElement()
    {
        ObjectiveUiElement = InventoryUIManager.Instance.AddObjectiveToScrolableList(icon, Title, Description, true, completeTime);
    }
}

public abstract class Objective : MonoBehaviour, IObjective
{
    [Header("Localization")]
    [SerializeField] private LocalizedString titleLocalized;
    [SerializeField] private LocalizedString descriptionLocalized;

    [SerializeField] protected Sprite icon;
    [SerializeField] protected List<RewardResource> possibleRewards = new List<RewardResource>();
    [SerializeField] protected int numOfRewardResources = 3;
    protected List<RewardResource> rewardResources = new List<RewardResource>();

    public string Title => titleLocalized.GetLocalizedString();
    public string Description => descriptionLocalized.GetLocalizedString();
    public Sprite Icon { get { return icon; } }
    public List<RewardResource> RewardResources { get { return rewardResources; } }
    public bool IsCompleted { get; protected set; }
    public UnityAction<IObjective> OnObjectiveCompleted { get; set; }
    public ObjectiveUIElement ObjectiveUiElement { get; protected set; }

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

    public virtual void CreateUIElement()
    {
        ObjectiveUiElement = InventoryUIManager.Instance.AddObjectiveToScrolableList(icon, Title, Description);
    }

    public override bool Equals(object obj)
    {
        if (obj is Objective otherObjective)
        {
            return Title.Equals(otherObjective.Title) && Description.Equals(otherObjective.Description);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return System.HashCode.Combine(Title, Description);
    }
}

public interface IObjective
{
    string Title { get;}
    string Description { get; }
    Sprite Icon { get; }
    List<RewardResource> RewardResources { get; }
    bool IsCompleted { get; }
    ObjectiveUIElement ObjectiveUiElement { get; }
    UnityAction<IObjective> OnObjectiveCompleted { get; set; }
    void Reset(); 
    void CreateUIElement();
    void GenerateReward();
}

[System.Serializable]
public record RewardResource
{
    public ResourceType resourceType;
    public int amount;
}