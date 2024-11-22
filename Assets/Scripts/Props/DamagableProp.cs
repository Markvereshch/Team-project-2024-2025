using UnityEngine;
using UnityEngine.Events;

public class DamagableProp : MonoBehaviour, IDamagable
{
    [SerializeField] private float health;
    [SerializeField] private float maxHealth = 1;

    public bool IsDead { get; private set; }
    public bool Invincible { get; set; }
    public UnityAction<float, GameObject> OnDamaged { get; set; }
    public UnityAction OnDie { get; set; }

    private void Start()
    {
        health = maxHealth;    
    }

    public void TakeDamage(float damage, GameObject damageSource)
    {
        var sourceEntity = damageSource.GetComponentInParent<VehicleHealth>();
        if (Invincible || !sourceEntity)
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

    virtual protected void HandleDeath()
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
