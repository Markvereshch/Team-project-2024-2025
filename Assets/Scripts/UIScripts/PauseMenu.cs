using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    public static bool isPauseMenuOpened = false;

    void Start()
    {
        pauseMenu.SetActive(false);
    }

    public void OnPauseInput(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.IsGameOver)
            return;

        if (InventoryUIManager.Instance.IsInventoryOpened)
        {
            InventoryUIManager.Instance.OpenInventory();
            return;
        }

        if (!isPauseMenuOpened)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    public void PauseGame()
    {
        if (pauseMenu != null)
        {
            GameManager.Instance.IsGamePaused = true;
            isPauseMenuOpened = true;
            pauseMenu.SetActive(true);
            Cursor.visible = true;
            Time.timeScale = 0f;
        }
    }

    public void ResumeGame()
    {
        if (pauseMenu != null)
        {
            GameManager.Instance.IsGamePaused = false;
            isPauseMenuOpened = false;
            pauseMenu.SetActive(false);
            Cursor.visible = false;
            Time.timeScale = 1f;
        }
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
