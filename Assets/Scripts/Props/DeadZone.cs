using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        var health = other.GetComponentInParent<VehicleHealth>();
        if (health != null)
        {
            health.Kill();
        }
    }
}
