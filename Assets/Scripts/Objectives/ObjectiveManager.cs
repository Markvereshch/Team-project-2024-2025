using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    [Header("Common Objective Manager Settings")]
    [Tooltip("Max number of available objectives at one time")]
    [SerializeField] private int numOfObjectives = 3;
    [Tooltip("Number of completed(failed) objectives")]
    [SerializeField] private int completedObjectives = 0;
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
    public List<Objective> CurrentObjectives { get { return currentObjectives; } }

    private GameObject player;
    private ResourceManager resourceManager;
    private List<Objective> objectives = new List<Objective>();
    private List<Objective> currentObjectives = new List<Objective>();

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

        for (int i = 0; i < numOfObjectives; i++)
        {
            var objective = objectives[Random.Range(0, objectives.Count)];
            objective.Reset();
            PrepareObjective(objective);
            objective.OnObjectiveCompleted += CompleteObjective;
            currentObjectives.Add(objective);
            Debug.Log($"Objective added: {objective.Title}");
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
    }

    private void CompleteObjective(IObjective objective)
    {
        foreach(var awardResource in objective.RewardResources)
        { 
            resourceManager.ChangeResourceAmount(awardResource.amount, awardResource.resourceType);
        }
        completedObjectives++;
        objective.OnObjectiveCompleted -= CompleteObjective;

        if (objective.IsCompleted)
        {
            Debug.Log($"Objective Completed: {objective.Title}. {completedObjectives}/{numOfObjectives} done.");
        }
        else
        {
            Debug.Log($"Objective Failed: {objective.Title}. {completedObjectives}/{numOfObjectives} done.");
        }

        if (completedObjectives ==  numOfObjectives)
        {
            StartCoroutine(ObjectivesCoroutine());
        }
    }

    private void DestroyObjectives()
    {
        currentObjectives.Clear();
        completedObjectives = 0;
    }
}
