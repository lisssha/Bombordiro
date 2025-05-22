using UnityEngine;
using UnityEngine.EventSystems;

public class MonetizableItem : MonoBehaviour, IPointerClickHandler
{
    [Header("��������� ������")]
    public float baseReward = 1f;
    public float rewardMultiplier = 1.1f;

    private int evolutionLevel = 0;

    public void OnPointerClick(PointerEventData eventData)
    {
        float reward = baseReward * Mathf.Pow(rewardMultiplier, evolutionLevel);
        GameManager.Instance.AddMoney(reward);

        // �� ����� ���� ��������� ������ - GameManager ������� ��� �������������
    }

    public void SetEvolutionLevel(int level)
    {
        evolutionLevel = level;
    }

    private void ShowFloatingText(string text)
    {
        // ���������� ������������ ������ (����� ������������ TextMeshPro)
        Debug.Log(text); // ��������� ����������
    }
}