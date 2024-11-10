using UnityEngine;
using UnityEngine.Events;

public class EntityHealth : MonoBehaviour, IDamagable
{
    [SerializeField] private float health;
    [SerializeField] private EntityConfig config;
    public Fraction Fraction { get; set; }
    public bool IsDead { get; private set; }
    public GameObject AgentAI { get; private set; }
    public bool Invincible { get; set; }
    public UnityAction<float, GameObject> OnDamaged { get; set; }
    public UnityAction OnDie { get; set; }

    private float maxHealth;
    private AICarMovement aiCarMovement;

    private void Awake()
    {
        AgentAI = config.agentAI;
        health = maxHealth = config.health;
    }

    private void Start()
    {
        aiCarMovement = GetComponent<AICarMovement>();
    }

    public void TakeDamage(float damage, GameObject damageSource)
    {
        var sourceEntity = damageSource.GetComponentInParent<EntityHealth>();
        if ((sourceEntity && IsFriend(sourceEntity.Fraction)) || Invincible || !sourceEntity)
            return;

        float healthBefore = health;
        health -= damage;
        health = Mathf.Clamp(health, 0f, maxHealth);

        float trueDamageAmount = healthBefore - health;
        if (trueDamageAmount > 0f)
        {
            OnDamaged?.Invoke(trueDamageAmount, damageSource);
        }
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
