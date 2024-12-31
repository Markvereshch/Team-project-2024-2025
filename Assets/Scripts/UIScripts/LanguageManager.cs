using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LanguageManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown languageDropdown;
    [SerializeField] private string defaultLanguage = "en";

    private IEnumerator Start()
    {
        if (languageDropdown == null)
        {
            Debug.LogError("Language Dropdown is not assigned in the inspector.");
            yield break;
        }
        yield return LocalizationSettings.InitializationOperation;

        string savedLanguage = PlayerPrefs.GetString("SelectedLanguage", defaultLanguage);
        var locale = LocalizationSettings.AvailableLocales.GetLocale(savedLanguage);
        if (locale == null)
        {
            Debug.LogWarning($"Saved language '{savedLanguage}' not found. Using default locale.");
            locale = LocalizationSettings.AvailableLocales.Locales[0];
        }
        LocalizationSettings.SelectedLocale = locale;

        PopulateDropdown();
        languageDropdown.value = GetCurrentLanguageIndex();
        languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
    }

    private void PopulateDropdown()
    {
        languageDropdown.ClearOptions();

        var localeOptions = new List<string>();
        foreach (var locale in LocalizationSettings.AvailableLocales.Locales)
        {
            localeOptions.Add(locale.LocaleName);
        }

        languageDropdown.AddOptions(localeOptions);
    }

    private int GetCurrentLanguageIndex()
    {
        var currentLocale = LocalizationSettings.SelectedLocale;
        int index = LocalizationSettings.AvailableLocales.Locales.IndexOf(currentLocale);
        if (index == -1)
        {
            Debug.LogWarning("Current locale not found in available locales. Defaulting to first locale.");
            return 0;
        }
        return index;
    }

    public void OnLanguageChanged(int index)
    {
        if (index < 0 || index >= LocalizationSettings.AvailableLocales.Locales.Count)
        {
            Debug.LogError("Invalid language index selected.");
            return;
        }

        var selectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
        LocalizationSettings.SelectedLocale = selectedLocale;

        PlayerPrefs.SetString("SelectedLanguage", selectedLocale.Identifier.Code);
        PlayerPrefs.Save();
    }
}
