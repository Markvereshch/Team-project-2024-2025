using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [SerializeField] private Waypoint nextWaypoint;

    public void SetNextWaypoint(Waypoint nextWaypoint)
    {
        nextWaypoint.nextWaypoint = nextWaypoint;
    }

    public Waypoint GetNextWaypoint()
    {
        return nextWaypoint;
    }

    public void OnDrawGizmos()
    {
        if (nextWaypoint != null)
            Debug.DrawLine(transform.position, nextWaypoint.transform.position, Color.yellow);
    }
}
