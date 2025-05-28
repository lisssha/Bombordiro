using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameState
{
    public float money;
    public float spawnPrice;
    public List<PrefabData> prefabs;
}

[System.Serializable]
public class PrefabData
{
    public string prefabName;
    public Vector3 position;
    public float rotation;
    public Vector3 scale;
}