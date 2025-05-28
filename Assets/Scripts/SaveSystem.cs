using System.Collections.Generic;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }

    [SerializeField] private List<GameObject> availablePrefabs;
    [SerializeField] private Transform defaultParent; // Перетащите сюда SpawnArena из иерархии

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
        var saveData = new GameSaveData
        {
            money = GameManager.Instance.money,
            currentSpawnPrice = FindObjectOfType<ItemSpawner>().currentPrice,
            prefabs = new List<SavedPrefab>()
        };

        foreach (Item item in FindObjectsOfType<Item>())
        {
            if (item.GetComponent<ItemSpawner>() != null) continue;

            saveData.prefabs.Add(new SavedPrefab
            {
                prefabName = item.itemName,
                position = item.transform.localPosition, // Используем localPosition
                rotation = item.transform.localEulerAngles.z,
                scale = item.transform.localScale
            });
        }

        PlayerPrefs.SetString("GameSave", JsonUtility.ToJson(saveData));
        PlayerPrefs.Save();
        Debug.Log("Игра сохранена");
    }

    public void LoadGame()
    {
        if (!PlayerPrefs.HasKey("GameSave")) return;

        string json = PlayerPrefs.GetString("GameSave");
        GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);

        GameManager.Instance.money = saveData.money;
        FindObjectOfType<ItemSpawner>().currentPrice = saveData.currentSpawnPrice;

        // Очищаем старые префабы
        foreach (Item item in FindObjectsOfType<Item>())
        {
            if (item.GetComponent<ItemSpawner>() == null)
                Destroy(item.gameObject);
        }

        // Создаем новые префабы
        foreach (SavedPrefab prefabData in saveData.prefabs)
        {
            GameObject prefab = availablePrefabs.Find(p =>
                p.GetComponent<Item>().itemName == prefabData.prefabName);

            if (prefab != null && defaultParent != null)
            {
                GameObject instance = Instantiate(
                    prefab,
                    defaultParent,
                    false
                );

                instance.transform.localPosition = prefabData.position;
                instance.transform.localEulerAngles = new Vector3(0, 0, prefabData.rotation);
                instance.transform.localScale = prefabData.scale;
            }
        }
    }

    // Метод для сброса всех сохранений
    [ContextMenu("Reset Saves")]
    public void ResetAllSaves()
    {
        PlayerPrefs.DeleteKey("GameSave");
        PlayerPrefs.DeleteKey("CurrentSpawnPrice");
        PlayerPrefs.Save();
        Debug.Log("Все сохранения сброшены!");
    }
}