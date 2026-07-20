using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    public static ShopController Instance;

    [Header("UI")]
    public GameObject shopPanel;
    public Transform shopInventoryGrid, playerInventoryGrid;
    public GameObject shopSlotPrefab;
    public GameObject playerSlotPrefab;
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

    private void Update()
    {
        if (shopPanel != null && shopPanel.activeSelf && Keyboard.current != null && Keyboard.current[Key.Tab].wasPressedThisFrame)
            CloseShop();
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

        foreach(int itemID in currentShop.GetCurrentStock())
        {
            CreateShopSlot(shopInventoryGrid, itemID, 1, true);
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
        GameObject prefabToUse = isShop ? shopSlotPrefab : playerSlotPrefab;
        GameObject slotObj = Instantiate(prefabToUse, grid);

        LayoutRebuilder.ForceRebuildLayoutImmediate(grid.GetComponent<RectTransform>());

        GameObject itemPrefab = itemDictionary.GetItemPrefab(itemID);
        if (itemPrefab == null)
            return;

        GameObject itemInstance = Instantiate(itemPrefab, slotObj.transform);

        ShopSlot slot = slotObj.GetComponent<ShopSlot>();

        Image itemInstanceImage = itemInstance.GetComponent<Image>();
        bool slotHasIcon = slot != null && slot.itemIcon != null;
        if (itemInstanceImage != null)
        {
            if (slotHasIcon)
                itemInstanceImage.enabled = false;
            else
                UIUtils.FitAndPreserveAspectRatio(itemInstanceImage.GetComponent<RectTransform>(), 15f);
        }
            
        Item item = itemInstance.GetComponent<Item>();
        item.quantity = quantity;
        item.UpdateQuantityDisplay();

        item.SetQuantityTextStyle(shopQuantityTextPosition, shopQuantityTextFontSize);

        int price = isShop ? item.buyPrice : item.GetSellPrice();

        slot.isShopSlot = isShop;
        slot.SetItem(itemInstance, price);

        // Took me a dumb amount of time to find out this line was missing, im so mad rn!!!!
        item.PopulateShopSlot(slot.itemIcon, slot.nameText, slot.descriptionText, slot.priceText, price);

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
}
