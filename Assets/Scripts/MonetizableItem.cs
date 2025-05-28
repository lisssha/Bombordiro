using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public class MonetizableItem : MonoBehaviour, IPointerClickHandler
{
    [Header("Настройки дохода")]
    [SerializeField] public float baseReward = 1f;
    [SerializeField] public float rewardMultiplier = 1.1f;

    [SerializeField] private int evolutionLevel = 0;

    public void OnPointerClick(PointerEventData eventData)
    {
        float reward = baseReward * Mathf.Pow(rewardMultiplier, evolutionLevel);
        GameManager.Instance.AddMoney(reward);

        // Не нужно явно обновлять кнопки - GameManager сделает это автоматически
    }

    public void SetEvolutionLevel(int level)
    {
        evolutionLevel = level;
    }

    private void ShowFloatingText(string text)
    {
        // Реализация всплывающего текста (можно использовать TextMeshPro)
        Debug.Log(text); // Временная реализация
    }
}