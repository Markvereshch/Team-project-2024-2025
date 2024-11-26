using UnityEngine;
using UnityEngine.Events;

public class VehicleHealth : MonoBehaviour, IDamagable
{
    [SerializeField] private float currentHealth;
    [SerializeField] public AIVehicleConfig aiConfig;

    public Fraction Fraction { get; set; }
    public bool IsDead { get; private set; }
    public bool Invincible { get; set; }
    public UnityAction<float, GameObject> OnDamaged { get; set; }
    public UnityAction OnDie { get; set; }

    private AICarMovement aiCarMovement;
    private VehicleStats stats;

    private void Start()
    {
        stats = GetComponent<VehicleStats>();
        currentHealth =  stats.maxHealth;

        aiCarMovement = GetComponent<AICarMovement>();
    }

    public void TakeDamage(float damage, GameObject damageSource)
    {
        var sourceEntity = damageSource.GetComponentInParent<VehicleHealth>();
        if ((sourceEntity && IsFriend(sourceEntity.Fraction)) || Invincible)
            return;

        float healthBefore = currentHealth;
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, stats.maxHealth);

        float trueDamageAmount = healthBefore - currentHealth;
        if (trueDamageAmount > 0f)
        {
            OnDamaged?.Invoke(trueDamageAmount, damageSource);
        }
        if (sourceEntity != null)
            SetTarget(sourceEntity.gameObject);
        HandleDeath();
    }

    private void SetTarget(GameObject source)
    {
        if (aiCarMovement != null)
        {
            aiCarMovement.SetTarget(source.transform.position);
        }
    }

    public void Kill()
    {
        currentHealth = 0f;

        OnDamaged?.Invoke(stats.maxHealth, null);

        HandleDeath();
    }

    private bool IsFriend(Fraction attackersFraction) => attackersFraction == Fraction;

    private void HandleDeath()
    {
        if (IsDead)
            return;

        if (currentHealth <= 0f)
        {
            IsDead = true;
            OnDie?.Invoke();
        }
    }
}
