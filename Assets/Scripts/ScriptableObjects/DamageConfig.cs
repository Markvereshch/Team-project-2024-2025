using UnityEngine;

[CreateAssetMenu(fileName = "Damage Configuration", menuName = "Guns/DamageConfig", order = 1)]
public class DamageConfig : ScriptableObject
{
    public ParticleSystem.MinMaxCurve damageDispersion;

    private void Reset()
    {
        damageDispersion.mode = ParticleSystemCurveMode.Curve;
    }

    public int GetDamage(float distance = 0)
    {
        return Mathf.CeilToInt(damageDispersion.Evaluate(distance, Random.value));
    }
}
