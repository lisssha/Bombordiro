using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        GameManager.Instance.SaveGame();
        SceneManager.LoadScene("GameArea");
    }

    public void BackToMenu()
    {
        GameManager.Instance.SaveGame();
        SceneManager.LoadScene("Menu");
    }
    
    public void ExitGame()
    {
        Debug.Log("Выход из игры...");
        Application.Quit();
    }
}
