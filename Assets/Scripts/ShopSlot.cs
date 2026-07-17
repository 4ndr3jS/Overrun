using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    public GameObject currentItem;
    public int itemPrice;

    [Header("Row UI Refreces")]
    public TMP_Text priceText;
    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public Image itemIcon;
    public Image coinIcon;
    public bool isShopSlot = true;

    private void Awake()
    {
        if (!priceText)
            priceText = FindChild<TMP_Text>("PriceText");

        if (!descriptionText)
            descriptionText = FindChild<TMP_Text>("DescriptionText");

        if (!nameText)
            nameText = FindChild<TMP_Text>("NameText");

        if (!itemIcon)
            itemIcon = FindChild<Image>("ItemIcon");

        if (!coinIcon)
            coinIcon = FindChild<Image>("CoinIcon");
    }

    private T FindChild<T>(string name) where T : Component
    {
        Transform tf = transform.Find(name);
        return tf != null ? tf.GetComponent<T>() : null;
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
