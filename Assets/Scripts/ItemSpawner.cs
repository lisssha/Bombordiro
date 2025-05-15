using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject itemPrefab;
    public Transform spawnParent;
    public RectTransform spawnArea;

    public void SpawnItem()
    {


        RectTransform prefabRect = itemPrefab.GetComponent<RectTransform>();
        Vector2 itemSize = prefabRect.sizeDelta;

        Vector2 areaSize = spawnArea.rect.size;

        float maxX = (areaSize.x - itemSize.x) / 2f;
        float maxY = (areaSize.y - itemSize.y) / 2.5f;

        Vector2 randomPos = new Vector2(
            Random.Range(-maxX, maxX),
            Random.Range(-maxY, maxY)
        );

        GameObject newItem = Instantiate(itemPrefab, spawnArea);
        newItem.GetComponent<RectTransform>().anchoredPosition = randomPos;
    }
}
