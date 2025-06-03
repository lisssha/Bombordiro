using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeItemUI : MonoBehaviour
{
    public static UpgradeItemUI Instance;

    [Header("UI")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI costText;
    public Button upgradeButton;

    [Header("Настройки улучшения")]
    public string upgradeKey;
    public string upgradeName;
    public int baseCost;
    public float costMultiplier;

    private int currentLevel;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        var texts = GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (var txt in texts)
        {
            if (txt.name.Contains("Name")) nameText = txt;
            else if (txt.name.Contains("Level")) levelText = txt;
            else if (txt.name.Contains("Cost")) costText = txt;
        }

        upgradeButton = GetComponentInChildren<Button>(true);
        upgradeButton.onClick.AddListener(BuyUpgrade);

        UpdateUI();
    }

    public void Init(string key, string name, int baseCost, float costMultiplier)
    {
        Debug.Log($"Init called: {name}");

        this.upgradeKey = key;
        this.upgradeName = name;
        this.baseCost = baseCost;
        this.costMultiplier = costMultiplier;

        currentLevel = PlayerPrefs.GetInt(upgradeKey, 0);
        UpdateUI();
    }

    public void UpdateUI()
    {
        int cost = GetCurrentCost();

        nameText.text = upgradeName;
        levelText.text = $"Level: {currentLevel}";
        costText.text = $"Price: {cost}&";

        upgradeButton.interactable = GameManager.Instance.gems >= cost;

        //if (upgradeKey == "spawn_upgrade")
        //{
        //    ItemSpawner spawner = FindObjectOfType<ItemSpawner>();
        //    int maxAvailableLevel = spawner.GetMaxSpawnedPrefabIndex();
        //
        //    if (currentLevel >= maxAvailableLevel)
        //   {
        //        upgradeButton.interactable = false;
        //        Debug.Log($"Прокачка {upgradeKey} заблокирована. Максимальный уровень: {maxAvailableLevel}");
        //    }
        //}
    }

    int GetCurrentCost()
    {
        return Mathf.RoundToInt(baseCost * Mathf.Pow(costMultiplier, currentLevel));
    }

    void BuyUpgrade()
    {
        if (upgradeKey == "spawn_upgrade")
        {
            ItemSpawner spawner = FindObjectOfType<ItemSpawner>();
            int maxAvailableLevel = spawner.GetMaxSpawnedPrefabIndex();

            if (currentLevel + 1 > maxAvailableLevel)
            {
                Debug.Log($"Нельзя прокачать выше уровня {maxAvailableLevel}. Сначала получи животное через слияние.");
                return;
            }
        }

        int cost = GetCurrentCost();

        if (GameManager.Instance.TrySpendGems(cost))
        {
            currentLevel++;
            PlayerPrefs.SetInt(upgradeKey, currentLevel);
            UpdateUI();
            ApplyEffect();

            UpgradeShopUI.Instance?.UpdateAllItems();
        }
    }

    void ApplyEffect()
    {
        Debug.Log($"Улучшение '{upgradeName}' применено. Новый уровень: {currentLevel}");
        // Тут можешь вызывать, например:
        // UpgradeManager.Instance.ApplyUpgrade(upgradeKey, currentLevel);
    }
}
