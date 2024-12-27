using System.Collections;
using TMPro;
using UnityEngine;
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
        switch(source) 
        {
            case DeathCause.KIA:
                deathSourceIcon.sprite = killedInAction;
                deathSourceName.text = "KILLED IN ACTION";
                deathSourceDescription.text = 
                    "Ты не успел даже осознать, что случилось. Взрыв обрушивает тебя в тёмный мир, где только обломки твоего тела остаются в этом беспощадном аду. Крики сливаются с эхом снарядов, а ты чувствуешь, как твоя душа уходит туда, где нет ни боли, ни страха. Останки твои — лишь ещё одна жертва войны, которую никто не вспомнит.";
                break;
            case DeathCause.Radiation:
                deathSourceIcon.sprite = radiation;
                deathSourceName.text = "IRRADIATED";
                deathSourceDescription.text =
                    "Ты чувствуешь, как яд медленно проникает в каждую клетку твоего тела. Кожа покрывается язвами, а взгляд становится пустым, как мёртвое небо над головой. Внутри тебя разгорается огонь, но он не сжигает — он пожирает изнутри, оставляя только смерть. Тело уже не твоё, оно превращается в нечто чуждое, лишённое жизни и надежды.";
                break;
            case DeathCause.NoGasoline:
                deathSourceIcon.sprite = noGasoline;
                deathSourceName.text = "NO GASOLINE";
                deathSourceDescription.text = "A car without gasoline is like a wolf without legs. Good luck finding your way home on your own. You will probably die from dehydration.";
                break;
            default:
                deathSourceIcon.sprite = unknown;
                deathSourceName.text = "???";
                deathSourceDescription.text = "<NO DATA>";
                break;
        }
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
}

public enum DeathCause
{
    KIA,
    Radiation,
    NoGasoline,
    Unknown,
}
