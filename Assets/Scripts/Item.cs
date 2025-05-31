using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Item : MonoBehaviour
{
    public string itemName;

    private void Start()
    {
        Sprite sprite = GetComponent<Image>()?.sprite;
        if (sprite != null && AchievementSystem.Instance != null)
        {
            AchievementSystem.Instance.UnlockAchievement(itemName, sprite);
        }
    }
}
