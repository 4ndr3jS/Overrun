using UnityEngine;
using System.IO;
using Unity.Cinemachine;
using System.Linq;
using System.Collections.Generic;
using Unity.VisualScripting.AssemblyQualifiedNameParser;
using UnityEngine.Rendering.UI;

public class SaveController : MonoBehaviour
{

    private string saveLocation;
    private InventoryController inventoryController;
    private HotbarController hotbarController;
    private Chest[] chests;

    private ItemDictionary itemDictionary;

    void Start()
    {
        InitializeComponents();
        LoadGame();
    }

    private void InitializeComponents()
    {
        saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");
        inventoryController = FindAnyObjectByType<InventoryController>();
        hotbarController = FindAnyObjectByType<HotbarController>();
        chests = FindObjectsByType<Chest>();
        itemDictionary = FindAnyObjectByType<ItemDictionary>();
    }

    public void saveGame()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        InteractionDetector intDetector = player.GetComponentInChildren<InteractionDetector>();

        SaveData saveData = new SaveData
        {
            playerPosition = player.transform.position,
            mapBoundary = FindAnyObjectByType<CinemachineConfiner2D>().BoundingShape2D.gameObject.name,
            inventorySaveData = inventoryController.GetInventoryItems(),
            hotbarSaveData = hotbarController.GetHotbarItems(),
            chestSaveData = GetChestsState(),
            interactionRadius = intDetector != null ? intDetector.GetInteractionRadius() : 1f,
            playerCoins = CurrencyController.Instance.GetCoins(),
            playerHealth = PlayerVitals.Instance != null ? PlayerVitals.Instance.GetHealth() : 100f,
            playerStamina = PlayerVitals.Instance != null ? PlayerVitals.Instance.GetStamina() : 100f,
            selectedHotbar = hotbarController.GetSelectedSlot(),
            droppedItemsSaveData = GetDroppedItemsState()
        };

        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData));
    }

    private List<ChestSaveData> GetChestsState()
    {
        List<ChestSaveData> chestStates = new List<ChestSaveData>();

        foreach (Chest chest in chests)
        {
            ChestSaveData chestSaveData = new ChestSaveData
            {
                chestID = chest.ChestID,
                isOpened = chest.isOpened
            };
            chestStates.Add(chestSaveData);
        }

        return chestStates;
    }

    private List<DroppedItemsSaveData> GetDroppedItemsState()
    {
        List<DroppedItemsSaveData> droppedItems = new List<DroppedItemsSaveData>();

        GameObject[] worldItems = GameObject.FindGameObjectsWithTag("Item");

        foreach (GameObject obj in worldItems)
        {
            Item item = obj.GetComponent<Item>();
            if (item == null || item.isCollected)
                continue;

            if (obj.GetComponentInParent<Item>() != null || obj.GetComponentInParent<ShopSlot>() != null)
                continue;

            droppedItems.Add(new DroppedItemsSaveData
            {
                itemID = item.ID,
                quantity = item.quantity,
                positon = obj.transform.position
            });
        }

        return droppedItems;
    }

    public void LoadGame()
    {
        if (File.Exists(saveLocation)) {
            string json = File.ReadAllText(saveLocation);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            player.transform.position = saveData.playerPosition;

            FindAnyObjectByType<CinemachineConfiner2D>().BoundingShape2D = GameObject.Find(saveData.mapBoundary).GetComponent<PolygonCollider2D>();

            inventoryController.SetInventoryItems(saveData.inventorySaveData);
            hotbarController.SetHotbarItems(saveData.hotbarSaveData);

            LoadChestStates(saveData.chestSaveData);

            CurrencyController.Instance.SetCoins(saveData.playerCoins);

            InteractionDetector intDetector = player.GetComponentInChildren<InteractionDetector>();
            if (intDetector != null)
                intDetector.SetInteractionRadius(saveData.interactionRadius > 0f ? saveData.interactionRadius : 1f);

            if (PlayerVitals.Instance != null)
                PlayerVitals.Instance.SetVitals(saveData.playerHealth, saveData.playerStamina);

            hotbarController.SetSelectedSlotIndex(saveData.selectedHotbar);

            SpawnDroppedItems(saveData.droppedItemsSaveData);
        }
        else
        {
            saveGame();

            inventoryController.SetInventoryItems(new List<InventorySaveData>());
            hotbarController.SetHotbarItems(new List<InventorySaveData>());
        }
    }

    private void LoadChestStates(List<ChestSaveData> chestStates)
    {
        foreach(Chest chest in chests)
        {
            ChestSaveData chestSaveData = chestStates.FirstOrDefault(chestStates => chestStates.chestID == chest.ChestID);

            if(chestSaveData != null)
            {
                chest.SetOpened(chestSaveData.isOpened);
            }
        }
    }

    private void SpawnDroppedItems(List<DroppedItemsSaveData> droppedItems)
    {
        if (droppedItems == null || itemDictionary == null)
            return;

        foreach (DroppedItemsSaveData data in droppedItems)
        {
            GameObject itemPrefab = itemDictionary.GetItemPrefab(data.itemID);
            if (itemPrefab == null)
                return;

            GameObject spawnedItem = Instantiate(itemPrefab, data.positon, Quaternion.identity);

            Item item = spawnedItem.GetComponent<Item>();
            if(item != null)
            {
                item.quantity = data.quantity;
                item.UpdateQuantityDisplay();
            }
        }
    }
}
