using UnityEngine;

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
        if (muzzleFireEffects.Length > 0)
        {
            muzzleFireEffects[gunBaseScript.currentMuzzle].Play();
        }
    }
}
