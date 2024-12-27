using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInitializer : MonoBehaviour
{
    [SerializeField] private List<Transform> possibleSpawnPoints = new List<Transform>();
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Camera radarCamera;
    [SerializeField] private GameObject defaultPrefab;
    [SerializeField] private GameManager gameManager;

    [SerializeField] private Sprite playerIcon;
    [SerializeField] private float radarCameraDistance = 40f;

    private void Start()
    {
        GameObject player;

        if (!SelectedVehicle.Instance || !SelectedVehicle.Instance.SelectedVehiclePrefab)
        {
            player = SpawnVehicle(defaultPrefab, null);
        }
        else
        {
            var selectedVehicle = SelectedVehicle.Instance.SelectedVehiclePrefab;
            var selectedVehicleUpgrades = SelectedVehicle.Instance.SelectedVehicleData;
            player = SpawnVehicle(selectedVehicle, selectedVehicleUpgrades);
        }
        SetMainCamera(player);
        SetRadarCamera(player);
        InstantiateMinimapObject(player);
        gameManager.Player = player;
    }

    private GameObject SpawnVehicle(GameObject instance, VehicleData vehicleData)
    {
        int index = Random.Range(0, possibleSpawnPoints.Count);
        var spawnPoint = possibleSpawnPoints[index];
        var instantiated = Instantiate(instance, spawnPoint.transform.position, spawnPoint.transform.rotation);

        CarInitializer initialized = instantiated.GetComponent<CarInitializer>();

        initialized.SetCarPlayability(true);

        VehicleHealth eh = instantiated.GetComponent<VehicleHealth>();
        eh.Fraction = Fraction.Ally;
      
        if (vehicleData != null && initialized.TryGetComponent<UpgradeManager>(out var upgradeManager))
        {
            UpgradeInfo upgradeInfo = vehicleData.ToUpgradeInfo();
            upgradeManager.UpgradeInfo = upgradeInfo;
        }

        return instantiated;
    }

    private void SetMainCamera(GameObject target)
    {
        virtualCamera.LookAt = target.transform;
        virtualCamera.Follow = target.transform;
    }

    private void SetRadarCamera(GameObject target)
    {
        radarCamera.transform.SetParent(null);
        Vector3 newPosition = target.transform.position + Vector3.up * radarCameraDistance;
        radarCamera.transform.position = newPosition;
        radarCamera.transform.rotation = Quaternion.Euler(90f, 0f, 180f);
        radarCamera.transform.SetParent(target.transform);
    }

    private void InstantiateMinimapObject(GameObject target)
    {
        var minimapObject = target.GetComponentInChildren<MinimapObject>();
        minimapObject.Sprite = playerIcon;
        minimapObject.Color = Color.yellow;
        minimapObject.IsFreezed = false;
    }
}
