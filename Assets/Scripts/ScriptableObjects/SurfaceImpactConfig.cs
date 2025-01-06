using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SurfaceImpact", menuName = "Impact/SurfaceImpact")]
public class SurfaceImpactConfig : ScriptableObject
{
    public GameObject impactEffect;
    public List<AudioClip> impactSounds = new List<AudioClip>();
    public List<AudioClip> onCollisionSound = new List<AudioClip>();

    public AudioClip GetRandomImpactSound()
    {
        int index = Random.Range(0, impactSounds.Count);
        return impactSounds[index];
    }

    public AudioClip GetRandomCollisionSound()
    {
        int index = Random.Range(0, onCollisionSound.Count);
        return onCollisionSound[index];
    }
}