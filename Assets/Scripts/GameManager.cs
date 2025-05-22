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
}