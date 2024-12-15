using UnityEngine;

public class TurretControl : MonoBehaviour
{
    public GameObject turret;
    public GameObject barrel;

    //Must be placed in WeaponCharacteristicScript
    public float turnSpeed = 100f;
    public float elevationSpeed = 16f;
    //Must be placed in TargetSeekerScript
    public Vector3 targetPos;
    private Camera mainCamera;

    [Range(0, 180)]
    private float rightRotationLimit = 180f;
    [Range(0, 180)]
    private float leftRotationLimit = 180f;
    [Range(0, 90)]
    private float elevationRotationLimit = 25f;
    [Range(0, 90)]
    private float depressionRotationLimit = 25f;

    private GunPlaceScript gunPlace;
    private GunBaseScript gunBase;

    private void Start()
    {
        gunPlace = transform.parent.gameObject.GetComponent<GunPlaceScript>();
        gunBase = gameObject.GetComponent<GunBaseScript>();
        rightRotationLimit = gunPlace.RightRotationLimit;
        leftRotationLimit = gunPlace.LeftRotationLimit;
        elevationRotationLimit = gunPlace.ElevationLimit;
        depressionRotationLimit = gunPlace.DepressionLimit;

        //turnSpeed = gunBase.weaponCharacteristics.turnSpeed;
        //elevationSpeed = gunBase.weaponCharacteristics.elevationSpeed;

        mainCamera = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;

    }

    private void Update()
    {
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            FindTarget();
            HorizontalRotation();
            VerticalRotation();
        }
    }

    private void FindTarget()
    {
        targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
        Debug.DrawLine(barrel.transform.position, targetPos, Color.red);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(targetPos, 5.0f);
        Gizmos.DrawLine(transform.position, transform.forward * 200.0f);
    }

    private void HorizontalRotation()
    {
        Vector3 target = targetPos;
        var dirOfTarget = (target - transform.position);
        dirOfTarget.y = 0f;
        float targAngle = Vector3.SignedAngle(dirOfTarget, transform.parent.forward, Vector3.up);

        if (targAngle < -rightRotationLimit)
        {
            targAngle = -rightRotationLimit;
        }

        if (targAngle > leftRotationLimit)
        {
            targAngle = leftRotationLimit;
        }

        if (rightRotationLimit != leftRotationLimit)
        {
            float currentAngle = Vector3.SignedAngle(transform.forward, transform.parent.forward, Vector3.up);
            var delta = targAngle - currentAngle;
            delta = Mathf.Min(turnSpeed * Time.deltaTime, Mathf.Abs(delta)) * Mathf.Sign(delta);

            transform.localRotation *= Quaternion.Euler(0f, -delta, 0f);
        }
        else
        {
            Vector3 targetPositionInLocalSpace = transform.InverseTransformPoint(targetPos);
            targetPositionInLocalSpace.y = 0.0f;
            var limitedRotation = Vector3.RotateTowards(Vector3.forward, targetPositionInLocalSpace, Mathf.Deg2Rad * leftRotationLimit, float.MaxValue);
            Quaternion whereToRotate = Quaternion.LookRotation(limitedRotation);
            turret.transform.localRotation = Quaternion.RotateTowards(turret.transform.localRotation, whereToRotate, turnSpeed * Time.deltaTime);
        }
    }

    private void VerticalRotation()
    {
        Vector3 targetPositionInLocalSpace = turret.transform.InverseTransformPoint(targetPos);
        targetPositionInLocalSpace.x = 0.0f;
        Vector3 limitedRotation = targetPositionInLocalSpace;
        if (targetPositionInLocalSpace.y >= 0.0f)
        {
            limitedRotation = Vector3.RotateTowards(Vector3.forward, targetPositionInLocalSpace, Mathf.Deg2Rad * elevationRotationLimit, float.MaxValue);
        }
        else
        {
            limitedRotation = Vector3.RotateTowards(Vector3.forward, targetPositionInLocalSpace, Mathf.Deg2Rad * depressionRotationLimit, float.MaxValue);
        }
        Quaternion whereToRotate = Quaternion.LookRotation(limitedRotation);
        barrel.transform.localRotation = Quaternion.RotateTowards(barrel.transform.localRotation, whereToRotate, elevationSpeed * Time.deltaTime);
    }
}
