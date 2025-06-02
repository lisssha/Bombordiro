using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public class MonetizableItem : MonoBehaviour, IPointerClickHandler
{
    [Header("��������� ������")]
    [SerializeField] public float baseReward = 1f;
    [SerializeField] public float rewardMultiplier = 1.1f;

    [SerializeField] private int evolutionLevel = 0;

    [SerializeField] public Canvas worldCanvas; // ������� World-Space Canvas


    public void OnPointerClick(PointerEventData eventData)
    {
        float reward = baseReward * Mathf.Pow(rewardMultiplier, evolutionLevel);
        GameManager.Instance.AddMoney(reward);

        

        // ���� ��������� ��������� (5%)
        if (Random.value < 0.05f)
        {
            int gemsEarned = Mathf.Max(1, evolutionLevel + 1); // 1 ��� ������ ������
            GameManager.Instance.AddGems(gemsEarned);
            ShowFloatingText($"+{gemsEarned} +{reward:F1}", Color.blue);
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

        // 1. �������� �������� ������� (���� ��������/��� ��������)
        Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);

        // 2. ��������� �������� ������� � ��������� ������� ������ Canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            GameManager.Instance.floatingTextParent.GetComponent<RectTransform>(),
            screenPos,
            null, // null, ������ ��� Overlay-����� � ������ �� ������������
            out Vector2 localPoint
        );

        // 3. ������ ����� � ������������� ��� ��������� �������
        GameObject ft = Instantiate(
            GameManager.Instance.floatingTextPrefab,
            GameManager.Instance.floatingTextParent.transform
        );

        ft.GetComponent<RectTransform>().anchoredPosition = localPoint;
        
        FloatingText floatingText = ft.GetComponent<FloatingText>();
        floatingText.SetText(value, color);
    }








}