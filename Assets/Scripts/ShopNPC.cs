using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ShopNPC : MonoBehaviour, IInteractable
{
    public string ShopID = "shop_merchant_01";
    public string shopKeeperName = "Merchant";

    public List<ShopStockItem> defaultShopStock = new();
    private List<ShopStockItem> currentShopStock = new();

    private bool isInitalized = false;

    [System.Serializable]
    public class ShopStockItem
    {
        public int itemID;
        public int quantity;
    }

    void Start()
    {
        InitShop();
    }

    private void InitShop()
    {
        if (isInitalized)
            return;

        currentShopStock = new List<ShopStockItem>();
        foreach (var item in defaultShopStock)
        {
            currentShopStock.Add(new ShopStockItem
            {
                itemID = item.itemID,
                quantity = item.quantity
            });
        }
        isInitalized = true;
    }

    public bool CanInteract()
    {
        return true;
    }

    public void Interact()
    {
        if (ShopController.Instance == null)
            return;

        if (ShopController.Instance.shopPanel.activeSelf)
        {
            ShopController.Instance.CloseShop();
        }

        else
        {
            ShopController.Instance.OpenShop(this);
        }
    }

    public List<ShopStockItem> GetCurrentStock()
    {
        return currentShopStock;
    }

    public void SetStock(List<ShopStockItem> stock)
    {
        currentShopStock = stock;
    }

    public void AddToStock(int itemID, int quantiity)
    {
        ShopStockItem exists = currentShopStock.Find(s => s.itemID == itemID);
        if(exists != null)
        {
            exists.quantity += quantiity;
        }
        else
        {
            currentShopStock.Add(new ShopStockItem
            {
                itemID = itemID,
                quantity = quantiity
            });
        }
    }

    public bool RemoveFromShopStock(int itemID, int quantity)
    {
        ShopStockItem exists = currentShopStock.Find(s => s.itemID == itemID);
        if (exists != null && exists.quantity >= quantity)
        {
            exists.quantity -= quantity;
            return true;
        }
        return false;
    }
}
