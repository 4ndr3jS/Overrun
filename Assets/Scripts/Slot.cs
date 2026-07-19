using UnityEngine;

public class Slot : MonoBehaviour
{
    // The item that is currently being held in this slot
    public GameObject currentItem;

    public GameObject equippedOutline;

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
