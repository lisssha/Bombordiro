using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSaveData
{
    public float money;
    public float currentSpawnPrice;
    public List<SavedPrefab> prefabs = new List<SavedPrefab>();
}

[System.Serializable]
public class SavedPrefab
{
    public string prefabName;
    public Vector3 position;
    public float rotation;
    public Vector3 scale;

    // Компоненты
    public ItemData itemData;
    public DraggableItemData draggableData;
    public MonetizableItemData monetizableData;
}

[System.Serializable]
public class ItemData
{
    public string itemName;
}

[System.Serializable]
public class DraggableItemData
{
    public int evolutionLevel;
}

[System.Serializable]
public class MonetizableItemData
{
    public float baseReward;
    public float rewardMultiplier;
}