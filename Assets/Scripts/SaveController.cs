using UnityEngine;
using System.IO;
using Unity.Cinemachine;
using System.Linq;
using System.Collections.Generic;
using Unity.VisualScripting.AssemblyQualifiedNameParser;

public class SaveController : MonoBehaviour
{

    private string saveLocation;
    private InventoryController inventoryController;
    private HotbarController hotbarController;
    private Chest[] chests;

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
}
