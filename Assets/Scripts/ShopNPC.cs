using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ShopNPC : MonoBehaviour, IInteractable
{
    public string ShopID = "shop_merchant_01";
    public string shopKeeperName = "Merchant";

    // unlimited stock
    public List<int> shopItemIDs = new();

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

    public List<int> GetCurrentStock()
    {
        return shopItemIDs;
    }
}
