using UnityEngine;

[CreateAssetMenu(fileName = "Reloading Configuration", menuName = "Guns/ReloadingConfig", order = 4)]
public class ReloadingConfig : ScriptableObject
{
    public float reloadTime = 5f;
    public float waitForAutoreloadingStart = 1f;
    public int bulletsToAddDuringAutoreloading = 1;
    public int maxAmmo = 500;
    public int clipSize = 50;
}
