using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Slot : MonoBehaviour
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
}
