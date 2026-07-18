using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Item : MonoBehaviour
{
    [Header("ID")]
    public int ID;
    public string Name;

    [TextArea(2, 4)]
    public string Description;

    [Header("Stack")]
    public int quantity = 1;
    public bool isCollected = false;
    public TMP_Text quantityText;

    [Header("Pricing")]
    public int buyPrice = 100;
    [Range(0, 1)]
    public float sellPriceMultiplier = 0.5f;

    [Header("Holding")]
    public bool isHoldable = false;
    public Sprite heldSprite;

    private Image iconImage;

    private void Awake()
    {
        isCollected = false;
        quantityText = GetComponentInChildren<TMP_Text>();
        iconImage = GetComponent<Image>();
        UpdateQuantityDisplay();
    }

    public Sprite Icon
    {
        get
        {
            if (iconImage == null)
                iconImage = GetComponent<Image>();
            return iconImage != null ? iconImage.sprite : null;
        }
    }

    public int GetSellPrice()
    {
        return Mathf.RoundToInt(buyPrice * sellPriceMultiplier);
    }

    public void PopulateShopSlot(Image iconTarget, TMP_Text nameTarget, TMP_Text descriptionTarget, TMP_Text priceTarget, int price)
    {
        if (iconTarget != null)
            iconTarget.sprite = Icon;

        if (nameTarget != null)
            nameTarget.text = Name;

        if (descriptionTarget != null)
            descriptionTarget.text = Description;

        if (priceTarget != null)
            priceTarget.text = price.ToString();
    }

    public void UpdateQuantityDisplay()
    {
        if(quantityText != null)
        {
            quantityText.text = quantity > 1 ? quantity.ToString() : "";
        }
    }

    public void AddToStack(int amount = 1)
    {
        quantity += amount;
        UpdateQuantityDisplay();
    }

    public int RemoveFromStack(int amount = 1)
    {
        int removed = Mathf.Min(amount, quantity);
        quantity -= removed;
        UpdateQuantityDisplay();
        return removed;
    }

    public GameObject CloneItem(int newQuantity)
    {
        GameObject clone = Instantiate(gameObject);
        Item cloneItem = clone.GetComponent<Item>();
        cloneItem.quantity = newQuantity;
        cloneItem.UpdateQuantityDisplay();
        return clone;
    }

    public virtual void UseItem()
    {
        Debug.Log("Using item " + Name);
    }

    public void PickUp()
    {
        Sprite itemIcon = GetComponent<Image>().sprite;
        if(ItemPickerUIController.Instance != null)
        {
            ItemPickerUIController.Instance.ShowItemPickup(Name, itemIcon);
        }
    }

    public void SetQuantityTextStyle(Vector2 anchoredPosition, float fontSize)
    {
        if (quantityText == null)
            return;

        RectTransform rt = quantityText.GetComponent<RectTransform>();
        rt.anchoredPosition = anchoredPosition;
        quantityText.fontSize = fontSize;
    }
}
