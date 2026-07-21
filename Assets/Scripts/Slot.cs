using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerClickHandler
{
    // The item that is currently being held in this slot
    public GameObject currentItem;

    public GameObject equippedOutline;

    public bool useQuantityStyle;
    public Vector2 quantityTextPos = new Vector2(18f, -18f);
    public float quantityTextSize = 12f;
    private GameObject lastStyledItem;

    private void LateUpdate()
    {
        if (currentItem == lastStyledItem)
            return;

        lastStyledItem = currentItem;
        ApplyQuantityStyle();
    }

    private void ApplyQuantityStyle()
    {
        if (currentItem == null)
            return;

        Item item = currentItem.GetComponent<Item>();
        if (item == null)
            return;

        if (useQuantityStyle)
            item.SetQuantityTextStyle(quantityTextPos, quantityTextSize);
        else
            item.ResetQuantityTextStyle();
    } 

    public void SetEquipped(bool isEquipped)
    {
        if (equippedOutline != null)
        {
            equippedOutline.SetActive(isEquipped);
            if (isEquipped)
                equippedOutline.transform.SetAsLastSibling();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            UseAsHotbarSlot();
    }

    public void UseAsHotbarSlot()
    {
        HotbarController hotbarC = GetComponentInParent<HotbarController>();
        if (hotbarC == null)
            return;

        hotbarC.UseItemInSlot(transform.GetSiblingIndex());
    }
}
