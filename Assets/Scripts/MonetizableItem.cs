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

    public float ClickIncomeMultiplier => 1f + PlayerPrefs.GetInt("click_income", 0);
    public void OnPointerClick(PointerEventData eventData)
    {
        float reward = baseReward * Mathf.Pow(rewardMultiplier, evolutionLevel) * ClickIncomeMultiplier;
        GameManager.Instance.AddMoney(reward);

        

        // Шанс выпадения алмазиков (5%)
        if (Random.value < 0.05f)
        {
            int gemsEarned = 1 + PlayerPrefs.GetInt("click_income", 0);
            GameManager.Instance.AddGems(gemsEarned);
            ShowFloatingText($"+{reward:F1}$", Color.blue);
        }
        else
        {
            ShowFloatingText($"+{reward:F1}$", Color.white);
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

        // 1. Получаем экранную позицию (куда кликнули/где животное)
        Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);

        // 2. Переводим экранную позицию в локальную позицию внутри Canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            GameManager.Instance.floatingTextParent.GetComponent<RectTransform>(),
            screenPos,
            null, // null, потому что Overlay-режим — камера не используется
            out Vector2 localPoint
        );

        // 3. Создаём текст и устанавливаем его локальную позицию
        GameObject ft = Instantiate(
            GameManager.Instance.floatingTextPrefab,
            GameManager.Instance.floatingTextParent.transform
        );

        ft.GetComponent<RectTransform>().anchoredPosition = localPoint;
        
        FloatingText floatingText = ft.GetComponent<FloatingText>();
        floatingText.SetText(value, color);
    }








}