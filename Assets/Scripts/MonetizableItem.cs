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
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("GameManager.Instance is null");
            return;
        }

        if (GameManager.Instance.worldCanvas == null)
        {
            Debug.LogWarning("GameManager.Instance.worldCanvas is null");
            return;
        }

        if (GameManager.Instance.floatingTextPrefab == null)
        {
            Debug.LogWarning("floatingTextPrefab is null in GameManager");
            return;
        }

        Vector3 screenPos = Input.mousePosition;
        GameObject ft = Instantiate(GameManager.Instance.floatingTextPrefab, screenPos, Quaternion.identity, GameManager.Instance.floatingTextParent.transform);


        FloatingText floatingText = ft.GetComponent<FloatingText>();
        floatingText.SetText(value, color);
    }

}