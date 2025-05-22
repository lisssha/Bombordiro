using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(Item))]
[RequireComponent(typeof(BoxCollider2D))] // ��������� ������������ ���������
public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // ����������
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;
    private AudioSource audioSource;
    private BoxCollider2D boxCollider;

    [Header("Drag Settings")]
    public RectTransform boundaryArea;
    public float scaleOnDrag = 1.2f;
    private Vector3 originalScale;

    [Header("Audio Settings")]
    public AudioClip pickupSound;
    public AudioClip dropSound;

    [Header("Merge Settings")]
    public EvolutionData evolutionData;
    public float mergeDistance = 100f;
    private EvolutionData.EvolutionStage currentStage;

    private void Awake()
    {
        // ������������� �����������
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        audioSource = GetComponent<AudioSource>();
        boxCollider = GetComponent<BoxCollider2D>();

        // ��������� AudioSource
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        // ��������� ����������
        if (boxCollider != null)
        {
            boxCollider.isTrigger = false;
            boxCollider.size = rectTransform.rect.size;
        }

        originalScale = transform.localScale;
    }

    private void Start()
    {
        // ����� ������
        if (boundaryArea == null)
        {
            boundaryArea = GameObject.Find("SpawnArena")?.GetComponent<RectTransform>();
        }

        // ����� ������� ������ ��������
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
            FindCurrentEvolutionStage(); // ��������� ����� �����
            if (currentStage == null) return;
        }

        // ���� ��� ���������� � �������
        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(
            transform.position,
            mergeDistance * canvas.scaleFactor // ��������� ������� �������
        );

        foreach (var collider in nearbyColliders)
        {
            if (collider.gameObject == gameObject) continue;

            Item otherItem = collider.GetComponent<Item>();
            if (otherItem != null && otherItem.itemName == thisItem.itemName)
            {
                MergeItems(otherItem.gameObject);
                return; // ������� ����� ������� �������
            }
        }
    }

    private void MergeItems(GameObject other)
    {
        Debug.Log($"Merging {gameObject.name} with {other.name}");

        Vector3 spawnPos = (transform.position + other.transform.position) / 2f;

        // ��������������� �����
        PlayMergeSound();

        // �������� ������ �������
        if (currentStage.nextStage != null)
        {
            var newItem = Instantiate(
                currentStage.nextStage,
                spawnPos,
                Quaternion.identity,
                canvas.transform
            );

            Debug.Log($"Created new {newItem.name}");
        }

        // ����������� ��������
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
}