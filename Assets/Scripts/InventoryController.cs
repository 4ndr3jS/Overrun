using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    private ItemDictionary itemDictionary;

    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public int slotCount;
    public GameObject[] ItemPrefabs;

    public static InventoryController Instance { get; private set; }

    void Awake()
    {
        itemDictionary = FindAnyObjectByType<ItemDictionary>();

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public bool AddItem(GameObject itemPrefab)
    {
        Item itemToAdd = itemPrefab.GetComponent<Item>();
        if (itemToAdd == null)
            return false;

        // This is for an existing item in a slot
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null && slot.currentItem != null)
            {
                Item slotItem = slot.currentItem.GetComponent<Item>();
                if(slotItem != null && slotItem.ID == itemToAdd.ID)
                {
                    slotItem.AddToStack();
                    return true;
                }
            }
        }
        
        // This is for checking an empty slot
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if(slot != null && slot.currentItem == null)
            {
                GameObject newItem = Instantiate(itemPrefab, slotTransform);
                UIUtils.FitAndPreserveAspectRatio(newItem.GetComponent<RectTransform>(), 15f);
                slot.currentItem = newItem;
                return true;
            }
        }

        Debug.Log("Inventory is full!!!");
        return false;
    }

    

    public List<InventorySaveData> GetInventoryItems()
    {
        List<InventorySaveData> invData = new List<InventorySaveData>();
        foreach(Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if(slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                invData.Add(new InventorySaveData
                {
                    itemID = item.ID,
                    slotIndex = slotTransform.GetSiblingIndex(),
                    quantity = item.quantity
                });
            }
        }
        return invData;
    }

    public void SetInventoryItems(List<InventorySaveData> inventorySaveData) 
    {
        // We need to clear all from the invPanel to get rid of any dupes
        foreach(Transform child in inventoryPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // Creating the slots
        for(int i = 0; i < slotCount; i++)
        {
            Instantiate(slotPrefab, inventoryPanel.transform);
        }

        foreach (InventorySaveData data in inventorySaveData)
        {
            Slot slot = inventoryPanel.transform.GetChild(data.slotIndex).GetComponent<Slot>();

            GameObject itemPrefab = itemDictionary.GetItemPrefab(data.itemID);
            if (itemPrefab != null)
            {
                GameObject item = Instantiate(itemPrefab, slot.transform);
                UIUtils.FitAndPreserveAspectRatio(item.GetComponent<RectTransform>(), 15f);

                Item itemComponent = item.GetComponent<Item>();
                if (itemComponent != null && data.quantity > 1)
                {
                    itemComponent.quantity = data.quantity;
                    itemComponent.UpdateQuantityDisplay();
                }

                slot.currentItem = item;
            }
        }
    }

    public void RefreshAllItems()
    {
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(inventoryPanel.GetComponent<RectTransform>());
        
        foreach(Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if(slot != null && slot.currentItem != null)
            {
                UIUtils.FitAndPreserveAspectRatio(slot.currentItem.GetComponent<RectTransform>(), 15f);
            }
        }
    }
}
