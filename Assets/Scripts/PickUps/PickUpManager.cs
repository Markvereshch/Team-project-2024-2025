using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PickUpManager : MonoBehaviour
{
    private Dictionary<int, PickUpScript> pickableItems;
    private GameObject player = null;
    private AudioSource cameraAudioSource;
    [SerializeField] private List<AudioClip> onWeaponSetSound = new List<AudioClip>();
    public UnityAction<GunBaseScript> OnWeaponEquipped { get; set; }
    
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
        cameraAudioSource = Camera.main.GetComponent<AudioSource>();
        pickableItems = new Dictionary<int, PickUpScript>();
    }

    public void FixedUpdate()
    {
        if (pickableItems.Count > 1)
        {
            var nearestPickUp = FindNearestObject();
            InGameUIManager.Instance.SetInfoIcon(nearestPickUp.Icon, IconType.PickableObject);
        }
    }

    public void AddPickableItems(PickUpScript pickUp)
    {
        if (pickableItems.TryAdd(pickUp.GetInstanceID(), pickUp))
        {
            var nearestPickUp = FindNearestObject();
            InGameUIManager.Instance.SetInfoIcon(nearestPickUp.Icon, IconType.PickableObject);
        }
    }

    public void RemovePickableItems(PickUpScript pickUp)
    {
        if (pickableItems.ContainsKey(pickUp.GetInstanceID()))
        {
            pickableItems.Remove(pickUp.GetInstanceID());

            if (pickableItems.Count == 0) 
            {
                InGameUIManager.Instance.SetInfoIcon(null, IconType.PickableObject);
            }
            else
            {
                var nearestPickUp = FindNearestObject();
                InGameUIManager.Instance.SetInfoIcon(nearestPickUp.Icon, IconType.PickableObject);
            }
        }
    }

    public void PickUpTurret()
    {
        if (player != null && pickableItems.Count > 0)
        {
            PickUpScript nearestObject = FindNearestObject();
            RemovePickableItems(nearestObject);
            var weaponScript = InstantiateWeapon(nearestObject.WeaponPrefab);
            OnWeaponEquipped?.Invoke(weaponScript);
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

    private GunBaseScript InstantiateWeapon(GameObject weaponPrefab)
    {
        GunPlaceScript gunPlace = player.GetComponentInChildren<GunPlaceScript>();
        TurretControl installedWeapon = gunPlace.GetComponentInChildren<TurretControl>();
        if (installedWeapon)
        {
            DropOldWeapon(installedWeapon.gameObject);
            Destroy(installedWeapon.gameObject);
        }

        GameObject newWeapon = Instantiate(weaponPrefab, gunPlace.transform.position, gunPlace.transform.rotation);
        if (onWeaponSetSound.Count > 0)
        {
            int index = Random.Range(0, onWeaponSetSound.Count);
            cameraAudioSource.PlayOneShot(onWeaponSetSound[index]);
        }
        newWeapon.transform.parent = gunPlace.transform;

        GunBaseScript weaponScript = newWeapon.GetComponent<GunBaseScript>();
        ResourceManager resourceManager = player.GetComponent<ResourceManager>();
        resourceManager.WeaponToDrop = weaponPrefab.GetComponent<GunBaseScript>().weaponConfig.droppedWeaponPrefab;
        weaponScript.resourceManager = resourceManager;

        return weaponScript;
    }

    private void DropOldWeapon(GameObject oldWeapon)
    {
        var shootingScript = oldWeapon.GetComponent<GunBaseScript>();
        if (shootingScript)
        {
            Instantiate(shootingScript.weaponConfig.droppedWeaponPrefab, new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z), player.transform.rotation);
        }
    }
}
