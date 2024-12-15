using UnityEngine;

public class AICarControl : MonoBehaviour, IVehicleController
{
    public AITargetSeeker Seeker { get; private set; }
    public bool IsTransport { get; set; }
    public IShootable Weapon { get; set; }
    public ReloadScript ManualReloading { get; set; }
    public TurretControl TurretControl { get; set; }
    public AICarMovement CarMovement { get; private set; }
    public ICarState CurrentState { get; private set; }

    private VehicleHealth entityHealth;

    private void Start()
    {
        Initialize();

        if (CurrentState == null)
        {
            if (IsTransport)
            {
                CurrentState = new TransportCarState();
                CurrentState.EnterState(this);
                (CurrentState as TransportCarState).SetInitialTarget(WaypointManager.GetNearestRoad(this));
            }
            else
            {
                CurrentState = new PatrolCarState();
                CurrentState.EnterState(this);
            }
        }
    }

    private void Update()
    {
        if (entityHealth.IsDead)
        {
            return;
        }
        CurrentState.UpdateState();
    }

    private void Initialize()
    {
        entityHealth = GetComponent<VehicleHealth>();
        TurretControl = GetComponentInChildren<TurretControl>();
        Weapon = GetComponentInChildren<IShootable>();
        Seeker = GetComponent<AITargetSeeker>();
        CarMovement = GetComponent<AICarMovement>();
    }

    public void SetState(ICarState state)
    {
        Initialize();
        CurrentState = state;
        CurrentState.EnterState(this);
    }
}
