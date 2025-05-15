using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;

    [Header("Ограничение области")]
    public RectTransform boundaryArea; 

    [Header("Аудио(не делала)")]
    public AudioClip pickupSound;
    public AudioClip dropSound;
    private AudioSource audioSource;

    [Header("Визуальные эффекты")]
    public float scaleOnDrag = 1.2f;
    private Vector3 originalScale;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        audioSource = GetComponent<AudioSource>();
        originalScale = transform.localScale;
    }


    void Start()
    {
        if (boundaryArea == null)
        {
            GameObject boundaryGO = GameObject.Find("SpawnArena");
            if (boundaryGO != null)
            {
                boundaryArea = boundaryGO.GetComponent<RectTransform>();
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.7f;
        canvasGroup.blocksRaycasts = false;

        transform.localScale = originalScale * scaleOnDrag;

        if (audioSource && pickupSound)
            audioSource.PlayOneShot(pickupSound);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

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
    [Header("какой префаб будет если соеденить")]
    public GameObject resultPrefab; 
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        transform.localScale = originalScale;

        if (audioSource && dropSound)
            audioSource.PlayOneShot(dropSound);

        Item thisItem = GetComponent<Item>();
        if (thisItem == null) return;

        foreach (var other in GameObject.FindGameObjectsWithTag("Item"))
        {
            if (other == this.gameObject) continue;

            float distance = Vector3.Distance(transform.position, other.transform.position);
            if (distance < 100f) // это на каком расстоянии элементы и если онр меньше то они стакаются
            {
                Item otherItem = other.GetComponent<Item>();
                if (otherItem != null && otherItem.itemName == thisItem.itemName)
                {
                    
                    Vector3 spawnPos = (transform.position + other.transform.position) / 2;
                    Destroy(other);
                    Destroy(gameObject);

                    // новый объект
                    Instantiate(resultPrefab, spawnPos, Quaternion.identity, canvas.transform);

                    break;
                }
            }
        }
    }
    }