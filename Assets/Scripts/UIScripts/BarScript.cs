using UnityEngine;
using UnityEngine.UI;

public class BarScript : MonoBehaviour
{
    [SerializeField] public Image frontFillImage;
    [SerializeField] private Image backFillImage;
    [SerializeField] private float lerpTimer;
    [SerializeField] private float chaseSpeed = 5f;
    private bool isUIDepricated = false;
    public float CurrentValue { get; set; }
    public float MaxValue { get; set; }

    private void Update()
    {
        if (isUIDepricated)
            UpdateUI();
    }

    public void UpdateBar(float newValue)
    {
        CurrentValue = newValue;
        lerpTimer = 0f;
        CurrentValue = Mathf.Clamp(CurrentValue, 0, MaxValue);
        isUIDepricated = true;
    }

    private void UpdateUI()
    {
        float frontFill = frontFillImage.fillAmount;
        float backFill = backFillImage.fillAmount;
        float fraction = CurrentValue / MaxValue;

        if (Mathf.Approximately(backFill, fraction) && Mathf.Approximately(frontFill, fraction))
        {
            isUIDepricated = false;
            lerpTimer = 0f;
            return;
        }

        lerpTimer += Time.deltaTime;
        float percentComplete = lerpTimer / chaseSpeed;
        percentComplete *= percentComplete;

        if (backFill > fraction)
        {
            frontFillImage.fillAmount = fraction;
            backFillImage.color = Color.red;
            backFillImage.fillAmount = Mathf.Lerp(backFill, fraction, percentComplete);
        }
        if (frontFill < fraction)
        {
            backFillImage.fillAmount = fraction;
            backFillImage.color = Color.green;
            frontFillImage.fillAmount = Mathf.Lerp(frontFill, backFillImage.fillAmount, percentComplete);
        }
    }

    public void SetBar(float currentValue, float maxValue)
    {
        gameObject.SetActive(true);
        CurrentValue = currentValue;
        MaxValue = maxValue;
        lerpTimer = 0f;
        isUIDepricated = false;

        float fraction = CurrentValue / MaxValue;
        frontFillImage.fillAmount = backFillImage.fillAmount = fraction;
    }
}
