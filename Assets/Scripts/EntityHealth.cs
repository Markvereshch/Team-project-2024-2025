using UnityEngine;
using UnityEngine.Events;

public class EntityHealth : MonoBehaviour, IDamagable
{
    [SerializeField] private float health;
    [SerializeField] private EntityConfig config;
    public Fraction fraction;

    public bool IsDead { get; private set; }
    public GameObject agentAI { get; private set; }
    public bool Invincible { get; set; }
    public UnityAction<float, GameObject> OnDamaged { get; set; }
    public UnityAction OnDie { get; set; }

    private float maxHealth;

    private void Start()
    {
        agentAI = config.agentAI;
        health = maxHealth = config.health;
    }

    public void TakeDamage(float damage, GameObject damageSource)
    {
        var sourceEntity = damageSource.GetComponentInParent<EntityHealth>();
        if ((sourceEntity && IsFriend(sourceEntity.fraction)) || Invincible)
            return;

        float healthBefore = health;
        health -= damage;
        health = Mathf.Clamp(health, 0f, maxHealth);

        float trueDamageAmount = healthBefore - health;
        if (trueDamageAmount > 0f)
        {
            OnDamaged?.Invoke(trueDamageAmount, damageSource);
        }

        HandleDeath();
    }

    public void Kill()
    {
        health = 0f;

        OnDamaged?.Invoke(maxHealth, null);

        HandleDeath();
    }

    private bool IsFriend(Fraction attackersFraction) => attackersFraction == fraction;

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
