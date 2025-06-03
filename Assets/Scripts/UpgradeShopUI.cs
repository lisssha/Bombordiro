using UnityEngine;
using System.Collections.Generic;

public class UpgradeShopUI : MonoBehaviour
{
    public static UpgradeShopUI Instance { get; private set; }

    [System.Serializable]
    public class UpgradeData
    {
        public string upgradeKey;
        public string upgradeName;
        public int baseCost;
        public float costMultiplier = 1.5f;
    }

    private void Awake()
    {
        Instance = this;
    }

    public GameObject upgradeItemPrefab;
    public Transform upgradePanel;

    private List<UpgradeData> upgrades = new List<UpgradeData>
    {
        new UpgradeData { upgradeKey = "spawn_upgrade", upgradeName = "Spawn Upgrade", baseCost = 10 },
        new UpgradeData { upgradeKey = "click_income", upgradeName = "Click Income", baseCost = 20 }
    };

    private List<UpgradeItemUI> upgradeItems = new List<UpgradeItemUI>();
    private void Start()
    {
        foreach (var upgrade in upgrades)
        {
            GameObject ui = Instantiate(upgradeItemPrefab, upgradePanel);
            UpgradeItemUI itemUI = ui.GetComponent<UpgradeItemUI>();
            itemUI.Init(upgrade.upgradeKey, upgrade.upgradeName, upgrade.baseCost, upgrade.costMultiplier);
            upgradeItems.Add(itemUI);
        }
    }

    public void UpdateAllItems()
    {
        foreach (var item in upgradeItems)
            item.UpdateUI();
    }


    public GameObject upgradePanelObject; // —юда передаЄм саму UpgradePanel

    public void TogglePanel()
    {
        bool isActive = upgradePanelObject.activeSelf;
        upgradePanelObject.SetActive(!isActive);
        UpgradeItemUI.Instance.UpdateUI();
    }

    public void OpenPanel()
    {
        upgradePanelObject.SetActive(true);
    }

    public void ClosePanel()
    {
        upgradePanelObject.SetActive(false);
    }
}
