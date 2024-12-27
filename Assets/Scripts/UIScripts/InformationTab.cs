using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InformationTab : MonoBehaviour 
{
    [SerializeField] Image infoImage;
    [SerializeField] TMP_Text informationText;
    [SerializeField] float deactivationTime = 5f;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Image backgroundImage;
    private Coroutine deactivationCoroutine;

    public void CreateInfoIcon(string text, Color color, Sprite sprite = null)
    {
        if (sprite != null)
            infoImage.sprite = sprite;

        informationText.text = text;
        canvasGroup.alpha = 1f;
        backgroundImage.color = new Color(color.r, color.g, color.b, color.a);

        if (deactivationCoroutine != null)
            StopCoroutine(deactivationCoroutine);

        gameObject.SetActive(true);
        deactivationCoroutine = StartCoroutine(DeactivationCoroutine());
    } 

    private IEnumerator DeactivationCoroutine()
    {
        float elapsed = 0f;
        while (elapsed < deactivationTime)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / deactivationTime);
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
