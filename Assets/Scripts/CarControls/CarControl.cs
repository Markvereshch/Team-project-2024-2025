using UnityEngine;

public class CarControl : MonoBehaviour
{
    [Header("Mobility stats")]
    [SerializeField] private float motorTorque;
    [SerializeField] private float brakeTorque;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float steeringRange;
    [SerializeField] private float steeringRangeAtMaxSpeed;
    [SerializeField] private float brakeAcceleration;

    [Header("Center of mass")]
    [SerializeField] private float centreOfGravityOffset = -1f;
    [SerializeField] private Vector3 centerOfMass = new Vector3(0.34f, 0f, 0.06f);

    private WheelControl[] wheels;
    private VehicleStats stats;
    private Rigidbody rigidBody;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.centerOfMass = centerOfMass;
    }

    private void Start()
    { 
        rigidBody.centerOfMass += Vector3.up * centreOfGravityOffset;
        wheels = GetComponentsInChildren<WheelControl>();
        stats = GetComponent<VehicleStats>();
        ApplyStats();
    }

    private void ApplyStats()
    {
        motorTorque = stats.motorTorque;
        brakeTorque = stats.brakeTorque;
        steeringRange = stats.steeringRange;
        steeringRangeAtMaxSpeed = stats.steeringRangeAtMaxSpeed;
        maxSpeed = stats.maxSpeed;
        brakeAcceleration = stats.brakeAcceleration;
    }

    public void Move(bool brakePressed, Vector2 movementInput)
    {
        float verticalInput = movementInput.y;

        float horizontalInput = movementInput.x;

        float forwardSpeed = Vector3.Dot(transform.forward, rigidBody.velocity);

        float speedFactor = Mathf.InverseLerp(0, maxSpeed, forwardSpeed);

        float currentMotorTorque = Mathf.Lerp(motorTorque, 0, speedFactor);

        float currentSteerRange = Mathf.Lerp(steeringRange, steeringRangeAtMaxSpeed, speedFactor);

        bool isAccelerating = Mathf.Sign(verticalInput) == Mathf.Sign(forwardSpeed);

        foreach (var wheel in wheels)
        {
            if (wheel.steerable)
            {
                wheel.WheelCollider.steerAngle = horizontalInput * currentSteerRange;
            }

            if (isAccelerating)
            {
                if (wheel.motorized)
                {
                    wheel.WheelCollider.motorTorque = verticalInput * currentMotorTorque;
                }
                wheel.WheelCollider.brakeTorque = 0;
            }
            else
            {
                wheel.WheelCollider.brakeTorque = Mathf.Abs(verticalInput) * brakeTorque;
                wheel.WheelCollider.motorTorque = 0;
            }
        }
        if (brakePressed)
        {
            PerformBrake();
        }
    }

    private void PerformBrake()
    {
        foreach (var wheel in wheels)
        {
            wheel.WheelCollider.brakeTorque = brakeAcceleration;
        }
    }
}