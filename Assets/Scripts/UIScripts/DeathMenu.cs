using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathMenu : MonoBehaviour
{
    [Header("References to parts of death menu")]
    [SerializeField] private TMP_Text deathSourceName;
    [SerializeField] private TMP_Text deathSourceDescription;
    [SerializeField] private Image deathSourceIcon;
    [SerializeField] private Image fadeImage;
    [SerializeField] private GameObject defeatMenu;
    [Header("Death icons")]
    [SerializeField] private Sprite unknown;
    [SerializeField] private Sprite killedInAction;
    [SerializeField] private Sprite noGasoline;
    [SerializeField] private Sprite radiation;

    private LocalizedString localizedDeathCause;
    private LocalizedString localizedDeathDescription;

    private IEnumerator FadeScreen()
    {
        float duration = 2f;
        float elapsed = 0f;

        Color color = fadeImage.color;
        color.a = 0f;
        fadeImage.color = color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsed / duration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 1f;
        fadeImage.color = color;
    }

    public void SetDefeatImage(DeathCause source = DeathCause.Unknown)
    {
        switch (source)
        {
            case DeathCause.KIA:
                deathSourceIcon.sprite = killedInAction;
                localizedDeathCause = new LocalizedString("DeathLocalizationTable", "DeathCause_KIA");
                localizedDeathDescription = new LocalizedString("DeathLocalizationTable", "DeathDescription_KIA");
                break;
            case DeathCause.Radiation:
                deathSourceIcon.sprite = radiation;
                localizedDeathCause = new LocalizedString("DeathLocalizationTable", "DeathCause_Radiation");
                localizedDeathDescription = new LocalizedString("DeathLocalizationTable", "DeathDescription_Radiation");
                break;
            case DeathCause.NoGasoline:
                deathSourceIcon.sprite = noGasoline;
                localizedDeathCause = new LocalizedString("DeathLocalizationTable", "DeathCause_NoGasoline");
                localizedDeathDescription = new LocalizedString("DeathLocalizationTable", "DeathDescription_NoGasoline");
                break;
            default:
                deathSourceIcon.sprite = unknown;
                localizedDeathCause = new LocalizedString("DeathLocalizationTable", "DeathCause_Unknown");
                localizedDeathDescription = new LocalizedString("DeathLocalizationTable", "DeathDescription_Unknown");
                break;
        }

        localizedDeathCause.StringChanged += UpdateDeathSourceName;
        localizedDeathDescription.StringChanged += UpdateDeathSourceDescription;

        localizedDeathCause.RefreshString();
        localizedDeathDescription.RefreshString();
    }

    public IEnumerator DefeatCoroutine()
    {
        yield return StartCoroutine(FadeScreen());
        yield return new WaitForSeconds(1f);
        Cursor.visible = true;
        defeatMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void GoToHangar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Hangar");
    }

    private void UpdateDeathSourceName(string value)
    {
        deathSourceName.text = value;
    }

    private void UpdateDeathSourceDescription(string value)
    {
        deathSourceDescription.text = value;
    }
}

public enum DeathCause
{
    KIA,
    Radiation,
    NoGasoline,
    Unknown,
}
