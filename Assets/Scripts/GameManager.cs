using UnityEngine;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static event System.Action OnMoneyChanged;
    public static GameManager Instance;

    [Header("UI")]
    public Canvas worldCanvas; // Сюда перетащим канвас с FloatingText
    public GameObject floatingTextPrefab;
    public GameObject floatingTextParent;

    [Header("Баланс")]
    private float _money = 1000f;
    public float money
    {
        get => _money;
        set
        {
            _money = value;
            PlayerPrefs.SetFloat("PlayerMoney", value); // Автосохранение
            UpdateUI();
            OnMoneyChanged?.Invoke();
        }
    }
    public TextMeshProUGUI moneyText;

    [Header("Алмазики")]
    private int _gems = 0;
    public int gems
    {
        get => _gems;
        set
        {
            _gems = value;
            PlayerPrefs.SetInt("PlayerGems", value);
            UpdateUI();
        }
    }
    public TextMeshProUGUI gemsText; // Привяжи к UI


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _money = PlayerPrefs.GetFloat("PlayerMoney", 1000f); // Загрузка при старте
            _gems = PlayerPrefs.GetInt("PlayerGems", 0);
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
        ForceUpdateAllSpawners();

        ItemSpawner spawner = FindObjectOfType<ItemSpawner>();
        if (spawner != null) spawner.ForceUpdatePrice();

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

    public void AddGems(int amount)
    {
        gems += amount;
        UpdateUI();
    }

    public bool TrySpendGems(int amount)
    {
        if (_gems >= amount)
        {
            gems -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }


    private void ForceUpdateAllSpawners()
    {
        var spawners = FindObjectsOfType<ItemSpawner>();
        foreach (var spawner in spawners)
        {
            spawner.UpdatePriceDisplay();
        }
    }

    public bool CanAfford(float amount)
    {
        return money >= amount;
    }

    public void UpdateUI()
    {
        if (moneyText != null)
            moneyText.text = $"{money:F1}$";
        if (gemsText != null)
            gemsText.text = $"{gems}&";

    }
    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        // Инициализация UI и других систем
        UpdateUI();

        // Дополнительная загрузка, если нужно
        if (SaveSystem.Instance != null &&
            SceneManager.GetActiveScene().name == "GameArea")
        {
            SaveSystem.Instance.LoadGame();
        }
    }

    [ContextMenu("Add Test Money")]
    public void AddTestMoney()
    {
        money = 1000f; // Установите нужную сумму
        UpdateUI();
        Debug.Log($"Деньги установлены: {money}");
    }


}