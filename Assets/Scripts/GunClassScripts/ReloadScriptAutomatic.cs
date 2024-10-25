using System.Collections;
using UnityEngine;

public class ReloadScriptAutomatic : MonoBehaviour, IReloadable
{
    private AudioSource audioSource;
    private GunBaseScript gunBaseScript;
    public bool onReload = false;

    private void Start()
    {
        gunBaseScript = GetComponent<GunBaseScript>();
        audioSource = GetComponent<AudioSource>();
    }

    public IEnumerator PerformReloading()
    {
        onReload = true;
        while (CheckLoadConditions())
        {
            gunBaseScript.isReloading = true;
            if (Time.time <= gunBaseScript.lastShootTime + gunBaseScript.reloadConfig.waitForAutoreloadingStart - gunBaseScript.reloadConfig.reloadTime 
                || gunBaseScript.currentClip >= gunBaseScript.reloadConfig.clipSize)
            {
                break;
            }    
            gunBaseScript.audioConfig.PlayStartReloadClip(audioSource);
            yield return new WaitForSeconds(gunBaseScript.reloadConfig.waitForAutoreloadingStart);
            Reload();
            gunBaseScript.isReloading = false;
            if (gunBaseScript.reloadConfig.clipSize == gunBaseScript.currentClip)
            {
                gunBaseScript.audioConfig.PlayStopReloadClip(audioSource);
            }
            yield return new WaitForSeconds(gunBaseScript.reloadConfig.reloadTime);
        }
        gunBaseScript.isReloading = false;
        onReload = false;
    }

    public bool IsAbleToReload()
    {
        return (Time.time >= gunBaseScript.lastShootTime + gunBaseScript.reloadConfig.waitForAutoreloadingStart 
            && gunBaseScript.currentClip < gunBaseScript.reloadConfig.clipSize && onReload == false);
    }

    private void Reload()
    {
        int bulletsToRefill = gunBaseScript.reloadConfig.clipSize - gunBaseScript.currentClip;
        int bulletNumber = bulletsToRefill > gunBaseScript.reloadConfig.bulletsToAddDuringAutoreloading ? gunBaseScript.reloadConfig.bulletsToAddDuringAutoreloading : bulletsToRefill;

        bulletNumber = (gunBaseScript.resourceManager.GetResourceAmount(gunBaseScript.weaponConfig.shootableResource) - bulletNumber) >= 0 ? bulletNumber : gunBaseScript.resourceManager.GetResourceAmount(gunBaseScript.weaponConfig.shootableResource);
        gunBaseScript.currentClip += bulletNumber;
        gunBaseScript.resourceManager.ChangeResourceAmount(-bulletNumber, gunBaseScript.weaponConfig.shootableResource);
    }

    private bool CheckLoadConditions()
    {
        return (gunBaseScript.currentClip < gunBaseScript.reloadConfig.clipSize) && (gunBaseScript.resourceManager.GetResourceAmount(gunBaseScript.weaponConfig.shootableResource) > 0); 
    }

}
