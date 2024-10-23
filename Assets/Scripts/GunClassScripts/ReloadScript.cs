using System.Collections;
using UnityEngine;

public class ReloadScript : MonoBehaviour, IReloadable
{
    private AudioSource audioSource;
    private GunBaseScript gunBaseScript;
    private KeyCode reloadKey = KeyCode.R;
    
    private void Start()
    {
        gunBaseScript = GetComponent<GunBaseScript>();
        audioSource = GetComponent<AudioSource>();
    }

    public bool IsAbleToReload()
    {
        return ((gunBaseScript.currentClip < gunBaseScript.reloadConfig.clipSize && gunBaseScript.ammoManager.currentAmmo > 0 && Input.GetKeyDown(reloadKey)) 
            || gunBaseScript.currentClip == 0) && gunBaseScript.isReloading == false;
    }

    public IEnumerator PerformReloading()
    {
        gunBaseScript.audioConfig.PlayStartReloadClip(audioSource);
        gunBaseScript.isReloading = true;
        yield return new WaitForSeconds(gunBaseScript.reloadConfig.reloadTime);
        Reload();
        gunBaseScript.isReloading = false;
        gunBaseScript.audioConfig.PlayStopReloadClip(audioSource);
    }

    private void Reload()
    {
        int bulletsToRefill = gunBaseScript.reloadConfig.clipSize - gunBaseScript.currentClip;
        bulletsToRefill = (gunBaseScript.ammoManager.currentAmmo - bulletsToRefill) >= 0 ? bulletsToRefill : gunBaseScript.ammoManager.currentAmmo;
        gunBaseScript.currentClip += bulletsToRefill;
        gunBaseScript.ammoManager.currentAmmo -= bulletsToRefill;
    }
}