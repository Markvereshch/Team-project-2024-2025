using UnityEngine;
using UnityEngine.Events;

public class VehicleHealth : MonoBehaviour, IDamagable
{
    [SerializeField] public AIVehicleConfig aiConfig;

    public float CurrentHealth { get; private set; }
    public bool IsDead { get; private set; }
    public bool Invincible { get; set; }
    public UnityAction<float, GameObject> OnDamaged { get; set; }
    public UnityAction OnDie { get; set; }
    public UnityAction<VehicleHealth> OnKilled { get; set; }
    public VehicleStats Stats { get; private set; }

    public GameObject LastDamageSource { get; private set; }

    public Fraction Fraction 
    { 
        get { return fraction; }
        set 
        { 
            fraction = value;

            if (mapObject == null)
                mapObject = GetComponentInChildren<MinimapObject>();

            if (fraction == Fraction.Ally)
                mapObject.Color = InGameUIManager.Instance.AllyColor;
            else
                mapObject.Color = InGameUIManager.Instance.EnemyColor;
        }
    }
    private Fraction fraction;
    private MinimapObject mapObject;
    private AICarMovement aiCarMovement;

    private void Start()
    {
        if (mapObject == null)
            mapObject = GetComponentInChildren<MinimapObject>();

        Stats = GetComponent<VehicleStats>();
        CurrentHealth =  Stats.maxHealth;
        aiCarMovement = GetComponent<AICarMovement>();
    }

    public void TakeDamage(float damage, GameObject damageSource)
    {
        LastDamageSource = damageSource;

        var sourceEntity = damageSource.GetComponentInParent<VehicleHealth>();
        if ((sourceEntity && IsFriend(sourceEntity.Fraction)) || Invincible)
            return;

        float healthBefore = CurrentHealth;
        CurrentHealth -= damage;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, Stats.maxHealth);

        float trueDamageAmount = healthBefore - CurrentHealth;

        if (trueDamageAmount > 0f)
        {
            OnDamaged?.Invoke(trueDamageAmount, damageSource);
        }

        if (sourceEntity != null)
            SetTarget(sourceEntity.gameObject);

        HandleDeath(sourceEntity);
    }

    public bool IsHalfDead()
    {
        return CurrentHealth * 2 > Stats.maxHealth;
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
        CurrentHealth = 0f;

        OnDamaged?.Invoke(Stats.maxHealth, null);

        HandleDeath(null);
    }

    private bool IsFriend(Fraction attackersFraction) => attackersFraction == Fraction;

    private void HandleDeath(VehicleHealth lastDamageSource)
    {
        if (IsDead)
            return;

        if (CurrentHealth <= 0f)
        {
            IsDead = true;
            OnDie?.Invoke();
        }

        if (IsDead && lastDamageSource)
        {
            Debug.Log(lastDamageSource);
            OnKilled?.Invoke(lastDamageSource);
        }
    }
}
