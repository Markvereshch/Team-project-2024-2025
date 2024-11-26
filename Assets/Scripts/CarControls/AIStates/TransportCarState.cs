using UnityEngine;

public class TransportCarState : ICarState
{
    private AICarControl carControl;
    [SerializeField] private Waypoint currentWaypoint;
    [SerializeField] private float waypointReachThreshold = 5.0f;

    public void EnterState(AICarControl carControl)
    {
        this.carControl = carControl;
    }

    public void SetInitialTarget(Waypoint waypoint)
    {
        carControl.CarMovement.SetTarget(waypoint.gameObject);
        currentWaypoint = waypoint;
    }

    public void ExitState()
    {
    }

    public void UpdateState()
    {
        if (carControl.TurretControl != null)
        {
            carControl.TurretControl.Move();
        }
        if (carControl.Weapon != null && carControl.Seeker.Target != null && carControl.TurretControl.IsTargetInSight(carControl.Seeker.Target))
        {
            carControl.Weapon.Shoot(true);
        }
        if (currentWaypoint != null && Vector3.Distance(carControl.gameObject.transform.position, currentWaypoint.transform.position) <= waypointReachThreshold)
        {
            currentWaypoint = currentWaypoint.GetNextWaypoint();
            if (currentWaypoint != null)
            {
                carControl.CarMovement.SetTarget(currentWaypoint.gameObject);
            }
            else
            {
                carControl.CarMovement.SetTarget(null);
            }
        }
        else if (currentWaypoint == null)
        {
            carControl.CarMovement.PerformStop();
        }
    }
}