using System.Collections;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    [SerializeField] private float damageAmount = 2f;
    private Coroutine damageCoroutine;
    private bool inZone;

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        var health = other.GetComponentInParent<EntityHealth>();
        if (health != null)
        {
            damageCoroutine = StartCoroutine(Damage(health));
            inZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        var health = other.GetComponentInParent<EntityHealth>();
        if (health != null && damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
            inZone = false;
        }
    }

    private IEnumerator Damage(EntityHealth health)
    {
        while (inZone)
        {
            health.TakeDamage(damageAmount, gameObject);
            yield return new WaitForSeconds(1f);
        }
    }
}
