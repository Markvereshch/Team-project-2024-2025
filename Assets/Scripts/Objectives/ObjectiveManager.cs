using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;

public class ObjectiveManager : MonoBehaviour
{
    [Header("Localized Status Keys")]
    [SerializeField] private LocalizedString newObjectiveStatus;
    [SerializeField] private LocalizedString completedObjectiveStatus;
    [SerializeField] private LocalizedString failedObjectiveStatus;
    [Header("Common Objective Manager Settings")]
    [Tooltip("Max number of available objectives at one time")]
    [SerializeField] private int maxNumberOfObjectives = 3;
    [Tooltip("Number of completed(failed) objectives")]
    [SerializeField] private int completedObjectives = 0;
    [Header("Information tab colors")]
    [SerializeField] private Color newObjectiveColor = new Color(255f, 255f, 255f, 30f);
    [SerializeField] private Color completedObjectiveColor = new Color(0f, 255f, 0f, 30f);
    [SerializeField] private Color failedObjectiveColor = new Color(255f, 0f, 0f, 30f);

    public static ObjectiveManager Instance { get; private set; }

    public GameObject Player
    {
        get
        {
            return player;
        }
        set
        {
            player = value;
            resourceManager = player.GetComponent<ResourceManager>();
            StartCoroutine(ObjectivesCoroutine());
        }
    }
    public HashSet<Objective> CurrentObjectives { get { return currentObjectives; } }

    private GameObject player;
    private ResourceManager resourceManager;
    private List<Objective> objectives = new List<Objective>();
    private HashSet<Objective> currentObjectives = new HashSet<Objective>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public IEnumerator ObjectivesCoroutine()
    {
        yield return new WaitForSeconds(3f);
        GenerateObjectives();
        yield return null;
    }

    public void GenerateObjectives()
    {
        if (objectives.Count == 0)
        {
            objectives = GetComponents<Objective>().ToList();
        }

        DestroyObjectives();

        for (int i = 0; i < maxNumberOfObjectives; i++)
        {
            var objective = objectives[Random.Range(0, objectives.Count)];
            if (currentObjectives.Add(objective))
            {
                objective.Reset();
                PrepareObjective(objective);
                objective.OnObjectiveCompleted += CompleteObjective;
                string localizedStatus = newObjectiveStatus.GetLocalizedString();
                InGameUIManager.Instance.ObjectiveInfoList.AddInformation($"{localizedStatus}: \"{objective.Title}\"", newObjectiveColor, objective.Icon);
            }
        }
    }

    private void PrepareObjective(IObjective objective)
    {
        if (player == null)
            return;

        if (objective is ReachLocationObjective reachLocationObjective)
        {
            reachLocationObjective.PlayerTransform = player.transform;
            reachLocationObjective.SetDestinationPoint();
        }
        else if (objective is HuntSeasonObjective huntSeasonObjective) 
        {
            huntSeasonObjective.PlayerHealth = player.GetComponent<VehicleHealth>();
        }
        else if (objective is KillEnemyObjective killEnemyObjective)
        {
            killEnemyObjective.PrepareEnemy();
        }
        else if (objective is ConvoyObjective convoyObjective)
        {
            convoyObjective.Player = player;
            convoyObjective.PrepareTransport();
        }

        objective.GenerateReward();
        objective.CreateUIElement();
    }

    private void CompleteObjective(IObjective objective)
    {
        foreach(var awardResource in objective.RewardResources)
        { 
            if (awardResource.amount > 0)
                resourceManager.ChangeResourceAmount(awardResource.amount, awardResource.resourceType, true);
        }
        completedObjectives++;
        objective.OnObjectiveCompleted -= CompleteObjective;

        string localizedStatus = objective.IsCompleted
                    ? completedObjectiveStatus.GetLocalizedString()
                    : failedObjectiveStatus.GetLocalizedString();

        InGameUIManager.Instance.ObjectiveInfoList.AddInformation(
            $"{localizedStatus}: {objective.Title}",
            objective.IsCompleted ? completedObjectiveColor : failedObjectiveColor,
            objective.Icon
        );

        objective.ObjectiveUiElement.SetStatus(objective.IsCompleted);

        if (completedObjectives ==  currentObjectives.Count)
        {
            StartCoroutine(ObjectivesCoroutine());
        }
    }

    private void DestroyObjectives()
    {
        currentObjectives.Clear();
        InventoryUIManager.Instance.RemoveOldObjectives();
        completedObjectives = 0;
    }
}
