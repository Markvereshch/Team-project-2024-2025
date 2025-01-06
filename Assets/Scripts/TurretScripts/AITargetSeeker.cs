using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class AITargetSeeker : MonoBehaviour, ITargetSeeker
{
    [SerializeField] private GameObject target;

    private SphereCollider spotRange;
    private VehicleHealth carrierHealth;

    public GameObject Target { get { return target; } }
    public UnityEvent<GameObject> OnTargetLost = new UnityEvent<GameObject>();
    public UnityEvent<GameObject> OnTargetFound = new UnityEvent<GameObject>();

    [SerializeField] private float sphereColliderRadius = 45f;

    private void Start()
    {
        carrierHealth = GetComponent<VehicleHealth>();
        spotRange = gameObject.AddComponent<SphereCollider>();
        spotRange.isTrigger = true;
        spotRange.radius = sphereColliderRadius;
        spotRange.enabled = true;
        OnTargetLost?.Invoke(null);
    }

    private void OnTriggerEnter(Collider other)
    {
        var spottedObject = other.gameObject.GetComponentInParent<VehicleHealth>();
        if (!spottedObject || carrierHealth.Fraction == spottedObject.Fraction || spottedObject.IsDead)
            return;

        if (spottedObject == SFXManager.Instance.PlayerHealth)
            SFXManager.Instance.AddNearbyEnemy();

        if (target == null)
        {
            spottedObject.OnDie += OnTargetDied;
            target = other.gameObject;

            OnTargetFound?.Invoke(target);
        }
    }

    private void OnTargetDied()
    {
        OnTargetLost?.Invoke(target);
        target = null;
    }

    private void OnTriggerExit(Collider other)
    {
        var exitedObject = other.gameObject.GetComponentInParent<VehicleHealth>();
        if (exitedObject == SFXManager.Instance.PlayerHealth && carrierHealth.Fraction != exitedObject.Fraction)
            SFXManager.Instance.RemoveNearbyEnemy();

        if (other.gameObject == target)
        {
            target = FindNearestTarget();
            OnTargetLost?.Invoke(target);
        }
    }

    public Vector3 FindTargetPosition()
    {
        return target == null ? Vector3.zero : target.transform.position;
    }

    private GameObject FindNearestTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, sphereColliderRadius);
        GameObject nearestTarget = null;
        float shortestDistance = Mathf.Infinity;

        foreach (var collider in colliders)
        {
            var spottedObject = collider.gameObject.GetComponentInParent<VehicleHealth>();
            if (spottedObject != null && carrierHealth != null && carrierHealth.Fraction != spottedObject.Fraction && !spottedObject.IsDead)
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestTarget = collider.gameObject;
                }
            }
        }

        return nearestTarget;
    }
}
