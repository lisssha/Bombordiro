using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(Item))]
[RequireComponent(typeof(BoxCollider2D))] // Добавляем обязательный коллайдер
[System.Serializable]
public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // Компоненты
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Canvas canvas;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private BoxCollider2D boxCollider;

    [Header("Drag Settings")]
    [SerializeField] public RectTransform boundaryArea;
    [SerializeField] public float scaleOnDrag = 1.2f;
    [SerializeField] private Vector3 originalScale;

    [Header("Audio Settings")]
    [SerializeField] public AudioClip pickupSound;
    [SerializeField] public AudioClip dropSound;

    [Header("Merge Settings")]
    [SerializeField] public EvolutionData evolutionData;
    [SerializeField] public float mergeDistance = 100f;
    [SerializeField] private EvolutionData.EvolutionStage currentStage;

    private void Awake()
    {
        // Инициализация компонентов
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        audioSource = GetComponent<AudioSource>();
        boxCollider = GetComponent<BoxCollider2D>();

        // Настройка AudioSource
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        // Настройка коллайдера
        if (boxCollider != null)
        {
            boxCollider.isTrigger = false;
            boxCollider.size = rectTransform.rect.size;
        }

        originalScale = transform.localScale;
    }

    private void Start()
    {
        // Поиск границ
        if (boundaryArea == null)
        {
            boundaryArea = GameObject.Find("SpawnArena")?.GetComponent<RectTransform>();
        }

        // Поиск текущей стадии эволюции
        FindCurrentEvolutionStage();
    }

    private void FindCurrentEvolutionStage()
    {
        if (evolutionData != null)
        {
            currentStage = evolutionData.GetStageForPrefab(gameObject);
        }
    }

    #region Drag Handlers
    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.7f;
        canvasGroup.blocksRaycasts = false;
        transform.localScale = originalScale * scaleOnDrag;
        PlaySound(pickupSound);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        ClampToBoundary();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        transform.localScale = originalScale;
        PlaySound(dropSound);
        TryMergeItems();
    }
    #endregion

    private void ClampToBoundary()
    {
        if (boundaryArea == null) return;

        Vector3[] itemCorners = new Vector3[4];
        Vector3[] boundaryCorners = new Vector3[4];

        rectTransform.GetWorldCorners(itemCorners);
        boundaryArea.GetWorldCorners(boundaryCorners);

        Vector3 offset = Vector3.zero;

        for (int i = 0; i < 4; i++)
        {
            if (itemCorners[i].x < boundaryCorners[0].x)
                offset.x += boundaryCorners[0].x - itemCorners[i].x;
            if (itemCorners[i].x > boundaryCorners[2].x)
                offset.x -= itemCorners[i].x - boundaryCorners[2].x;

            if (itemCorners[i].y < boundaryCorners[0].y)
                offset.y += boundaryCorners[0].y - itemCorners[i].y;
            if (itemCorners[i].y > boundaryCorners[2].y)
                offset.y -= itemCorners[i].y - boundaryCorners[2].y;
        }

        rectTransform.position += offset;
    }

    private void TryMergeItems()
    {
        Item thisItem = GetComponent<Item>();
        if (thisItem == null)
        {
            Debug.LogWarning("Item component missing!");
            return;
        }

        if (currentStage == null)
        {
            Debug.LogWarning("Evolution stage not found!");
            FindCurrentEvolutionStage(); // Попробуем найти снова
            if (currentStage == null) return;
        }

        // Ищем все коллайдеры в радиусе
        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(
            transform.position,
            mergeDistance * canvas.scaleFactor // Учитываем масштаб канваса
        );

        foreach (var collider in nearbyColliders)
        {
            if (collider.gameObject == gameObject) continue;

            Item otherItem = collider.GetComponent<Item>();
            if (otherItem != null && otherItem.itemName == thisItem.itemName)
            {
                MergeItems(otherItem.gameObject);
                return; // Выходим после первого слияния
            }
        }
    }

    private void MergeItems(GameObject other)
    {
        Vector3 spawnPos = (transform.position + other.transform.position) / 2f;

        if (currentStage.nextStage != null)
        {
            var newItem = Instantiate(
                currentStage.nextStage,
                spawnPos,
                Quaternion.identity,
                canvas.transform
            );

            // Фиксируем масштаб
            newItem.GetComponent<RectTransform>().localScale = Vector3.one;

            // Воспроизводим звук слияния
            PlayMergeSound();
        }

        StartCoroutine(DestroyAfterDelay(other));
    }
    private System.Collections.IEnumerator DestroyAfterDelay(GameObject other)
    {
        yield return new WaitForSeconds(0.05f);
        Destroy(other);
        Destroy(gameObject);
    }

    private void PlayMergeSound()
    {
        if (currentStage.mergeSound == null) return;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound(currentStage.mergeSound);
        }
        else if (audioSource != null)
        {
            audioSource.PlayOneShot(currentStage.mergeSound);
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(
            transform.position,
            mergeDistance * (canvas != null ? canvas.scaleFactor : 1)
        );
    }

    private void ShowFloatingText(string text)
    {
        // Реализация всплывающего текста (можно использовать TextMeshPro)
    }
}