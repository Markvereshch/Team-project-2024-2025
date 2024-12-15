using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInitializer : MonoBehaviour
{
    [SerializeField] private List<Transform> possibleSpawnPoints = new List<Transform>();
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private GameObject defaultPrefab;
    [SerializeField] private GameManager gameManager;

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
        SetCamera(player);
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

    private void SetCamera(GameObject target)
    {
        virtualCamera.LookAt = target.transform;
        virtualCamera.Follow = target.transform;
    }
}
