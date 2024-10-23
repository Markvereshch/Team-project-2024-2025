using UnityEngine;
using UnityEngine.Events;

public class GunEffectsScript : MonoBehaviour
{
    [SerializeField] ParticleSystem[] muzzleFireEffects;
    private GunBaseScript gunBaseScript;

    private void Start()
    {
        gunBaseScript = GetComponent<GunBaseScript>();
        gunBaseScript.onShoot.AddListener(InvokeMuzzleFireEffect);
    }
    public void InvokeMuzzleFireEffect()
    {
        Debug.Log("EFFECT MUST BE PLAYED!");
        if (muzzleFireEffects.Length > 0)
        {
            muzzleFireEffects[gunBaseScript.currentMuzzle].Play();
        }
    }
}
