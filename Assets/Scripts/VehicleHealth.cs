using UnityEngine;
using UnityEngine.Events;

public class VehicleHealth : MonoBehaviour, IDamagable
{
    [SerializeField] private float health;
    [SerializeField] private VehicleConfig config;
    [SerializeField] public AIVehicleConfig aiConfig;

    public Fraction Fraction { get; set; }
    public bool IsDead { get; private set; }
    public bool Invincible { get; set; }
    public UnityAction<float, GameObject> OnDamaged { get; set; }
    public UnityAction OnDie { get; set; }

    private float maxHealth;
    private AICarMovement aiCarMovement;

    private void Start()
    {
        health = maxHealth = config.health;
        aiCarMovement = GetComponent<AICarMovement>();
        FetchHealthBonus();
    }

    public void FetchHealthBonus()
    {
        if (TryGetComponent<UpgradeManager>(out var updateManager))
        {
            health = maxHealth += updateManager.HealthBonus;
        }
    }

    public void TakeDamage(float damage, GameObject damageSource)
    {
        var sourceEntity = damageSource.GetComponentInParent<VehicleHealth>();
        if ((sourceEntity && IsFriend(sourceEntity.Fraction)) || Invincible)
            return;

        float healthBefore = health;
        health -= damage;
        health = Mathf.Clamp(health, 0f, maxHealth);

        float trueDamageAmount = healthBefore - health;
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
        health = 0f;

        OnDamaged?.Invoke(maxHealth, null);

        HandleDeath();
    }

    private bool IsFriend(Fraction attackersFraction) => attackersFraction == Fraction;

    private void HandleDeath()
    {
        if (IsDead)
            return;

        if (health <= 0f)
        {
            IsDead = true;
            OnDie?.Invoke();
        }
    }
}
