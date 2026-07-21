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

    public float playerHealth;
    public float playerStamina;

    public int monstersKilled;
    public int highestWave;

    public int selectedHotbar = -1;
    public List<DroppedItemsSaveData> droppedItemsSaveData;
}

[System.Serializable]
public class ChestSaveData
{
    public string chestID;
    public bool isOpened;
}


[System.Serializable]
public class DroppedItemsSaveData
{
    public int itemID;
    public int quantity;
    public Vector3 positon;
}