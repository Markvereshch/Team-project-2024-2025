using UnityEngine;

public class SelectedVehicle : MonoBehaviour
{
    public static SelectedVehicle Instance { get; private set; }

    public GameObject SelectedVehiclePrefab { get; set; }
    public VehicleData SelectedVehicleData { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
