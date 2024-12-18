using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ReloadScript : MonoBehaviour, IReloadable
{
    public UnityAction OnReloadEnd { get; set; }
    private AudioSource audioSource;
    private GunBaseScript gunBaseScript;
    public bool isReloadRequested;
    
    private void Start()
    {
        gunBaseScript = GetComponent<GunBaseScript>();
        audioSource = GetComponent<AudioSource>();

        var controller = gameObject.GetComponentInParent<IVehicleController>();
        if (controller != null)
        {
            controller.ManualReloading = this;
        }
    }

    public bool IsAbleToReload()
    {
        return 
            ((gunBaseScript.currentClip < gunBaseScript.reloadConfig.clipSize && isReloadRequested) || gunBaseScript.currentClip == 0)
            && gunBaseScript.isReloading == false
            && gunBaseScript.resourceManager.GetResourceAmount(gunBaseScript.weaponConfig.shootableResource) > 0;
    }

    public IEnumerator PerformReloading()
    {
        gunBaseScript.audioConfig.PlayStartReloadClip(audioSource);
        gunBaseScript.isReloading = true;
        yield return new WaitForSeconds(gunBaseScript.reloadConfig.reloadTime);
        Reload();
        gunBaseScript.audioConfig.PlayStopReloadClip(audioSource);
        if (gunBaseScript.audioConfig.reloadStopClip != null)
            yield return new WaitForSeconds(gunBaseScript.audioConfig.reloadStopClip.length/2);
        gunBaseScript.isReloading = false;
    }

    private void Reload()
    {
        int bulletsToRefill = gunBaseScript.reloadConfig.clipSize - gunBaseScript.currentClip;
        bulletsToRefill = (gunBaseScript.resourceManager.GetResourceAmount(gunBaseScript.weaponConfig.shootableResource) - bulletsToRefill) >= 0 ? bulletsToRefill : gunBaseScript.resourceManager.GetResourceAmount(gunBaseScript.weaponConfig.shootableResource);
        gunBaseScript.currentClip += bulletsToRefill;
        gunBaseScript.resourceManager.ChangeResourceAmount(-bulletsToRefill, gunBaseScript.weaponConfig.shootableResource);
        OnReloadEnd?.Invoke();
    }
}
