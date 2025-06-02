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
        animator.Play("Float"); // Название твоего аниматора. Убедись, что оно такое.
    }

    // Этот метод вызывается в конце анимации через Animation Event
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
