using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class AchievementsMenu : MonoBehaviour
{
    [SerializeField] private GameObject window; // панель AchievementsWindow
    [SerializeField] private Transform contentParent; // Content из Scroll View
    [SerializeField] private GameObject achievementEntryPrefab; // Префаб строки

    private void Start()
    {
        window.SetActive(false);
    }

    public void OpenAchievements()
    {
        window.SetActive(true);
        PopulateAchievements();
    }

    public void CloseAchievements()
    {
        window.SetActive(false);
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject); // очистка перед следующим открытием
        }
    }

    private void PopulateAchievements()
    {
        var data = AchievementSystem.Instance.GetAllAchievements();
        foreach (var ach in data.Values)
        {
            GameObject entry = Instantiate(achievementEntryPrefab, contentParent);
            entry.GetComponent<AchievementEntryUI>().Setup(ach);
        }
    }
}
