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

    // Этот метод вызывается в конце анимации через Animation Event
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
