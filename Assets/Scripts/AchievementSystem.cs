using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class AchievementSystem : MonoBehaviour
{
    public static AchievementSystem Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private GameObject achievementPanel;
    [SerializeField] private Image achievementImage;
    [SerializeField] private TMP_Text achievementText;
    [SerializeField] private float displayTime = 3f;

    private Dictionary<string, bool> unlockedAchievements = new Dictionary<string, bool>();

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

    public void CheckFirstCreation(string prefabName, Sprite prefabSprite)
    {
        if (!unlockedAchievements.ContainsKey(prefabName))
        {
            unlockedAchievements[prefabName] = true;
            ShowAchievement($"Впервые создан {prefabName}", prefabSprite);
            SaveAchievements();
        }
    }

    private void ShowAchievement(string text, Sprite image)
    {
        achievementText.text = text;
        achievementImage.sprite = image;
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
        List<string> keys = new List<string>(unlockedAchievements.Keys);
        PlayerPrefs.SetString("UnlockedAchievements", string.Join(",", keys));
    }

    private void LoadAchievements()
    {
        if (PlayerPrefs.HasKey("UnlockedAchievements"))
        {
            string[] keys = PlayerPrefs.GetString("UnlockedAchievements").Split(',');
            foreach (string key in keys)
            {
                unlockedAchievements[key] = true;
            }
        }
    }
}