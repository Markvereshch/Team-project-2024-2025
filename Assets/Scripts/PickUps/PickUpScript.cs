using UnityEngine;

public class PickUpScript : MonoBehaviour
{
    [SerializeField] GameObject weaponPrefab;

    private void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                var gunPlace = player.GetComponentInChildren<GunPlaceScript>();
                var oldTurret = player.GetComponentInChildren<GunBaseScript>();

                if (oldTurret != null)
                {
                    Destroy(oldTurret.gameObject);
                }

                Instantiate(weaponPrefab, gunPlace.transform.position, gunPlace.transform.rotation, gunPlace.transform);

                Destroy(gameObject);

                Debug.Log("Turret picked up and attached.");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
    }
}
