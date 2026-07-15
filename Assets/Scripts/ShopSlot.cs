using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopSlot : MonoBehaviour
{
    public GameObject currentItem;
    public int itemPrice;
    public TMP_Text priceText;
    public bool isShopSlot = true;

    private void Awake()
    {
        if (!priceText)
        {
            priceText = transform.Find("PriceText").GetComponent<TMP_Text>();
        }
    }

    public void UpdatePriceDisplay()
    {
        if(priceText && currentItem)
        {
            priceText.text = itemPrice.ToString();
        }
    }

    public void SetItem(GameObject item, int price)
    {
        currentItem = item;
        itemPrice = price;
        UpdatePriceDisplay();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"[ShopSlot] OnPointerClick on {gameObject.name}. currentItem = {(currentItem != null ? currentItem.name : "NULL")}");

        if (currentItem == null)
            return;

        ShopItemHandler handler = currentItem.GetComponent<ShopItemHandler>();
        Debug.Log($"[ShopSlot] handler found = {handler != null}");
        handler?.OnPointerClick(eventData);
    }
}
