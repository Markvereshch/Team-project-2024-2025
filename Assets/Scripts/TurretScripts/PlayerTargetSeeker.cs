using UnityEngine;

public class PlayerTargetSeeker : MonoBehaviour, ITargetSeeker
{
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    public Vector3 FindTargetPosition()
    {
        if (mainCamera == null)
        {
            return Vector3.forward;
        }

        var targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Ray rayToWorld = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(rayToWorld, out hit))
        {
            targetPos = hit.point;
        }
        else
        {
            targetPos = mainCamera.transform.TransformPoint(Vector3.forward * 200.0f);
        }
        Debug.DrawLine(mainCamera.transform.position, targetPos, Color.green);
        return targetPos;
    }
}
