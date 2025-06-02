using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class AchievementsMenu : MonoBehaviour
{
    [SerializeField] private GameObject window; // панель AchievementsWindow
    [SerializeField] private Transform contentParent; // Content из Scroll View
    [SerializeField] private GameObject achievementEntryPrefab; // ѕрефаб строки

    private void Start()
    {
        window.SetActive(false);
    }

    public void OpenAchievements()
    {
        if (window == null || contentParent == null || achievementEntryPrefab == null)
        {
            Debug.LogError("AchievementsMenu не настроен: одно из полей пустое!");
            return;
        }

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
