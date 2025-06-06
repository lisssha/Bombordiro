using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class ItemSpawner : MonoBehaviour
{
    [Header("��������� ������")]
    public GameObject itemPrefab;
    public Transform spawnParent;
    public RectTransform spawnArea;
    public Button spawnButton;

    [Header("��������� ����")]
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

        spawnButton.onClick.RemoveAllListeners();
        spawnButton.onClick.AddListener(TrySpawnItem);
        UpdatePriceDisplay();
    }

    public void TrySpawnItem()
    {
        Debug.Log("����� ������!");
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

    private void SpawnItem()
    {
        int upgradeLevel = GetSpawnUpgradeLevel();
        Debug.Log($"upgradeLevel: {upgradeLevel}");
        int prefabIndex = Mathf.Clamp(upgradeLevel, 0, spawnPrefabs.Length - 1);
        Debug.Log($"prefabIndex: {prefabIndex}");
        GameObject selectedPrefab = spawnPrefabs[prefabIndex];

        Vector2 randomPos = GetRandomPosition();
        GameObject newItem = Instantiate(selectedPrefab, spawnArea);

        newItem.GetComponent<RectTransform>().localScale = Vector3.one;
        newItem.GetComponent<RectTransform>().anchoredPosition = randomPos;

        if (!newItem.TryGetComponent<MonetizableItem>(out _))
        {
            newItem.AddComponent<MonetizableItem>().baseReward = 1f;
        }
    }


    private Vector2 GetRandomPosition()
    {
        RectTransform prefabRect = itemPrefab.GetComponent<RectTransform>();
        Vector2 spawnSize = spawnArea.rect.size;

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
        priceText.text = $"{currentPrice:F1}";
        spawnButton.interactable = GameManager.Instance.CanAfford(currentPrice);

        Debug.Log($"Price updated: {currentPrice}");
    }
    private IEnumerator ShakeButton()
    {
        // �������� ������ ������
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

    [Header("�������")]
    public GameObject[] spawnPrefabs;

    private int GetSpawnUpgradeLevel()
    {
        return PlayerPrefs.GetInt("spawn_upgrade", 0);
    }


    public int GetMaxSpawnedPrefabIndex()
    {
        int maxIndex = -1;

        Item[] itemsOnScene = FindObjectsOfType<Item>();

        foreach (var item in itemsOnScene)
        {
            string name = item.gameObject.name.Replace("(Clone)", "").Trim();

            for (int i = 0; i < spawnPrefabs.Length; i++)
            {
                if (spawnPrefabs[i].name == name)
                {
                    if (i > maxIndex)
                        maxIndex = i;
                }
            }
        }

        return maxIndex;
    }


}