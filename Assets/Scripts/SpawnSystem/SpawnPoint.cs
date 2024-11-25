using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [Header("Car prefabs")]
    [SerializeField] private List<GameObject> easyVehicles = new List<GameObject>();
    [SerializeField] private List<GameObject> mediumVehicles = new List<GameObject>();
    [SerializeField] private List<GameObject> hardVehicles = new List<GameObject>();

    [Header("Weapon lists")]
    [SerializeField] private List<GameObject> easyWeapons = new List<GameObject>();
    [SerializeField] private List<GameObject> mediumWeapons = new List<GameObject>();
    [SerializeField] private List<GameObject> hardWeapons = new List<GameObject>();

    [Header("Spawn options")]
    [SerializeField] private byte minEnemiesToSpawn = 1;
    [SerializeField] private byte maxEnemiesToSpawn = 3;
    [SerializeField] private float spawnHeight = 3f;

    (ResourceType resource, int minAmount, int maxAmount)[] easyEnemyDrop = { 
        (ResourceType.Wood, 1, 10), 
        (ResourceType.Scrap, 1, 8), 
        (ResourceType.Coins, 1, 12)
    };

    public SpawnPointDifficulty difficulty;
    private SpawnManager spawnManager;
    private float sphereColliderRadius;
    private int amountOfAmmo = 500;
    private int radiusDivisor = 2;
    private GameObject player;
    

    private void Start()
    {
        sphereColliderRadius = GetComponent<SphereCollider>().radius;
        spawnManager = FindObjectOfType<SpawnManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        var isPlayer = other.GetComponentInParent<PlayerInputController>();
        if (isPlayer && spawnManager.CanSpawn())
        {
            player = isPlayer.gameObject;
            spawnManager.SetLastSpawnTime();
            SpawnEnemies();
        }
    }

    private void SpawnEnemies()
    {
        switch(difficulty)
        {
            case SpawnPointDifficulty.Easy:
                SpawnEnemies(easyVehicles, easyWeapons);
                break;
            case SpawnPointDifficulty.Medium:
                SpawnEnemies(mediumVehicles, mediumWeapons);
                break;
            case SpawnPointDifficulty.Hard:
                SpawnEnemies(hardVehicles, hardWeapons);
                break;
        }
    }

    private void SpawnEnemies(List<GameObject> vehicles, List<GameObject> weapons)
    {
        if (vehicles.Count == 0)
        {
            throw new UnityException("There are no prefabs in the easy vehicles list");
        }

        int enemiesToSpawn = Random.Range(minEnemiesToSpawn, maxEnemiesToSpawn);
        int index;

        GameObject enemyGroupObject = new GameObject("EnemyGroup#" + System.Guid.NewGuid());
        EnemyGroup enemyGroup = enemyGroupObject.AddComponent<EnemyGroup>();
        enemyGroup.SetPlayer(player);

        for (int i = 0; i <  enemiesToSpawn; i++)
        {
            index = Random.Range(0, vehicles.Count);
            Vector3 spawnPosition = new Vector3(
                transform.position.x + Random.Range(sphereColliderRadius/radiusDivisor, sphereColliderRadius/radiusDivisor), 
                transform.position.y + 2f, 
                transform.position.z + Random.Range(-sphereColliderRadius/radiusDivisor, sphereColliderRadius/radiusDivisor));

            Vector3 direction = player.transform.position - spawnPosition;
            Quaternion rotation = Quaternion.LookRotation(direction);
            var instantiated = Instantiate(vehicles[index], spawnPosition, rotation);

            SpawnWeapon(weapons, instantiated);
            CarInitializer initialized = instantiated.GetComponent<CarInitializer>();
            initialized.SetCarPlayability(false);

            VehicleHealth eh = instantiated.GetComponent<VehicleHealth>();
            eh.Fraction = Fraction.Enemy;

            AICarMovement ai = instantiated.GetComponent<AICarMovement>();
            ai.Activate(player.transform.position);

            enemyGroup.AddEnemy(instantiated);
        }
        spawnManager.AddEnemyGroup(enemyGroup);
    }

    private void SpawnWeapon(List<GameObject> weaponList, GameObject vehicle)
    {
        if (weaponList.Count == 0)
        {
            throw new UnityException("There are no prefabs in the weaponList");
        }

        var gunPlace = vehicle.GetComponentInChildren<GunPlaceScript>();
        if (gunPlace != null) 
        {
            int index = Random.Range(0, weaponList.Count);
            var newWeapon = Instantiate(weaponList[index], gunPlace.transform.position, gunPlace.transform.rotation);
            newWeapon.transform.parent = gunPlace.transform;

            GunBaseScript weaponScript = newWeapon.GetComponent<GunBaseScript>();
            weaponScript.currentClip = weaponScript.reloadConfig.clipSize;
            ResourceManager resourceManager = vehicle.GetComponent<ResourceManager>();
            if (weaponScript)
            {
                weaponScript.resourceManager = resourceManager;
            }
            resourceManager.WeaponToDrop = weaponScript.weaponConfig.droppedWeaponPrefab;
            var shootableResource = weaponScript.weaponConfig.shootableResource;
            weaponScript.resourceManager.ChangeResourceAmount(amountOfAmmo, shootableResource);
            AddResourcesToInventory(resourceManager, easyEnemyDrop);
        }
    }

    private void AddResourcesToInventory(ResourceManager resourceManager, (ResourceType resource, int minAmount, int maxAmount)[] possibleDrop)
    {
        for (int i = 0; i < possibleDrop.Length; i++) 
        {
            int index = Random.Range(0, possibleDrop.Length);
            ResourceType typeToAdd = possibleDrop[index].resource;
            int amountToAdd = Random.Range(possibleDrop[index].minAmount, possibleDrop[index].maxAmount);
            resourceManager.ChangeResourceAmount(amountToAdd, typeToAdd);
        }
    }
}

public enum SpawnPointDifficulty
{
    Easy,
    Medium,
    Hard,
}
