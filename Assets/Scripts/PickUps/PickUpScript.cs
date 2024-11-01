using UnityEngine;

public class PickUpScript : MonoBehaviour
{
    [SerializeField] GameObject weaponPrefab;
    PickUpManager manager;

    private void Start()
    {
        manager = FindObjectOfType<PickUpManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var car = other.GetComponentInParent<CarControl>();
        if (car)
        {
            manager.Player = car.gameObject;
            manager.AddPickableItems(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.GetComponentInParent<CarControl>() != null)
        {
            manager.RemovePickableItems(this);
        }
    }

    public GameObject WeaponPrefab
    {
        get { return weaponPrefab; }
    }
}
