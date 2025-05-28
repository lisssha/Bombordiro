using System.Collections.Generic;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }

    [SerializeField] private List<GameObject> availablePrefabs; // Список всех возможных префабов

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

        // Сохраняем только префабы с компонентом Item
        foreach (Item item in FindObjectsOfType<Item>())
        {
            // Пропускаем сам спавнер, если он имеет компонент Item
            if (item.GetComponent<ItemSpawner>() != null) continue;

            saveData.prefabs.Add(new SavedPrefab
            {
                prefabName = item.itemName,
                position = item.transform.position,
                rotation = item.transform.rotation.eulerAngles.z,
                scale = item.transform.localScale
            });
        }

        PlayerPrefs.SetString("GameSave", JsonUtility.ToJson(saveData));
        PlayerPrefs.Save();
    }

    public void LoadGame()
    {
        if (!PlayerPrefs.HasKey("GameSave")) return;

        string json = PlayerPrefs.GetString("GameSave");
        GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);

        // Восстанавливаем деньги
        GameManager.Instance.money = saveData.money;

        // Восстанавливаем префабы
        foreach (SavedPrefab prefabData in saveData.prefabs)
        {
            GameObject prefab = availablePrefabs.Find(p =>
                p.GetComponent<Item>().itemName == prefabData.prefabName);

            Debug.Log($"Загружаем префаб: {prefabData.prefabName}, найдено: {prefab != null}");

            if (prefab != null)
            {
                Instantiate(prefab, prefabData.position,
                    Quaternion.Euler(0, 0, prefabData.rotation));
            }
        }

    }

    private GameObject GetPrefabByName(string name)
    {
        return availablePrefabs.Find(p => p.GetComponent<Item>().itemName == name);
    }
}