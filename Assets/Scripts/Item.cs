using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public int ID;
    public string Name;

    public void PickUp()
    {
        Sprite itemIcon = GetComponent<Image>().sprite;
        if(ItemPickerUIController.Instance != null)
        {
            ItemPickerUIController.Instance.ShowItemPickup(Name, itemIcon);
        }
    }
}
