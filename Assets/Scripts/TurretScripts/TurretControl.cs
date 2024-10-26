using UnityEngine;

public class TurretControl : MonoBehaviour
{
    public GameObject turret;
    public GameObject barrel;

    private ITargetSeeker targetSeeker;

    private float turnSpeed = 100f;
    private float elevationSpeed = 16f;
    public Vector3 targetPos;

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

        targetSeeker = gameObject.GetComponentInParent<ITargetSeeker>();
        CarInitializer carInitializer = gameObject.GetComponentInParent<CarInitializer>();
        carInitializer.OnTargetSeekerChanged.AddListener(SetTargetSeeker);

        rightRotationLimit = gunPlace.RightRotationLimit;
        leftRotationLimit = gunPlace.LeftRotationLimit;
        elevationRotationLimit = gunPlace.ElevationLimit;
        depressionRotationLimit = gunPlace.DepressionLimit;

        turnSpeed = gunBase.weaponConfig.turnSpeed;
        elevationSpeed = gunBase.weaponConfig.elevationSpeed;
    }
    
    private void Update()
    {
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            targetPos = targetSeeker.FindTargetPosition();
            Debug.DrawLine(barrel.transform.position, targetPos, Color.red);
            HorizontalRotation();
            VerticalRotation();
        }
    }

    private void SetTargetSeeker(ITargetSeeker newTargetSeeker)
    {
        targetSeeker = newTargetSeeker;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(targetPos, 5.0f);
        Gizmos.DrawLine(transform.position, transform.forward * 200.0f);
    }

    private void HorizontalRotation()
    {
        var dirOfTarget = (targetPos - transform.position);
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
