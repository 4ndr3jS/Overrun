using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Item : MonoBehaviour
{
    public int ID;
    public string Name;
    public int quantity = 1;
    public bool isCollected = false;

    public TMP_Text quantityText;

    public int buyPrice = 100;
    [Range(0, 1)]
    public float sellPriceMultiplier = 0.5f; 

    private void Awake()
    {
        isCollected = false;
        quantityText = GetComponentInChildren<TMP_Text>();
        UpdateQuantityDisplay();
    }

    public int GetSellPrice()
    {
        return Mathf.RoundToInt(buyPrice * sellPriceMultiplier);
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
}
