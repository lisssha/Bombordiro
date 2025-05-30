using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        StartCoroutine(LoadGameScene());
    }

    private IEnumerator LoadGameScene()
    {
        SceneManager.LoadScene("GameArea");
        yield return null; // Ждем завершения загрузки

        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.LoadGame();
        }
    }

    public void BackToMenu()
    {
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.SaveGame();
        }
        SceneManager.LoadScene("Menu");
    }

    public void ExitGame()
    {
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.SaveGame();
        }
        Application.Quit();
    }
}