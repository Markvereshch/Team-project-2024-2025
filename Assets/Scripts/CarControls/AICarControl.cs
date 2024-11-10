using UnityEngine;

public class AICarControl : MonoBehaviour, IVehicleController
{
    public AITargetSeeker Seeker { get; private set; }
    public bool IsTransport { get; set; }
    public IShootable Weapon { get; set; }
    public ReloadScript ManualReloading { get; set; }
    public TurretControl TurretControl { get; set; }
    public AICarMovement CarMovement { get; private set; }

    private ICarState currentState;
    private EntityHealth entityHealth;

    private void Start()
    {
        entityHealth = GetComponent<EntityHealth>();
        TurretControl = GetComponentInChildren<TurretControl>();
        Weapon = GetComponentInChildren<IShootable>();
        Seeker = GetComponent<AITargetSeeker>();
        CarMovement = GetComponent<AICarMovement>();

        if (IsTransport)
        {
            currentState = new TransportCarState();
            currentState.EnterState(this);
            (currentState as TransportCarState).SetInitialTarget(WaypointManager.GetNearestRoad(this));
        }
        else
        {
            currentState = new PatrolCarState();
            currentState.EnterState(this);
        }
    }

    private void Update()
    {
        if (entityHealth.IsDead)
        {
            return;
        }
        currentState.UpdateState();
    }

    public void SetState(ICarState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }
}
