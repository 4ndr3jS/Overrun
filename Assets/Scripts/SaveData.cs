using UnityEngine;
using System.Collections.Generic;

[System.Serializable]

public class SaveData
{
    public Vector3 playerPosition;
    public string mapBoundary;
    public List<InventorySaveData> inventorySaveData;
    public List<InventorySaveData> hotbarSaveData;
    public List<ChestSaveData> chestSaveData;
    public float interactionRadius;

    public int playerCoins;
    public List<ShopInstanceData> shopStates = new();
}

[System.Serializable]
public class ChestSaveData
{
    public string chestID;
    public bool isOpened;
}

[System.Serializable]
public class ShopInstanceData
{
    public string shopID;
    public List<ShopItemData> stock = new();
}

[System.Serializable]
public class ShopItemData
{
    public int itemID;
    public int quantity;
}