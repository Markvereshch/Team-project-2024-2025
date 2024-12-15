using UnityEngine;

public class PickUpScript : MonoBehaviour
{
    [SerializeField] GameObject weaponPrefab;
    [SerializeField] Sprite icon;
    PickUpManager manager;
    public Sprite Icon { get { return icon; } }

    private void Start()
    {
        manager = FindObjectOfType<PickUpManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var car = other.GetComponentInParent<PlayerInputController>();
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
