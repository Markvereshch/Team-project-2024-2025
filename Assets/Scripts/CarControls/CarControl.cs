using UnityEngine;

public class CarControl : BaseCarControl
{
    protected override void Awake()
    {
        base.Awake();
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

        audioController.PlayEngineSound();

        foreach (var wheel in wheelControls)
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
            PerformStop();
        }
    }
}