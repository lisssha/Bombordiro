using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Animator animator;

    public void SetText(string value, Color color)
    {
        textMesh.text = value;
        textMesh.color = color;
        animator.Play("FloatingText");
    }

    // ���� ����� ���������� � ����� �������� ����� Animation Event
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
