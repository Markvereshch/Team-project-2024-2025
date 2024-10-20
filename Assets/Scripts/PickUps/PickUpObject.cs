using UnityEngine;

public class PickUpObject : MonoBehaviour
{
    private Transform transformComponent;
    private float rotationSpeed = 0.5f;

    void Start()
    {
        transformComponent = gameObject.GetComponent<Transform>();
    }

    void Update()
    {
        transformComponent.Rotate(0, rotationSpeed, 0);
    }
}
