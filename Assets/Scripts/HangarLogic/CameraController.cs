using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] List<Transform> waypoints = new List<Transform>();
    [SerializeField] private float smoothTime = 0.1f;
    [SerializeField] private float minDistance = 0.05f;
    private Transform currentTarget;
    private bool isMoving = false;

    private void Start()
    {
        SetTarget(waypoints[0]);        
    }

    private void Update()
    {
        if (isMoving)
        {
            MoveCamera();
        }
    }

    public void SetPosition(int state)
    {
        if (state < 0 || state >= waypoints.Count)
        {
            Debug.LogError("Invalid state passed as a parameter to UI Button");
            return;
        }
        SetTarget(waypoints[state]);
    }

    private void SetTarget(Transform target)
    {
        currentTarget = target;
        isMoving = true;
    }

    private void MoveCamera()
    {
        if (currentTarget == null)
            return;

        Transform cameraTransform = virtualCamera.transform;

        Vector3 velocity = Vector3.zero;
        cameraTransform.position = Vector3.SmoothDamp(
            cameraTransform.position,
            currentTarget.position,
            ref velocity,
            smoothTime);

        if (Vector3.Distance(cameraTransform.position, currentTarget.position) < minDistance)
        {
            cameraTransform.position = currentTarget.position;
            isMoving = false;
        }
    }
}
