using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class AITargetSeeker : MonoBehaviour, ITargetSeeker
{
    [SerializeField] private GameObject target;

    private SphereCollider spotRange;
    private VehicleHealth carrierHealth;
    private Coroutine removeTargetCoroutine;

    public GameObject Target { get { return target; } }
    public UnityEvent<GameObject> OnTargetLost = new UnityEvent<GameObject>();
    public UnityEvent<GameObject> OnTargetFound = new UnityEvent<GameObject>();

    [SerializeField] private float sphereColliderRadius = 45f;
    [SerializeField] private float timeToCalmDown = 10f;

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
        if (target == null)
        {
            var spottedObject = other.gameObject.GetComponentInParent<VehicleHealth>();
            if (spottedObject != null && carrierHealth != null && carrierHealth.Fraction != spottedObject.Fraction && !spottedObject.IsDead)
            {
                spottedObject.OnDie += OnTargetDied;
                target = other.gameObject;
                if (removeTargetCoroutine != null)
                {
                    StopCoroutine(removeTargetCoroutine);
                    removeTargetCoroutine = null;
                }
                OnTargetFound?.Invoke(target);
            }
        }
    }

    private void OnTargetDied()
    {
        OnTargetLost?.Invoke(target);
        target = null;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == target)
        {
            removeTargetCoroutine = StartCoroutine(RemoveTarget());
        }
    }

    private IEnumerator RemoveTarget()
    {
        yield return new WaitForSeconds(timeToCalmDown);
        if (target != null)
        {
            OnTargetLost?.Invoke(target);
            target = null;
        }
    }

    public Vector3 FindTargetPosition()
    {
        return target == null ? Vector3.zero : target.transform.position;
    }

    private void OnDestroy()
    {
        if (removeTargetCoroutine != null)
        {
            StopCoroutine(removeTargetCoroutine);
        }
        Destroy(spotRange);
    }
}
