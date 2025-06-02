using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public class MonetizableItem : MonoBehaviour, IPointerClickHandler
{
    [Header("Настройки дохода")]
    [SerializeField] public float baseReward = 1f;
    [SerializeField] public float rewardMultiplier = 1.1f;

    [SerializeField] private int evolutionLevel = 0;

    [SerializeField] public Canvas worldCanvas; // Привяжи World-Space Canvas


    public void OnPointerClick(PointerEventData eventData)
    {
        float reward = baseReward * Mathf.Pow(rewardMultiplier, evolutionLevel);
        GameManager.Instance.AddMoney(reward);
        ShowFloatingText($"+{reward:F1}$", Color.white);

        // Шанс выпадения алмазиков (5%)
        if (Random.value < 0.05f)
        {
            int gemsEarned = Mathf.Max(1, evolutionLevel + 1); // 1 для первой стадии
            GameManager.Instance.AddGems(gemsEarned);
            ShowFloatingText($"+{gemsEarned}", Color.blue);
        }
    }
    public void SetEvolutionLevel(int level)
    {
        evolutionLevel = level;
    }

    private void ShowFloatingText(string value, Color color)
    {
        if (GameManager.Instance == null ||
            GameManager.Instance.worldCanvas == null ||
            GameManager.Instance.floatingTextPrefab == null)
            return;

        // Экранная позиция мыши (где был клик)
        Vector2 screenPos = Input.mousePosition;

        // Переводим экранные координаты в мировые (в Canvas)
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            GameManager.Instance.worldCanvas.GetComponent<RectTransform>(),
            screenPos,
            Camera.main,
            out Vector3 worldPos
        );

        // Создаём объект во worldCanvas в нужной позиции
        GameObject ft = Instantiate(
            GameManager.Instance.floatingTextPrefab,
            worldPos,
            Quaternion.identity,
            GameManager.Instance.floatingTextParent.transform
        );

        FloatingText floatingText = ft.GetComponent<FloatingText>();
        floatingText.SetText(value, color);
    }


}