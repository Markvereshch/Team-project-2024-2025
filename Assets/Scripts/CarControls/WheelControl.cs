using UnityEngine;

public class WheelControl : MonoBehaviour
{
    public Transform wheelModel;

    [HideInInspector] public WheelCollider WheelCollider;
    [SerializeField] private float rotationSmoothing = 0.5f;

    public bool steerable;
    public bool motorized;

    private Vector3 position;
    private Quaternion rotation;

    private void Start()
    {
        WheelCollider = GetComponent<WheelCollider>();
    }

    private void Update()
    {
        WheelCollider.GetWorldPose(out position, out rotation);
        wheelModel.transform.position = position;
        wheelModel.transform.rotation = Quaternion.Lerp(wheelModel.transform.rotation, rotation, Time.deltaTime * rotationSmoothing);
    }
}