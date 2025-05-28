using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class ItemSpawner : MonoBehaviour
{
    [Header("Настройки спавна")]
    public GameObject itemPrefab;
    public Transform spawnParent;
    public RectTransform spawnArea;
    public Button spawnButton;

    [Header("Настройки цены")]
    public TextMeshProUGUI priceText;
    public float basePrice = 100f;
    public float priceIncreaseMultiplier = 1.15f;

    public float currentPrice
    {
        get => PlayerPrefs.GetFloat("CurrentSpawnPrice", basePrice);
        set => PlayerPrefs.SetFloat("CurrentSpawnPrice", value);
    }
    private GameManager gameManager;
    private bool isSpawning = false;

    private void Start()
    {
        gameManager = GameManager.Instance;
        // currentPrice теперь загружается автоматически через свойство

        spawnButton.onClick.RemoveAllListeners();
        spawnButton.onClick.AddListener(TrySpawnItem);
        UpdatePriceDisplay();
    }

    public void TrySpawnItem()
    {
        Debug.Log("Спавн вызван!");
        if (isSpawning || !gameManager.CanAfford(currentPrice)) return;
        StartCoroutine(SpawnWithCooldown());
    }

    private IEnumerator SpawnWithCooldown()
    {
        isSpawning = true;

        if (gameManager.TrySpendMoney(currentPrice))
        {
            SpawnItem();
            currentPrice *= priceIncreaseMultiplier;
            UpdatePriceDisplay();
        }
        else
        {
            StartCoroutine(ShakeButton());
        }

        yield return new WaitForSeconds(0.2f);
        isSpawning = false;
    }

    // В ItemSpawner.cs измените метод SpawnItem:
    private void SpawnItem()
    {
        Vector2 randomPos = GetRandomPosition();
        GameObject newItem = Instantiate(itemPrefab, spawnArea);

        // Устанавливаем стандартный масштаб
        newItem.GetComponent<RectTransform>().localScale = Vector3.one;
        newItem.GetComponent<RectTransform>().anchoredPosition = randomPos;

        // Добавляем компонент для заработка
        if (!newItem.TryGetComponent<MonetizableItem>(out _))
        {
            newItem.AddComponent<MonetizableItem>().baseReward = 1f;
        }
    }

    private Vector2 GetRandomPosition()
    {
        RectTransform prefabRect = itemPrefab.GetComponent<RectTransform>();
        Vector2 spawnSize = spawnArea.rect.size;

        // Учитываем pivot префаба
        Vector2 pivotOffset = new Vector2(
            prefabRect.pivot.x * prefabRect.rect.width,
            prefabRect.pivot.y * prefabRect.rect.height
        );

        return new Vector2(
            Random.Range(-spawnSize.x / 2 + pivotOffset.x, spawnSize.x / 2 - pivotOffset.x),
            Random.Range(-spawnSize.y / 2 + pivotOffset.y, spawnSize.y / 2 - pivotOffset.y)
        );
    }

    public void UpdateButtonState()
    {
        UpdatePriceDisplay();
    }

    
    public void ForceUpdatePrice()
    {
        UpdatePriceDisplay();
    }

    public void UpdatePriceDisplay()
    {
        // Всегда обновляем текст и состояние кнопки
        priceText.text = $"{currentPrice:F1}$";
        spawnButton.interactable = GameManager.Instance.CanAfford(currentPrice);

        // Добавьте лог для отладки
        Debug.Log($"Price updated: {currentPrice}");
    }
    private IEnumerator ShakeButton()
    {
        // Анимация тряски кнопки
        Vector3 originalPos = spawnButton.transform.position;
        float duration = 0.5f;
        float magnitude = 10f;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            spawnButton.transform.position = originalPos + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        spawnButton.transform.position = originalPos;
    }

    private void OnEnable()
    {
        GameManager.OnMoneyChanged += UpdatePriceDisplay;
    }

    private void OnDisable()
    {
        GameManager.OnMoneyChanged -= UpdatePriceDisplay;
    }
}