using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static event System.Action OnMoneyChanged;
    public static GameManager Instance;

    [Header("Баланс")]
    public float money = 1000f;
    public TextMeshProUGUI moneyText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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