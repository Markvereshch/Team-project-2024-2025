using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    [SerializeField] private Image crosshair;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask raycastLayers;
    [SerializeField] private float maxDistance = 150f;
    [SerializeField] private BarScript targetHealthbar;

    VehicleHealth target;
    private void Update()
    {
        var mousePos = Input.mousePosition;
        crosshair.transform.position = mousePos;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance, raycastLayers, QueryTriggerInteraction.Ignore))
        {
            HandleHit(hitInfo);
        }
        else if (crosshair.color != Color.white)
        {
            crosshair.color = Color.white;
            if (target != null)
            {
                target.OnDamaged -= UpdateHealthBar;
                target = null;
                targetHealthbar.gameObject.SetActive(false);
            }
        }
    }

    private void HandleHit(RaycastHit hit)
    {
        var newTarget = hit.collider.GetComponentInParent<VehicleHealth>();
        if (newTarget != target)
        {
            if (target != null)
            {
                target.OnDamaged -= UpdateHealthBar;
            }
            target = newTarget;

            if (target != null && !target.IsDead && target.gameObject != InGameUIManager.Instance.Player)
            {
                targetHealthbar.SetBar(target.CurrentHealth, target.Stats.maxHealth);
                target.OnDamaged += UpdateHealthBar;
                if (target.Fraction == Fraction.Ally)
                {
                    crosshair.color = Color.green;
                    targetHealthbar.frontFillImage.color = Color.blue;
                }
                else if (target.Fraction == Fraction.Enemy)
                {
                    crosshair.color = Color.red;
                    targetHealthbar.frontFillImage.color = Color.magenta;
                }
            }
        }
    }

    private void UpdateHealthBar(float valueToRemove, GameObject source)
    {
        targetHealthbar.UpdateBar(targetHealthbar.CurrentValue - valueToRemove);
    }
}
