using UnityEngine;

public class ResourcePickUpScript : MonoBehaviour
{
    [SerializeField] private GunType ammoType;
    [SerializeField] private int ammoToAdd = 5;

    private void OnTriggerEnter(Collider other)
    {
        var car = other.GetComponentInParent<CarControl>();
        if (car)
        {
            var ammoManager = car.gameObject.GetComponent<AmmoManager>();
            ammoManager.ChangeAmmo(ammoToAdd, ammoType);
            Destroy(gameObject);
        }
    }
}
