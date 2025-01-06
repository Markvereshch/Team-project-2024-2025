using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class AICarMovement : BaseCarControl
{
    [Header("AI Navigation")]
    [SerializeField] private GameObject agentPrefab;

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
    private VehicleHealth vehicleHealth;
    private bool isAvoiding;
    private float avoidSensitivity = 1f;
    private float startReversingSpeed = 2f;
    private bool movingForwards = true;
    private float reversingCounter;
    private float stopNearAgentPosition = 25f;
    private bool isStopped;

    public bool IsAwaiting { get; set; }

    protected override void Awake()
    {
        base.Awake();
        vehicleHealth = GetComponent<VehicleHealth>();
    }

    public void Activate(Vector3 initialTarget)
    {
        if (agent == null)
        {
            agentPrefab = vehicleHealth.aiConfig.agentAI;
            spaceBetweenSensors = vehicleHealth.aiConfig.spaceBetweenSensors;
            frontSensorPosition = vehicleHealth.aiConfig.frontSensorPosition;
            backSensorPosition = vehicleHealth.aiConfig.backSensorPosition;

            GameObject tracker = Instantiate(agentPrefab, transform.position, transform.rotation);
            agent = tracker.GetComponent<NavMeshAgent>();
            SetTarget(initialTarget);
        }
    }

    public void Activate()
    {
        if (agent == null)
        {
            agentPrefab = vehicleHealth.aiConfig.agentAI;
            spaceBetweenSensors = vehicleHealth.aiConfig.spaceBetweenSensors;
            frontSensorPosition = vehicleHealth.aiConfig.frontSensorPosition;
            backSensorPosition = vehicleHealth.aiConfig.backSensorPosition;

            GameObject tracker = Instantiate(agentPrefab, transform.position, transform.rotation);
            agent = tracker.GetComponent<NavMeshAgent>();
        }
    }

    private void FixedUpdate()
    {
        if (IsAwaiting)
        {
            PerformStop();
        }
        else if (agent != null && !vehicleHealth.IsDead)
        {
            audioController.PlayEngineSound();
            HandleCarMovement();
            UseSensors(movingForwards);
            HandleReversing();
            StopIfTargetNotFound();
        }
    }

    public void SetTarget(GameObject target)
    {
        this.target = target;
        if (target != null)
        {
            agent.SetDestination(target.transform.position);
            isStopped = false;
        }
    }

    public void SetTarget(Vector3 target)
    {
        agent.SetDestination(target);
        isStopped = false;
        this.target = null;
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
        Collider collider = null;

        if (movingForwards && rigidBody.velocity.magnitude >= brakeStartSpeed &&
            Physics.Raycast(startPosition, direction, out hit, midSensorLength * lengthMultiplier, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                isAvoiding = true;
                collider = hit.collider;
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
                collider = hit.collider;
                Debug.DrawLine(rightSensorPos, hit.point, Color.blue);
            }
        }
        else if (Physics.Raycast(rightSensorPos, rightAngleDirection, out hit, angleSensorLength * lengthMultiplier, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                isAvoiding = true;
                avoidMultiplier -= avoidSensitivity / 2;
                collider = hit.collider;
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
                collider = hit.collider;
                Debug.DrawLine(leftSensorPos, hit.point, Color.blue);
            }
        }
        else if (Physics.Raycast(leftSensorPos, leftAngleDirection, out hit, angleSensorLength * lengthMultiplier, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                isAvoiding = true;
                avoidMultiplier += avoidSensitivity / 2;
                collider = hit.collider;
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
            UseHorn(collider);
        }
    }

    private void UseHorn(Collider collider)
    {
        var health = collider.GetComponentInParent<VehicleHealth>();
        if (health && health.Fraction == vehicleHealth.Fraction && !isStopped)
        {
            Horn();
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

    private void StopIfTargetNotFound()
    {
        if (target == null && Vector3.Distance(transform.position, agent.transform.position) < stopNearAgentPosition && (rigidBody.velocity.magnitude >= brakeStartSpeed*2 || isStopped))
        {
            if (!isStopped && audioController)
                audioController.PlayBrakeSound();

            PerformStop();
            isStopped = true;
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

        Vector3 relativePos = transform.InverseTransformPoint(agent.transform.position);
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

    private void OnDestroy()
    {
        if (agent)
        {
            Destroy(agent.gameObject);
        }
    }
}