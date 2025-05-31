using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementEntryUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text dateText;

    public void Setup(AchievementData data)
    {
        titleText.text = data.name;
        dateText.text = data.dateUnlocked;

        // ѕытаемс€ загрузить спрайт из ресурсов по имени
        Sprite sprite = Resources.Load<Sprite>(data.spritePath);
        if (sprite != null)
        {
            icon.sprite = sprite;
        }
    }
}
