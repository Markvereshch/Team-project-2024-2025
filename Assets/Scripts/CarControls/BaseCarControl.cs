using UnityEngine;

public class BaseCarControl : MonoBehaviour
{
    [Header("Car handling stats")]
    [SerializeField] protected float steeringRange;
    [SerializeField] protected float steeringRangeAtMaxSpeed;
    [SerializeField] protected float motorTorque;
    [SerializeField] protected float maxSpeed;
    [SerializeField] protected float brakeTorque;
    [SerializeField] protected float brakeAcceleration;

    [Header("Physics Settings")]
    [SerializeField] protected Vector3 centerOfMass = new Vector3(0.34f, 0f, 0.06f);
    [SerializeField] protected float centerOfGravityOffset = -1f;

    protected Rigidbody rigidBody;
    protected VehicleStats stats;
    protected WheelControl[] wheelControls;
    protected VehicleAudioController audioController;

    protected virtual void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.centerOfMass = centerOfMass + Vector3.up * centerOfGravityOffset;
        wheelControls = GetComponentsInChildren<WheelControl>();
        stats = GetComponent<VehicleStats>();
        audioController = GetComponent<VehicleAudioController>();
        ApplyStats();
    }

    protected void PerformBrake(bool performing)
    {
        foreach (var wheel in wheelControls)
        {
            wheel.WheelCollider.brakeTorque = performing ? brakeTorque : 0;
        }
        if (audioController)
            audioController.PlayBrakeSound();
    }

    public void PerformStop()
    {
        foreach (var wheel in wheelControls)
        {
            wheel.WheelCollider.brakeTorque = brakeAcceleration;
        }
    }

    public void Horn()
    {
        audioController.PlayHornSound();
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
}
