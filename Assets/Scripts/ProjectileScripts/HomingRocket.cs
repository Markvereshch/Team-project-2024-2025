using UnityEngine;

public class HomingRocket : ExplosiveProjectile
{
    [Header("Movement")]
    [SerializeField] private float speed = 30f;
    [SerializeField] private float rotationSpeed = 45f;
    private Rigidbody rigidBody;
    private GameObject target;

    [Header("Prediction")]
    [SerializeField] private float maxDistancePredict = 100;
    [SerializeField] private float minDistancePredict = 5;
    [SerializeField] private float maxTimePrediction = 5;
    [SerializeField] private float scanRadius = 30f;
    [SerializeField] private float scanRange = 180f;
    private Vector3 standardPrediction, deviatedPrediction;
    private Rigidbody targetRb;

    [Header("Deviation")]
    [SerializeField] private float deviationAmount = 50;
    [SerializeField] private float deviationSpeed = 2;

    private IDamagable damagableProp;

    override protected void Start()
    {
        base.Start();
        rigidBody = GetComponent<Rigidbody>();
        FindNearestTarget();
        damagableProp = GetComponent<IDamagable>();
        damagableProp.OnDie += base.Explode;
    }
    
    private void SetTarget(GameObject target)
    {
        this.target = target;
        targetRb = target ? target.GetComponent<Rigidbody>() : null;
    }

    private void FindNearestTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, scanRadius, damagableLayer, QueryTriggerInteraction.Ignore);
        float closestDistance = Mathf.Infinity;
        GameObject closestTarget = null;

        foreach (var collider in colliders)
        {
            var health = collider.gameObject.GetComponentInParent<VehicleHealth>();
            if (health != null && !health.IsDead && source != null && health.Fraction != source.GetComponentInParent<VehicleHealth>().Fraction)
            {
                Vector3 directionToTarget = (collider.transform.position - transform.position).normalized;
                float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

                if (angleToTarget <= scanRange / 2)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, collider.transform.position);
                    if (distanceToTarget < closestDistance)
                    {
                        closestDistance = distanceToTarget;
                        closestTarget = health.gameObject;
                    }
                }
            }
        }
        SetTarget(closestTarget);
    }

    private void FixedUpdate()
    {
        rigidBody.velocity = transform.forward * speed;
        
        if (target != null)
        {
            var leadPercentage = Mathf.InverseLerp(minDistancePredict, maxDistancePredict, Vector3.Distance(transform.position, target.transform.position));
            PredictMovement(leadPercentage);
            AddDeviation(leadPercentage);
            RotateRocket();
        }
    }

    private void PredictMovement(float leadPercentage)
    {
        var predictionTime = Mathf.Lerp(0, maxTimePrediction, leadPercentage);    
        standardPrediction = targetRb.position + targetRb.velocity * predictionTime;
    }

    private void AddDeviation(float leadPercentage)
    {
        var deviation = new Vector3(Mathf.Cos(Time.time * deviationSpeed), 0, 0);
        var predictionOffset = transform.TransformDirection(deviation) * deviationAmount * leadPercentage;
        deviatedPrediction = standardPrediction + predictionOffset;
    }

    private void RotateRocket()
    {
        var heading = deviatedPrediction - transform.position;
        var rotation = Quaternion.LookRotation(heading);
        rigidBody.MoveRotation(Quaternion.RotateTowards(transform.rotation, rotation, rotationSpeed * Time.deltaTime));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, standardPrediction);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(standardPrediction, deviatedPrediction);
    }
}
