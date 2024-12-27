using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveUIElement : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image objectiveIcon;
    [SerializeField] private Image completionStatusIcon;
    [SerializeField] private Image timerBG;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text objectiveNameText;
    [SerializeField] private TMP_Text objectiveDescriptionText;
    [Header("Completion statuses")]
    [SerializeField] private Sprite completedSprite;
    [SerializeField] private Sprite failedSprite;
    private float remainingTime;
    public void SetupTask(Sprite objectiveIconSprite, string objectiveName, string objectiveDescription, bool hasTimer = false, float timerValue = 0f)
    {
        objectiveIcon.sprite = objectiveIconSprite;
        objectiveNameText.text = objectiveName;
        objectiveDescriptionText.text = objectiveDescription;

        completionStatusIcon.gameObject.SetActive(false);
        timerBG.gameObject.SetActive(hasTimer);

        if (hasTimer)
        {
            remainingTime = timerValue;
            InvokeRepeating(nameof(UpdateTimer), 0, 1f);
        }
    }

    public void SetStatus(bool isCompleted)
    {
        completionStatusIcon.gameObject.SetActive(true);
        completionStatusIcon.sprite = isCompleted ? completedSprite : failedSprite;
    }

    public void UpdateTimer(float timerValue)
    {
        int minutes = (int) timerValue/60;
        int seconds = (int) timerValue % 60;
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void UpdateTimer()
    {
        remainingTime -= 1f;
        UpdateTimer(remainingTime);

        if (remainingTime <= 0)
        {
            CancelInvoke(nameof(UpdateTimer));
        }
    }
}
