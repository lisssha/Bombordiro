using UnityEngine;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;


public class GameManager : MonoBehaviour
{
    public static event System.Action OnMoneyChanged;
    public static GameManager Instance;

    [Header("������")]
    public float money = 1000f; // ��������� ������
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
        ForceUpdateAllSpawners(); // ��������� ��� ������

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
            moneyText.text = $"������: {money:F1}$";
    }
    private void Start()
    {
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.LoadGame();
        }
        UpdateUI();
    }

    [ContextMenu("Add Test Money")]
    public void AddTestMoney()
    {
        money = 1000f; // ���������� ������ �����
        UpdateUI();
        Debug.Log($"������ �����������: {money}");
    }


}