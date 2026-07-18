using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework.Internal.Execution;

public class ItemDictionary : MonoBehaviour
{
    public List<Item> itemPrefabs;
    private Dictionary<int, GameObject> itemDictionary;

    private void Awake()
    {
        itemDictionary = new Dictionary<int, GameObject>();

        foreach(Item item in itemPrefabs)
        {
            if (item == null)
                continue;

            if (itemDictionary.ContainsKey(item.ID))
            {
                Debug.LogError($"[ItemDictionary] duplicate item ID {item.ID} on '{item.name}'" + $"and '{itemDictionary[item.ID].name}'. Please work");
            }

            itemDictionary[item.ID] = item.gameObject;
        }
    }

    public GameObject GetItemPrefab(int itemID)
    {
        itemDictionary.TryGetValue(itemID, out GameObject prefab);
        if(prefab  == null)
        {
            Debug.LogWarning($"Item with ID: {itemID} not found in dictionary");
        }
        return prefab;
    }
}
