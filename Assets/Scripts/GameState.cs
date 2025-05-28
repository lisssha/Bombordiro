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
    public string name;
    public Vector2 position;
    public List<ComponentData> components;
}

[System.Serializable]
public class ComponentData
{
    public string type;
    public string data;
}