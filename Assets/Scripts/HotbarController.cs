using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.UIElements;


public class HotbarController : MonoBehaviour
{
    public GameObject hotbarPanel;
    public GameObject slotPrefab;
    public int slotCount = 8;

    private ItemDictionary itemDict;

    private Key[] hotbarKeys;

    public int selectedSlotIndex = -1;

    private void Awake()
    {
        itemDict = FindAnyObjectByType<ItemDictionary>();

        hotbarKeys = new Key[slotCount];
        for(int i = 0; i < slotCount; i++)
        {
            hotbarKeys[i] = i < 7 ? (Key)((int)Key.Digit1 + i) : Key.Digit0;
        }
    }

    void Update()
    {
        for(int i = 0; i < slotCount; i++)
        {
            if (Keyboard.current[hotbarKeys[i]].wasPressedThisFrame)
            {
                UseItemInSlot(i);
            }
        }
    }



    void UseItemInSlot(int index)
    {
        Slot slot = hotbarPanel.transform.GetChild(index).GetComponent<Slot>();
        if (slot.currentItem == null)
            return;
            

        Item item = slot.currentItem.GetComponent<Item>();

        if (item.isConsumable)
            item.UseItem();
        else
            EquipSlot(index);
    }

    private void EquipSlot(int index)
    {
        if (selectedSlotIndex == index)
            return;

        if (selectedSlotIndex >= 0 && selectedSlotIndex < hotbarPanel.transform.childCount)
        {
            Slot prevSlot = hotbarPanel.transform.GetChild(selectedSlotIndex).GetComponent<Slot>();
            if (prevSlot != null)
                prevSlot.SetEquipped(false);
        }

        selectedSlotIndex = index;

        Slot newSlot = hotbarPanel.transform.GetChild(index).GetComponent<Slot>();
        if (newSlot != null)
            newSlot.SetEquipped(true);
    }

    public int GetSelectedSlot() => selectedSlotIndex;

    public void SetSelectedSlotIndex(int index)
    {
        selectedSlotIndex = -1;

        if (index < 0 || index >= hotbarPanel.transform.childCount)
            return;

        Slot slot = hotbarPanel.transform.GetChild(index).GetComponent<Slot>();
        if (slot != null && slot.currentItem != null)
            EquipSlot(index);
    }

    public List<InventorySaveData> GetHotbarItems()
    {
        List<InventorySaveData> hotbarData = new List<InventorySaveData>();
        foreach (Transform slotTransform in hotbarPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                hotbarData.Add(new InventorySaveData
                {
                    itemID = item.ID,
                    slotIndex = slotTransform.GetSiblingIndex()
                });
            }
        }
        return hotbarData;
    }

    public void SetHotbarItems(List<InventorySaveData> inventorySaveData)
    {
        selectedSlotIndex = -1;

        foreach (Transform child in hotbarPanel.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < slotCount; i++)
        {
            Instantiate(slotPrefab, hotbarPanel.transform);
        }

        foreach (InventorySaveData data in inventorySaveData)
        {
            Slot slot = hotbarPanel.transform.GetChild(data.slotIndex).GetComponent<Slot>();

            GameObject itemPrefab = itemDict.GetItemPrefab(data.itemID);
            if (itemPrefab != null)
            {
                GameObject item = Instantiate(itemPrefab, slot.transform);
                UIUtils.FitAndPreserveAspectRatio(item.GetComponent<RectTransform>());
                slot.currentItem = item;
            }
        }
    }
}
