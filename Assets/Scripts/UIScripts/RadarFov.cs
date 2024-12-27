using UnityEngine;

public class RadarFov : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    void FixedUpdate()
    {
        if (InGameUIManager.Instance == null || InGameUIManager.Instance.Player == null)
            return;

        Vector3 mouseScreenPosition = Input.mousePosition;

        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(
            mouseScreenPosition.x,
            mouseScreenPosition.y,
            mainCamera.transform.position.y - InGameUIManager.Instance.Player.transform.position.y
        ));

        Vector3 directionToMouse = mouseWorldPosition - InGameUIManager.Instance.Player.transform.position;
        directionToMouse.y = 0f;

        float currentAngle = Vector3.SignedAngle(InGameUIManager.Instance.Player.transform.forward, directionToMouse, Vector3.up);

        transform.rotation = Quaternion.Euler(0f, 0f, -currentAngle);
    }
}
