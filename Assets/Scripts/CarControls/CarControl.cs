using UnityEngine;

public class CarControl : MonoBehaviour
{
    [SerializeField] private float motorTorque = 2000;
    [SerializeField] private float brakeTorque = 2000;
    [SerializeField] private float maxSpeed = 20;
    [SerializeField] private float steeringRange = 45;
    [SerializeField] private float steeringRangeAtMaxSpeed = 10;
    [SerializeField] private float centreOfGravityOffset = -1f;
    [SerializeField] private float brakeAcceleration = 5000.0f;

    private Vector3 centerOfMass = new Vector3(0.30f, 5f, 0.06f);
    private WheelControl[] wheels;
    private Rigidbody rigidBody;

    public KeyCode keyBrake = KeyCode.Space; //REPLACE WITH NEW INPUT SYSTEM

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.centerOfMass = centerOfMass;
    }

    private void Start()
    { 
        rigidBody.centerOfMass += Vector3.up * centreOfGravityOffset;
        wheels = GetComponentsInChildren<WheelControl>();
    }

    private void Update()
    {
        GetMovementInputs(out float verticalInput, out float horizontalInput);

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

        Brake();
    }

    private void GetMovementInputs(out float verticalInput, out float horizontalInput)
    {
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");
    }

    private void Brake()
    {
        if (Input.GetKey(keyBrake))
        {
            foreach (var wheel in wheels)
            {
                wheel.WheelCollider.brakeTorque = brakeAcceleration;
            }
        }
    }
}