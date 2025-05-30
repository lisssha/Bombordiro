using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSaveData
{
    public float money; // Добавляем поле для денег
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
}