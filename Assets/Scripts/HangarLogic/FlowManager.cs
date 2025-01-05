using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FlowManager : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string testSceneName;
    [SerializeField] private string menuSceneName;

    private HangarManager hangarManager;

    private void Awake()
    {
        hangarManager = GetComponent<HangarManager>();
        if (hangarManager == null)
        {
            Debug.LogError("HangarManager not found in scene.");
        }
    }

    public void StartRaid() // ѕока что кидает только на тестовый уровень
    {
        if (hangarManager == null || hangarManager.LastAvailableVehicle == null)
        {
            Debug.LogError("Cannot start raid: No vehicle selected.");
            return;
        }
        SelectedVehicle.Instance.SelectedVehiclePrefab = hangarManager.LastAvailableVehicle.Prefab;
        SelectedVehicle.Instance.SelectedVehicleData = hangarManager.AvailableVehicles.vehicles.Find(car => car.CarName == hangarManager.LastAvailableVehicle.CarName);

        SceneManager.LoadScene(testSceneName);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}
