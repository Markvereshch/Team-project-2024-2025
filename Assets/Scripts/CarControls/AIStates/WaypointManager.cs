using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public static Waypoint GetNearestRoad(AICarControl carControl)
    {
        Waypoint[] waypoints = FindObjectsOfType<Waypoint>();
        if (waypoints.Length == 0)
        {
            Debug.LogWarning("No waypoints found in the scene.");
            return null;
        }

        Waypoint nearestWaypoint = null;
        float nearestDistance = Mathf.Infinity;
        Vector3 carPosition = carControl.transform.position;

        foreach (Waypoint waypoint in waypoints)
        {
            float distance = Vector3.Distance(carPosition, waypoint.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestWaypoint = waypoint;
            }
        }

        return nearestWaypoint;
    }
}
