using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] DeathMenu deathMenu;

    public bool IsGamePaused { get; set; }
    public bool IsGameOver { get; set; }

    public GameObject Player
    { 
        get
        {
            return player;
        }
        set 
        {
            player = value;
            vehicleHealth = player.GetComponent<VehicleHealth>();
            resourceManager = player.GetComponent<ResourceManager>();
            vehicleHealth.OnDie += HandleDefeat;
            ObjectiveManager.Instance.Player = player;
            InGameUIManager.Instance.Player = player;
            InventoryUIManager.Instance.ResourceManager = resourceManager;
        } 
    }
    private GameObject player;
    private ResourceManager resourceManager;
    private VehicleHealth vehicleHealth;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(gameObject);
    }

    private void HandleDefeat()
    {
        IsGameOver = true;
        deathMenu.SetDefeatImage(GetCauseOfDeath());
        StartCoroutine(deathMenu.DefeatCoroutine());
    }

    private DeathCause GetCauseOfDeath()
    {
        if (vehicleHealth.LastDamageSource == null)
            return DeathCause.Unknown;
        else if (vehicleHealth.LastDamageSource.GetComponentInParent<VehicleHealth>())
            return DeathCause.KIA;
        else if (vehicleHealth.LastDamageSource.GetComponent<DamageZone>())
            return DeathCause.Radiation;
        return DeathCause.NoGasoline;
    }

    public void HandleEvacuation()
    {
        SaveCollectedResources();
        SceneManager.LoadScene("Hangar");
    }

    private void SaveCollectedResources()
    {
        var saveData = GameSaver.Load();
        var resources = saveData.resourcesSaveData;
        resourceManager.PrepareSaveData(resources);
        GameSaver.Save(saveData);
    }
}
