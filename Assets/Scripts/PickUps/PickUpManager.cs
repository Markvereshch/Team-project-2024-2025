using System.Collections.Generic;
using UnityEngine;

public class PickUpManager : MonoBehaviour, IPickUpManager
{
    private Dictionary<int, PickUpScript> pickableItems;
    private GameObject player = null;
    
    public GameObject Player
    {
        get { return player; }
        set
        {
            if (player == null)
            {
                player = value;
            }
        }
    }

    private void Start()
    {
        pickableItems = new Dictionary<int, PickUpScript>();
    }

    public void AddPickableItems(PickUpScript pickUp)
    {
        pickableItems.TryAdd(pickUp.GetInstanceID(), pickUp);
    }

    public void RemovePickableItems(PickUpScript pickUp)
    {
        if (pickableItems.ContainsKey(pickUp.GetInstanceID()))
        {
            pickableItems.Remove(pickUp.GetInstanceID());
        }
    }

    private void Update()
    {
        if (player != null && pickableItems.Count > 0 && Input.GetKeyDown(KeyCode.F))
        {
            PickUpScript nearestObject = FindNearestObject();
            RemovePickableItems(nearestObject);
            InstantiateWeapon(nearestObject.WeaponPrefab);
            Destroy(nearestObject.gameObject);
        }
    }

    private PickUpScript FindNearestObject()
    {
        PickUpScript nearest = null;
        float nearestDistance = Mathf.Infinity;
        Vector3 playerPosition = player.transform.position;

        foreach (KeyValuePair<int, PickUpScript> obj in pickableItems)
        {
            float distanceToPlayer = Vector3.Distance(obj.Value.transform.position, playerPosition);

            if (distanceToPlayer < nearestDistance)
            {
                nearestDistance = distanceToPlayer;
                nearest = obj.Value;
            }
        }

        return nearest;
    }

    private void InstantiateWeapon(GameObject weaponPrefab)
    {
        GunPlaceScript gunPlace = player.GetComponentInChildren<GunPlaceScript>();
        TurretControl installedWeapon = gunPlace.GetComponentInChildren<TurretControl>();
        if (installedWeapon)
        {
            DropOldWeapon(installedWeapon.gameObject);
            Destroy(installedWeapon.gameObject);
        }

        GameObject newWeapon = Instantiate(weaponPrefab, gunPlace.transform.position, gunPlace.transform.rotation);
        newWeapon.transform.parent = gunPlace.transform;

        GunBaseScript weaponScript = newWeapon.GetComponent<GunBaseScript>();
        if (weaponScript)
        {
            AmmoManager ammoManager = FindAnyObjectByType<AmmoManager>(); 
            weaponScript.ammoManager = ammoManager;
        }
    }

    private void DropOldWeapon(GameObject oldWeapon)
    {
        var shootingScript = oldWeapon.GetComponent<GunBaseScript>();
        if (shootingScript)
        {
            Instantiate(shootingScript.weaponConfig.droppedWeaponPrefab, new Vector3(player.transform.position.x, player.transform.position.y + 5f, player.transform.position.z), player.transform.rotation);
        }
    }
}
