using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Configuration", menuName = "Guns/WeaponConfig", order = 2)]
public class WeaponConfig : ScriptableObject
{
    public Vector3 spread = new Vector3(0.02f, 0.02f, 0.02f);
    public float recoilForceModifier = 100f;
    public float fireRate = 0.15f;
    public float range = 100f;
    public int bulletsInOneShoot = 1;
    public float turnSpeed = 100f;
    public float elevationSpeed = 16f;
    public GameObject droppedWeaponPrefab;
}
