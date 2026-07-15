using NUnit.Framework.Internal.Execution;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopItemHandler : MonoBehaviour, IPointerClickHandler
{
    private bool isShopItem;
    public Slot originalInvSlot;

    public void Initialise(bool shopItem) => isShopItem = shopItem;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right || eventData.button == PointerEventData.InputButton.Left)
        {
            if (isShopItem)
                BuyItem();

            else
                SellItem();
        }
    }

    private void BuyItem()
    {
        Item item = GetComponentInChildren<Item>();
        ShopSlot slot = GetComponent<ShopSlot>();
        if (!item || !slot)
            return;

        if (CurrencyController.Instance.GetCoins() < slot.itemPrice)
        {
            Debug.Log("Not enough coinsss");
            return;
        }

        GameObject itemPrefab = FindAnyObjectByType<ItemDictionary>().GetItemPrefab(item.ID);
        if (InventoryController.Instance.AddItem(itemPrefab))
        {
            CurrencyController.Instance.SpendCoins(slot.itemPrice);
            ShopController.Instance.RefreshPlayerInvDisplay();
            ShopController.Instance.RemoveItemFromShop(item.ID, 1);
        }
        else
        {
            Debug.Log("Inventory full");
        }
    }

    private void SellItem()
    {
        Item item = GetComponentInChildren<Item>();
        ShopSlot slot = GetComponent<ShopSlot>();
        if (!item || !slot || !originalInvSlot)
            return;

        Item invItem = originalInvSlot.currentItem?.GetComponent<Item>();
        if (!invItem)
            return;

        if (invItem.quantity > 1)
            invItem.RemoveFromStack(1);

        else
        {
            Destroy(originalInvSlot.currentItem);
            originalInvSlot.currentItem = null;
        }

        InventoryController.Instance.RebuildItemCounts();
        CurrencyController.Instance.AddCoins(slot.itemPrice);
        ShopController.Instance.RefreshPlayerInvDisplay();
        ShopController.Instance.AddItemToShop(item.ID, 1);
    }
}
