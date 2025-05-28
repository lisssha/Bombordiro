using UnityEngine;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

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

    public void UpdateUI()
    {
        if (moneyText != null)
            moneyText.text = $"Деньги: {money:F1}$";
    }
    private void Start()
    {
        if (SaveSystem.Instance != null)
        {
            string saveData = PlayerPrefs.GetString("GameSave");
            if (!string.IsNullOrEmpty(saveData))
            {
                GameState state = JsonUtility.FromJson<GameState>(saveData);
                money = state.money;
                FindObjectOfType<ItemSpawner>().currentPrice = state.spawnPrice;

                if (state.prefabs != null) // Добавляем проверку на null
                {
                    LoadPrefabs(state.prefabs);
                }
            }
        }
        UpdateUI();
    }

    private void LoadPrefabs(List<PrefabData> prefabsData)
    {
        // Удаляем существующие объекты
        Item[] existingItems = FindObjectsOfType<Item>();
        foreach (Item item in existingItems)
        {
            Destroy(item.gameObject);
        }

        // Создаем сохраненные объекты
        foreach (PrefabData prefabData in prefabsData)
        {
            GameObject prefab = Resources.Load<GameObject>(prefabData.name);
            if (prefab != null)
            {
                GameObject instance = Instantiate(prefab, prefabData.position, Quaternion.identity);

                // Восстанавливаем компоненты
                foreach (ComponentData componentData in prefabData.components)
                {
                    LoadComponent(instance, componentData);
                }
            }
        }
    }

    private void LoadComponent(GameObject obj, ComponentData componentData)
    {
        System.Type componentType = System.Type.GetType(componentData.type);
        if (componentType != null)
        {
            Component component = obj.GetComponent(componentType);
            if (component != null)
            {
                JsonUtility.FromJsonOverwrite(componentData.data, component);
            }
        }
    }

    private void OnApplicationQuit()
    {

    }
}