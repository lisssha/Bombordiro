using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSaveData
{
    public float money;
    public float currentSpawnPrice;
    public List<AnimalSaveData> animals = new List<AnimalSaveData>();
}

[System.Serializable]
public class AnimalSaveData
{
    public string itemName;
    public Vector2 position;
    public int evolutionLevel;
}
