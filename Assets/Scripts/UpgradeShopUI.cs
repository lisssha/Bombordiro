using UnityEngine;
using System.Collections.Generic;

public class UpgradeShopUI : MonoBehaviour
{
    [System.Serializable]
    public class UpgradeData
    {
        public string upgradeKey;
        public string upgradeName;
        public int baseCost;
        public float costMultiplier = 1.5f;
    }

    public GameObject upgradeItemPrefab;
    public Transform upgradePanel;

    public List<UpgradeData> upgrades = new List<UpgradeData>
    {
        new UpgradeData { upgradeKey = "spawn_upgrade", upgradeName = "Spawn Upgrade", baseCost = 10 },
        new UpgradeData { upgradeKey = "click_income", upgradeName = "Click Income", baseCost = 20 }
    };

    private void Start()
    {
        foreach (var upgrade in upgrades)
        {
            GameObject ui = Instantiate(upgradeItemPrefab, upgradePanel);
            UpgradeItemUI itemUI = ui.GetComponent<UpgradeItemUI>();
            itemUI.Init(upgrade.upgradeKey, upgrade.upgradeName, upgrade.baseCost, upgrade.costMultiplier);
        }
    }
}
