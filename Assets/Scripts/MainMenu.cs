using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("GameArea");
    }
    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
    public void ExitGame()
    {
        Debug.Log("����� �� ����...");
        Application.Quit();
    }
}
