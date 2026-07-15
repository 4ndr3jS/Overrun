using TMPro;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    public static ShopController Instance;

    [Header("UI")]
    public GameObject shopPanel;
    public Transform shopInventoryGrid, playerInventoryGrid;
    public GameObject shopSlotPrefab;
    public TMP_Text playerMoneyText, shopTitleText;

    private ItemDictionary itemDictionary;
    private ShopNPC currentShop;

    [Header("Shop Slot Item Override")]
    public Vector2 shopQuantityTextPosition = new Vector2(18f, -18f);
    public float shopQuantityTextFontSize = 12f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        itemDictionary = FindAnyObjectByType<ItemDictionary>();
        shopPanel.SetActive(false);
        if(CurrencyController.Instance != null)
        {
            CurrencyController.Instance.OnCoinChange += UpdateMoneyDisplay;
            UpdateMoneyDisplay(CurrencyController.Instance.GetCoins());
        }
    }

    private void UpdateMoneyDisplay(int amount)
    {
        if (playerMoneyText != null)
        {
            playerMoneyText.text = amount.ToString();
        }
    }

    public void OpenShop(ShopNPC shop)
    {
        currentShop = shop;
        shopPanel.SetActive(true);
        if (shopTitleText != null)
            shopTitleText.text = shop.shopKeeperName + "'s shop";

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(shopPanel.GetComponent<RectTransform>());

        RefreshShopDisplay();
        RefreshPlayerInvDisplay();
        PauseController.SetPause(true);
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
        currentShop = null;
        PauseController.SetPause(false);
    }

    public void RefreshShopDisplay()
    {
        if(currentShop == null)
            return;

        foreach (Transform child in shopInventoryGrid)
            Destroy(child.gameObject);

        foreach(var stockItem in currentShop.GetCurrentStock())
        {
            if (stockItem.quantity <= 0)
                continue;

            CreateShopSlot(shopInventoryGrid, stockItem.itemID, stockItem.quantity, true);
        }

    }

    public void RefreshPlayerInvDisplay()
    {
        if (InventoryController.Instance == null)
            return;

        foreach (Transform child in playerInventoryGrid)
            Destroy(child.gameObject);

        foreach (Transform slotTransform in InventoryController.Instance.inventoryPanel.transform)
        {
            Slot invSlot = slotTransform.GetComponent<Slot>();
            if(invSlot?.currentItem != null)
            {
                Item originalItem = invSlot.currentItem.GetComponent<Item>();
                CreateShopSlot(playerInventoryGrid, originalItem.ID, originalItem.quantity, false, invSlot);
            }
        }
    }

    private void CreateShopSlot(Transform grid, int itemID, int quantity, bool isShop, Slot originalSlot = null)
    {
        GameObject slotObj = Instantiate(shopSlotPrefab, grid);

        LayoutRebuilder.ForceRebuildLayoutImmediate(grid.GetComponent<RectTransform>());

        GameObject itemPrefab = itemDictionary.GetItemPrefab(itemID);
        if (itemPrefab == null)
            return;

        GameObject itemInstance = Instantiate(itemPrefab, slotObj.transform);
        UIUtils.FitAndPreserveAspectRatio(itemInstance.GetComponent<RectTransform>(), 15f);

        Item item = itemInstance.GetComponent<Item>();
        item.quantity = quantity;
        item.UpdateQuantityDisplay();

        item.SetQuantityTextStyle(shopQuantityTextPosition, shopQuantityTextFontSize);

        int price = isShop ? item.buyPrice : item.GetSellPrice();

        ShopSlot slot = slotObj.GetComponent<ShopSlot>();
        slot.isShopSlot = isShop;
        slot.SetItem(itemInstance, price);

        ItemDragHandler dragHandler = itemInstance.GetComponent<ItemDragHandler>();
        if (dragHandler)
            dragHandler.enabled = false;

        ShopItemHandler handler = slotObj.GetComponent<ShopItemHandler>();
        if (handler == null)
            handler = slotObj.AddComponent<ShopItemHandler>();

        handler.Initialise(isShop);
        if (!isShop)
            handler.originalInvSlot = originalSlot;
    }

    public void AddItemToShop(int itemID, int quantity)
    {
        if (!currentShop)
            return;

        currentShop.AddToStock(itemID, quantity);
        RefreshShopDisplay();
    }

    public bool RemoveItemFromShop(int itemID, int quantity)
    {
        if (!currentShop)
            return false;

        bool success = currentShop.RemoveFromShopStock(itemID, quantity);
        if (success)
            RefreshShopDisplay();
        return success;
    }
}
