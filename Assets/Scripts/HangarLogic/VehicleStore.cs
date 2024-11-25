using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VehicleStore", menuName = "Vehicles/Store")]
public class VehicleStore : ScriptableObject
{
    public List<VehiclePurchaseData> vehicles;

    public VehiclePurchaseData GetVehicleById(int id)
    {
        return vehicles.Find(vehicle => vehicle.Id == id);
    }
}

[System.Serializable]
public class VehiclePurchaseData
{
    public int Id;
    public string CarName;
    public GameObject Prefab;
    public uint BasePrice;
}
