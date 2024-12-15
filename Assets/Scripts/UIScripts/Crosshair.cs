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
    Outline targetOutline;
    [SerializeField] Color allyColors = Color.blue;
    [SerializeField] Color enemyColors = Color.red;

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
            if (targetOutline != null)
            {
                targetOutline.enabled = false;
                targetOutline = null;
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

                targetOutline = hit.collider.GetComponentInParent<Outline>();
                Color outlineColor;

                if (target.Fraction == Fraction.Ally)
                {
                    crosshair.color = allyColors;
                    targetHealthbar.frontFillImage.color = allyColors;
                    outlineColor = allyColors;
                }
                else
                {
                    crosshair.color = enemyColors;
                    targetHealthbar.frontFillImage.color = enemyColors;
                    outlineColor = enemyColors;
                }

                if (targetOutline)
                {
                    targetOutline.OutlineColor = outlineColor;
                    targetOutline.enabled = true;
                }
            }
        }
    }

    private void UpdateHealthBar(float valueToRemove, GameObject source)
    {
        targetHealthbar.UpdateBar(targetHealthbar.CurrentValue - valueToRemove);
    }
}
