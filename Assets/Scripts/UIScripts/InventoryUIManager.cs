using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryUIManager : MonoBehaviour
{
    [Header("Inventory panel")]
    [SerializeField] private GameObject inventoryPanel;
    [Header("Resources UI")]
    [SerializeField] private TMP_Text shotgunAmmoText;
    [SerializeField] private TMP_Text machineGunAmmoText;
    [SerializeField] private TMP_Text explosiveAmmoText;
    [SerializeField] private TMP_Text woodText;
    [SerializeField] private TMP_Text scrapText;
    [SerializeField] private TMP_Text electronicsText;
    [SerializeField] private TMP_Text gasolineText;
    [SerializeField] private TMP_Text coinsText;
    [Header("Objectives UI List")]
    [SerializeField] private Transform taskListContent;
    [SerializeField] private GameObject taskPrefab;
    private List<ObjectiveUIElement> activeTasks = new List<ObjectiveUIElement>();
    public bool IsInventoryOpened { get; set; }
    private Map mapManager;

    public ObjectiveUIElement AddObjectiveToScrolableList(Sprite taskIcon, string taskName, string taskDescription, bool hasTimer = false, float timerValue = 0f)
    {
        GameObject newTaskObject = Instantiate(taskPrefab, taskListContent);
        ObjectiveUIElement newTask = newTaskObject.GetComponent<ObjectiveUIElement>();
        newTask.SetupTask(taskIcon, taskName, taskDescription, hasTimer, timerValue);
        activeTasks.Add(newTask);

        return newTask;
    }

    public void Start()
    {
        mapManager = GetComponent<Map>();   
    }

    public void OpenInventory()
    {
        IsInventoryOpened = !IsInventoryOpened;
        GameManager.Instance.IsGamePaused = !GameManager.Instance.IsGamePaused;

        if (IsInventoryOpened)
        {
            UpdateResourceUI();
            InventoryPanel.SetActive(true);
            mapManager.LoadMap();
            Time.timeScale = 0f;
        }
        else
        {
            InventoryPanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    public void RemoveOldObjectives()
    {
        for (int i = 0; i < activeTasks.Count; i++)
        {
            Destroy(activeTasks[i].gameObject);
        }
        activeTasks.Clear();
    }

    public void UpdateObjectiveTimer(int taskIndex, float timerValue)
    {
        if (taskIndex >= 0 && taskIndex < activeTasks.Count)
        {
            activeTasks[taskIndex].UpdateTimer(timerValue);
        }
    }

    public void SetObjectiveCompletionStatus(int taskIndex, bool isCompleted)
    {
        if (taskIndex >= 0 && taskIndex < activeTasks.Count)
        {
            activeTasks[taskIndex].SetStatus(isCompleted);
        }
    }

    public GameObject InventoryPanel 
    { 
        get { return inventoryPanel;}
        set { inventoryPanel = value; } 
    }

    public ResourceManager ResourceManager { get; set; }

    public static InventoryUIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(gameObject);
    }

    public void UpdateResourceUI()
    {
        shotgunAmmoText.text = $"{ResourceManager.GetResourceAmount(ResourceType.ShotgunAmmo)}/{ResourceManager.GetMaxResourceAmount(ResourceType.ShotgunAmmo)}";
        machineGunAmmoText.text = $"{ResourceManager.GetResourceAmount(ResourceType.MachineGunAmmo)}/{ResourceManager.GetMaxResourceAmount(ResourceType.MachineGunAmmo)}";
        explosiveAmmoText.text = $"{ResourceManager.GetResourceAmount(ResourceType.ExplosiveAmmo)}/{ResourceManager.GetMaxResourceAmount(ResourceType.ExplosiveAmmo)}";
        woodText.text = $"{ResourceManager.GetResourceAmount(ResourceType.Wood)}/{ResourceManager.GetMaxResourceAmount(ResourceType.Wood)}";
        scrapText.text = $"{ResourceManager.GetResourceAmount(ResourceType.Scrap)}/{ResourceManager.GetMaxResourceAmount(ResourceType.Scrap)}";
        electronicsText.text = $"{ResourceManager.GetResourceAmount(ResourceType.Electronics)}/{ResourceManager.GetMaxResourceAmount(ResourceType.Electronics)}";
        gasolineText.text = $"{ResourceManager.GetResourceAmount(ResourceType.Gasoline)}/{ResourceManager.GetMaxResourceAmount(ResourceType.Gasoline)}";
        coinsText.text = $"{ResourceManager.GetResourceAmount(ResourceType.Coins)}/{ResourceManager.GetMaxResourceAmount(ResourceType.Coins)}";
    }
}
