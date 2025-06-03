using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private bool _isLoading = false;

    public void PlayGame()
    {
        if (_isLoading) return;
        _isLoading = true;

        StartCoroutine(LoadGameScene());
    }

    private IEnumerator LoadGameScene()
    {
        // Загружаем сцену асинхронно
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("GameArea");

        // Ждем завершения загрузки
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Убеждаемся, что все объекты инициализированы
        yield return new WaitForEndOfFrame();

        // Загружаем сохранение
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.LoadGame();
        }

        _isLoading = false;
    }

    public void BackToMenu()
    {
        if (_isLoading) return;
        _isLoading = true;

        SaveSystem.Instance.SaveGame();
        SceneManager.LoadScene("Menu");

        _isLoading = false;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private void OnApplicationQuit()
    {
        SaveSystem.Instance.SaveGame();
    }
}