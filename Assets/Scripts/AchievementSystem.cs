using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementSystem : MonoBehaviour
{
    public static AchievementSystem Instance;

    [Header("UI Elements")]
    [SerializeField] private GameObject achievementPanel;
    [SerializeField] private Image achievementImage;
    [SerializeField] private TMP_Text achievementText;
    [SerializeField] private float displayTime = 3f;

    private Dictionary<string, AchievementData> unlockedAchievements = new Dictionary<string, AchievementData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAchievements();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UnlockAchievement(string name, Sprite sprite)
    {
        if (unlockedAchievements.ContainsKey(name)) return;

        // Создаем запись
        var data = new AchievementData
        {
            name = name,
            spritePath = sprite.name, // сохраняем имя спрайта
            dateUnlocked = DateTime.Now.ToString("dd.MM.yyyy HH:mm")
        };

        unlockedAchievements[name] = data;
        ShowAchievement(data, sprite);
        SaveAchievements();
    }

    private void ShowAchievement(AchievementData data, Sprite sprite)
    {
        achievementText.text = $"Achievement unlocked!\nThe first {data.name} was created!";
        achievementImage.sprite = sprite;
        achievementPanel.SetActive(true);

        CancelInvoke(nameof(HideAchievement));
        Invoke(nameof(HideAchievement), displayTime);
    }

    private void HideAchievement()
    {
        achievementPanel.SetActive(false);
    }

    private void SaveAchievements()
    {
        string json = JsonUtility.ToJson(new AchievementList(unlockedAchievements.Values));
        PlayerPrefs.SetString("UnlockedAchievements", json);
        PlayerPrefs.Save();
    }

    private void LoadAchievements()
    {
        if (PlayerPrefs.HasKey("UnlockedAchievements"))
        {
            string json = PlayerPrefs.GetString("UnlockedAchievements");
            var list = JsonUtility.FromJson<AchievementList>(json);
            unlockedAchievements.Clear();
            foreach (var ach in list.achievements)
            {
                unlockedAchievements[ach.name] = ach;
            }
        }
    }

    public Dictionary<string, AchievementData> GetAllAchievements()
    {
        return unlockedAchievements;
    }

    [Serializable]
    private class AchievementList
    {
        public List<AchievementData> achievements = new List<AchievementData>();

        public AchievementList(IEnumerable<AchievementData> data)
        {
            achievements = new List<AchievementData>(data);
        }
    }
}
