using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject mainMenuPanel;       // Панель главного меню
    public GameObject settingsPanel;       // Панель настроек

    void Start()
    {
        // Показываем главное меню и скрываем меню настроек при запуске
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void OpenSettings()
    {
        // Скрываем главное меню и показываем меню настроек
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void BackToMainMenu()
    {
        // Скрываем меню настроек и показываем главное меню
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}
