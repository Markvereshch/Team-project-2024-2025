
using UnityEngine;
using UnityEngine.AI;

public class AICarMovement : MonoBehaviour
{
    [Header("AI Navigation")]
    [SerializeField] private GameObject agentPrefab;

    [Header("Wheel Controls")]
    [SerializeField] private WheelControl[] wheelControls;
    [SerializeField] private float steeringRange = 45f;
    [SerializeField] private float steeringRangeAtMaxSpeed = 40f;
    [SerializeField] private float motorTorque = 2000f;
    [SerializeField] private float maxSpeed = 20f;
    [SerializeField] private float brakeTorque = 1000f;
    [SerializeField] private float brakeAcceleration = 5000.0f;

    [Header("Physics Settings")]
    [SerializeField] private Vector3 centerOfMass = new Vector3(0.34f, 0f, 0.06f);
    [SerializeField] private float centerOfGravityOffset = -1f;

    [Header("Sensors")]
    [SerializeField] private float midSensorLength = 30f;
    [SerializeField] private float sideSensorLength = 22f;
    [SerializeField] private float angleSensorLength = 18f;
    [SerializeField] private float spaceBetweenSensors = 2f;
    [SerializeField] private float sideSensorAngle = 45f;
    [SerializeField] private Vector3 frontSensorPosition = new Vector3(0f, 0f, 5f);
    [SerializeField] private Vector3 backSensorPosition = new Vector3(0f, 0f, -5f);

    [Header("Logic")]
    [SerializeField] private float brakeStartSpeed = 6f;
    [SerializeField] private float waitForReversing = 2f;
    [SerializeField] private float reversingTime = 3f;
    [SerializeField] private float backSensorsLengthMultiplier = 0.4f;

    private NavMeshAgent agent;
    private GameObject target;
    private Rigidbody rigidBody;
    private EntityHealth entityHealth;
    private Vector3 previousTargetPosition;
    private bool isAvoiding;
    private float avoidSensitivity = 1f;
    private float startReversingSpeed = 2f;
    private bool movingForwards = true;
    private float reversingCounter;
    private float updateTargetPositionDistance = 1.0f;

    private void Awake()
    {
        entityHealth = GetComponent<EntityHealth>();
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.centerOfMass = centerOfMass + Vector3.up * centerOfGravityOffset;
        wheelControls = GetComponentsInChildren<WheelControl>();
    }

    private void Start()
    {
        if (agent == null)
        {
            agentPrefab = entityHealth.agentAI;
            GameObject tracker = Instantiate(agentPrefab, transform.position, transform.rotation) as GameObject;
            agent = tracker.GetComponent<NavMeshAgent>();
        }
    }

    private void FixedUpdate()
    {
        if (target != null && !entityHealth.IsDead)
        {
            UpdateAgentDestination();
            HandleCarMovement();
            UseSensors(movingForwards);
            HandleReversing();
        }
    }

    public void SetTarget(GameObject target)
    {
        this.target = target;
        if (target != null)
        {
            agent.SetDestination(target.transform.position);
        }
    }

    private void UseSensors(bool forwardMovement)
    {
        Vector3 direction = forwardMovement ? transform.forward : -transform.forward;
        Vector3 sensorPosition = forwardMovement ? frontSensorPosition : backSensorPosition;
        Vector3 startPosition = transform.position + transform.TransformDirection(sensorPosition);
        float currentAngle = forwardMovement ? sideSensorAngle : -sideSensorAngle;
        RaycastHit hit;

        float avoidMultiplier = 0;
        isAvoiding = false;

        float lengthMultiplier = movingForwards ? 1 : backSensorsLengthMultiplier;

        if (movingForwards && rigidBody.velocity.magnitude >= brakeStartSpeed &&
            Physics.Raycast(startPosition, direction, out hit, midSensorLength * lengthMultiplier, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                isAvoiding = true;
            }
            PerformBrake(isAvoiding);
            Debug.DrawLine(startPosition, hit.point, Color.red);
        }

        Vector3 rightSensorPos = startPosition + transform.TransformDirection(Vector3.right * spaceBetweenSensors);
        Vector3 rightAngleDirection = Quaternion.AngleAxis(currentAngle, transform.up) * direction;
        if (Physics.Raycast(rightSensorPos, direction, out hit, sideSensorLength * lengthMultiplier, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                isAvoiding = true;
                avoidMultiplier -= avoidSensitivity;
                Debug.DrawLine(rightSensorPos, hit.point, Color.blue);
            }
        }
        else if (Physics.Raycast(rightSensorPos, rightAngleDirection, out hit, angleSensorLength * lengthMultiplier, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                isAvoiding = true;
                avoidMultiplier -= avoidSensitivity / 2;
                Debug.DrawLine(rightSensorPos, hit.point, Color.cyan);
            }
        }

        Vector3 leftSensorPos = startPosition + transform.TransformDirection(Vector3.left * spaceBetweenSensors);
        Vector3 leftAngleDirection = Quaternion.AngleAxis(-currentAngle, transform.up) * direction;
        if (Physics.Raycast(leftSensorPos, direction, out hit, sideSensorLength * lengthMultiplier, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                isAvoiding = true;
                avoidMultiplier += avoidSensitivity;
                Debug.DrawLine(leftSensorPos, hit.point, Color.blue);
            }
        }
        else if (Physics.Raycast(leftSensorPos, leftAngleDirection, out hit, angleSensorLength * lengthMultiplier, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                isAvoiding = true;
                avoidMultiplier += avoidSensitivity / 2;
                Debug.DrawLine(leftSensorPos, hit.point, Color.cyan);
            }
        }

        if (avoidMultiplier == 0 &&
            Physics.Raycast(startPosition, direction, out hit, midSensorLength * lengthMultiplier, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                if (hit.normal.x < 0)
                {
                    avoidMultiplier -= avoidSensitivity;
                }
                else
                {
                    avoidMultiplier += avoidSensitivity;
                }
                Debug.DrawLine(startPosition, hit.point, Color.blue);
            }
        }

        if (isAvoiding)
        {
            foreach (var wheel in wheelControls)
            {
                if (wheel.steerable)
                {
                    wheel.WheelCollider.steerAngle = steeringRange * avoidMultiplier;
                }
            }
        }
    }

