using UnityEngine;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    public static event System.Action OnMoneyChanged;
    public static GameManager Instance;

    [Header("Баланс")]
    public float money = 500f; // Стартовые деньги
    public TextMeshProUGUI moneyText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        UpdateUI();
    }

    public void AddMoney(float amount)
    {
        money += amount;
        UpdateUI();
        OnMoneyChanged?.Invoke();
        ForceUpdateAllSpawners(); // Добавляем эту строку

        ItemSpawner spawner = FindObjectOfType<ItemSpawner>();
        if (spawner != null) spawner.ForceUpdatePrice();

    }

    private void ForceUpdateAllSpawners()
    {
        var spawners = FindObjectsOfType<ItemSpawner>();
        foreach (var spawner in spawners)
        {
            spawner.UpdatePriceDisplay();
        }
    }

    public bool TrySpendMoney(float amount)
    {
        if (CanAfford(amount))
        {
            money -= amount;
            UpdateUI();
            OnMoneyChanged?.Invoke();
            return true;
        }
        return false;
    }

    public bool CanAfford(float amount)
    {
        return money >= amount;
    }

    private void UpdateUI()
    {
        if (moneyText != null)
            moneyText.text = $"Деньги: {money:F1}$";
    }
    
    private float saveTimer = 0f;
    private const float SAVE_INTERVAL = 60f; // 60 секунд = 1 минута

    private void Update()
    {
        saveTimer += Time.deltaTime;
        if (saveTimer >= SAVE_INTERVAL)
        {
            SaveGame();
            saveTimer = 0f;
        }
    }

    public void SaveGame()
    {
        GameSaveData saveData = new GameSaveData();
        saveData.money = money;

        // Сохраняем текущую цену спавна
        ItemSpawner spawner = FindObjectOfType<ItemSpawner>();
        if (spawner != null)
        {
            saveData.currentSpawnPrice = spawner.currentPrice;
        }

        // Сохраняем всех животных
        Item[] allAnimals = FindObjectsOfType<Item>();
        foreach (Item animal in allAnimals)
        {
            AnimalSaveData animalData = new AnimalSaveData
            {
                itemName = animal.itemName,
                position = animal.transform.position,
                evolutionLevel = animal.GetComponent<DraggableItem>().GetEvolutionLevel()
            };
            saveData.animals.Add(animalData);
        }

        string json = JsonUtility.ToJson(saveData);
        string savePath = Application.persistentDataPath + "/gamesave.json";
        File.WriteAllText(savePath, json);

        Debug.Log("Игра сохранена!");
    }

    public void LoadGame()
    {
        string savePath = Application.persistentDataPath + "/gamesave.json";
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);

            // Восстанавливаем деньги
            money = saveData.money;
            UpdateUI();

            // Восстанавливаем цену спавна
            ItemSpawner spawner = FindObjectOfType<ItemSpawner>();
            if (spawner != null)
            {
                spawner.currentPrice = saveData.currentSpawnPrice;
                spawner.UpdatePriceDisplay();
            }

            // Удаляем текущих животных
            Item[] existingAnimals = FindObjectsOfType<Item>();
            foreach (Item animal in existingAnimals)
            {
                Destroy(animal.gameObject);
            }

            // Восстанавливаем животных
            foreach (AnimalSaveData animalData in saveData.animals)
            {
                GameObject prefab = Resources.Load<GameObject>(animalData.itemName);
                if (prefab != null)
                {
                    GameObject animal = Instantiate(prefab, animalData.position, Quaternion.identity);
                    animal.GetComponent<Item>().itemName = animalData.itemName;

                    DraggableItem draggable = animal.GetComponent<DraggableItem>();
                    if (draggable != null)
                    {
                        draggable.SetEvolutionLevel(animalData.evolutionLevel);
                    }
                }
            }

            Debug.Log("Игра загружена!");
        }
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private void Start()
    {
        LoadGame();
    }
}