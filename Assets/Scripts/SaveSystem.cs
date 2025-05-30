using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }

    [SerializeField] private List<GameObject> availablePrefabs;
    [SerializeField] private string spawnParentName = "SpawnArena"; // ���������� ���� SpawnArena �� ��������

    private Transform _spawnParent;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ������� ������������ ������ ��� ������ �������� �����
        GameObject parentObj = GameObject.Find(spawnParentName);
        _spawnParent = parentObj != null ? parentObj.transform : null;
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
                position = item.transform.localPosition, // ���������� localPosition
                rotation = item.transform.localEulerAngles.z,
                scale = item.transform.localScale
            });
        }

        PlayerPrefs.SetString("GameSave", JsonUtility.ToJson(saveData));
        PlayerPrefs.Save();
        Debug.Log("���� ���������");
    }

    public void LoadGame()
    {
        if (!PlayerPrefs.HasKey("GameSave")) return;

        // ����������, ��� ������������ ������ ������
        if (_spawnParent == null)
        {
            GameObject parentObj = GameObject.Find(spawnParentName);
            _spawnParent = parentObj != null ? parentObj.transform : null;

            if (_spawnParent == null)
            {
                Debug.LogError($"�� ������ ������������ ������: {spawnParentName}");
                return;
            }
        }

        string json = PlayerPrefs.GetString("GameSave");
        GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);

        // ������� ������ �������
        ClearExistingPrefabs();

        // ��������� �������
        StartCoroutine(LoadPrefabsCoroutine(saveData));
    }

    private IEnumerator LoadPrefabsCoroutine(GameSaveData saveData)
    {
        // ���� ���� ���� ��� ������������ �����
        yield return null;

        foreach (SavedPrefab prefabData in saveData.prefabs)
        {
            GameObject prefab = availablePrefabs.Find(p =>
                p.GetComponent<Item>().itemName == prefabData.prefabName);

            if (prefab != null && _spawnParent != null)
            {
                GameObject instance = Instantiate(
                    prefab,
                    _spawnParent,
                    false
                );

                instance.transform.localPosition = prefabData.position;
                instance.transform.localEulerAngles = new Vector3(0, 0, prefabData.rotation);
                instance.transform.localScale = prefabData.scale;

                // ���� ��������� ����� ����� ��������� ��������
                yield return new WaitForSeconds(0.01f);
            }
        }
    }

    private void ClearExistingPrefabs()
    {
        Item[] items = FindObjectsOfType<Item>();
        foreach (Item item in items)
        {
            if (item.GetComponent<ItemSpawner>() == null)
            {
                Destroy(item.gameObject);
            }
        }
    }

    // ����� ��� ������ ���� ����������
    [ContextMenu("Reset Saves")]
    public void ResetAllSaves()
    {
        PlayerPrefs.DeleteKey("GameSave");
        PlayerPrefs.DeleteKey("CurrentSpawnPrice");
        PlayerPrefs.Save();
        Debug.Log("��� ���������� ��������!");
    }
}