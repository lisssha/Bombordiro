using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }

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
    }

    public void SaveGame()
    {
        var state = new GameState
        {
            money = GameManager.Instance.money,
            spawnPrice = FindObjectOfType<ItemSpawner>().currentPrice,
            prefabs = SavePrefabs()
        };

        PlayerPrefs.SetString("GameSave", JsonUtility.ToJson(state));
        PlayerPrefs.Save();
        Debug.Log("Игра сохранена!");
    }

    private List<PrefabData> SavePrefabs()
    {
        List<PrefabData> prefabs = new List<PrefabData>();
        Item[] items = FindObjectsOfType<Item>();

        foreach (Item item in items)
        {
            prefabs.Add(new PrefabData
            {
                name = item.itemName,
                position = item.transform.position,
                components = SaveComponents(item.gameObject)
            });
        }

        return prefabs;
    }

    private List<ComponentData> SaveComponents(GameObject obj)
    {
        return new List<ComponentData>
        {
            SaveComponent<DraggableItem>(obj),
            SaveComponent<MonetizableItem>(obj)
        };
    }

    private ComponentData SaveComponent<T>(GameObject obj) where T : Component
    {
        T component = obj.GetComponent<T>();
        return new ComponentData
        {
            type = typeof(T).Name,
            data = JsonUtility.ToJson(component)
        };
    }
}
