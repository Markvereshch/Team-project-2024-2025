using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class AITargetSeeker : MonoBehaviour, ITargetSeeker
{
    private GameObject target;
    private Vector3 lastPosition;

    private SphereCollider spotRange;
    private EntityHealth carrierHealth;
    private Coroutine removeTargetCoroutine;

    public GameObject Target { get { return target; } }
    public UnityEvent OnTargetLost = new UnityEvent();

    [SerializeField] private float sphereColliderRadius = 45f;
    [SerializeField] private float timeToCalmDown = 3f;

    private void Start()
    {
        carrierHealth = GetComponent<EntityHealth>();
        spotRange = gameObject.AddComponent<SphereCollider>();
        spotRange.isTrigger = true;
        spotRange.radius = sphereColliderRadius;
        spotRange.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (target == null)
        {
            var spottedObject = other.gameObject.GetComponentInParent<EntityHealth>();
            if (spottedObject != null && carrierHealth != null && carrierHealth.fraction != spottedObject.fraction)
            {
                spottedObject.OnDie += OnTargetDied;
                target = other.gameObject;
                if (removeTargetCoroutine != null)
                {
                    StopCoroutine(removeTargetCoroutine);
                    removeTargetCoroutine = null;
                }
            }
        }
    }

    private void OnTargetDied()
    {
        OnTargetLost?.Invoke();
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
            lastPosition = target.transform.position;
            target = null;
            OnTargetLost?.Invoke();
        }
    }

    public Vector3 FindTargetPosition()
    {
        return target == null ? lastPosition : target.transform.position;
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