    private void HandleReversing()
    {
        if (rigidBody.velocity.magnitude < startReversingSpeed && movingForwards)
        {
            reversingCounter += Time.deltaTime;

            if (reversingCounter >= waitForReversing)
            {
                reversingCounter = 0;
                movingForwards = false;
            }
        }
        else if (movingForwards)
        {
            reversingCounter = 0;
        }

        if (!movingForwards)
        {
            reversingCounter += Time.deltaTime;
            if (reversingCounter >= reversingTime)
            {
                reversingCounter = 0;
                movingForwards = true;
            }
        }
    }

    private void UpdateAgentDestination()
    {
        Vector3 currentTargetPosition = target.transform.position;
        if (Vector3.Distance(previousTargetPosition, currentTargetPosition) > updateTargetPositionDistance)
        {
            agent.SetDestination(currentTargetPosition);
            previousTargetPosition = currentTargetPosition;
        }
    }

    private void HandleCarMovement()
    {
        float forwardSpeed = GetForwardSpeed();
        float steerAngle = CalculateSteerAngle(forwardSpeed);
        float torque = CalculateMotorTorque(forwardSpeed);

        ApplyWheelControls(steerAngle, torque, forwardSpeed);
    }

    private float GetForwardSpeed() =>
        Vector3.Dot(transform.forward, rigidBody.velocity);

    private float CalculateSteerAngle(float speed)
    {
        float speedFactor = Mathf.InverseLerp(0, maxSpeed, Mathf.Abs(speed));
        float steerRange = Mathf.Lerp(steeringRange, steeringRangeAtMaxSpeed, speedFactor);

        Vector3 relativePos = transform.InverseTransformPoint(agent.destination);
        return (relativePos.x / relativePos.magnitude) * steerRange;
    }

    private float CalculateMotorTorque(float speed) =>
        Mathf.Lerp(motorTorque, 0, Mathf.InverseLerp(0, maxSpeed, Mathf.Abs(speed)));

    private void ApplyWheelControls(float steerAngle, float motorTorque, float forwardSpeed)
    {
        int movementDirection = DetermineMovementDirection();
        bool isAccelerating = Mathf.Sign(movementDirection) == Mathf.Sign(forwardSpeed);

        foreach (var wheel in wheelControls)
        {
            if (wheel.steerable)
            {
                if (!isAvoiding)
                {
                    wheel.WheelCollider.steerAngle = steerAngle;
                }
            }

            if (wheel.motorized)
            {
                wheel.WheelCollider.motorTorque = isAccelerating ? motorTorque * movementDirection : 0;
            }

            wheel.WheelCollider.brakeTorque = isAccelerating ? 0 : brakeTorque;
        }
    }

    private int DetermineMovementDirection() => movingForwards ? 1 : -1;

    private void PerformBrake(bool performing)
    {
        foreach (var wheel in wheelControls)
        {
            wheel.WheelCollider.brakeTorque = performing ? brakeTorque : 0;
        }
    }

    public void PerformStop()
    {
        foreach (var wheel in wheelControls)
        {
            wheel.WheelCollider.brakeTorque = brakeAcceleration;
        }
    }

    private void OnDestroy()
    {
        if (agent)
        {
            Destroy(agent.gameObject);
        }
    }
}