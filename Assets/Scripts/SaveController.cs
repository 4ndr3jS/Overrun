using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Cinemachine;
using Unity.VisualScripting.AssemblyQualifiedNameParser;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class SaveController : MonoBehaviour
{

    private string saveLocation;
    private InventoryController inventoryController;
    private HotbarController hotbarController;
    private Chest[] chests;

    private ItemDictionary itemDictionary;

    [SerializeField] private Vector3 fallbackPos = Vector3.zero;
    [SerializeField] private Collider2D fallbackBounds;
    [SerializeField] private float minSavedY = -18f;

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
            droppedItemsSaveData = GetDroppedItemsState(),
            monstersKilled = PlayerVitals.Instance != null ? PlayerVitals.Instance.GetMonsterKills() : 0,
            highestWave = PlayerVitals.Instance != null ? PlayerVitals.Instance.GetHighestWave() : 0,
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

            bool invalidPos = saveData.playerPosition.y < minSavedY;

            Vector3 loadedPos = invalidPos ? fallbackPos : saveData.playerPosition;

            player.transform.position = loadedPos;

            CinemachineConfiner2D confiner = FindAnyObjectByType<CinemachineConfiner2D>();

            if (confiner != null)
            {
                if (invalidPos)
                {
                    if(fallbackBounds != null)
                    {
                        confiner.BoundingShape2D = fallbackBounds;
                        confiner.InvalidateBoundingShapeCache();
                    }
                    else
                    {
                        Debug.LogWarning("Fallback map bound is not assigned");
                    }
                }
                else
                {
                    GameObject savedBoundsObject = GameObject.Find(saveData.mapBoundary);
                    if(savedBoundsObject != null)
                    {
                        Collider2D savedBound = savedBoundsObject.GetComponent<Collider2D>();

                        if(savedBound != null)
                        {
                            confiner.BoundingShape2D = savedBound;
                            confiner.InvalidateBoundingShapeCache();
                        }
                    }
                }
            }

            inventoryController.SetInventoryItems(saveData.inventorySaveData);
            hotbarController.SetHotbarItems(saveData.hotbarSaveData);

            LoadChestStates(saveData.chestSaveData);

            CurrencyController.Instance.SetCoins(saveData.playerCoins);

            InteractionDetector intDetector = player.GetComponentInChildren<InteractionDetector>();
            if (intDetector != null)
                intDetector.SetInteractionRadius(saveData.interactionRadius > 0f ? saveData.interactionRadius : 1f);

            if (PlayerVitals.Instance != null)
            {
                PlayerVitals.Instance.SetVitals(saveData.playerHealth, 100f);

                PlayerVitals.Instance.SetPlayerStats(saveData.monstersKilled, saveData.highestWave);
            }
                

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
