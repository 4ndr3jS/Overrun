using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    private InventoryController inventoryController;


    void Start()
    {
        inventoryController = FindAnyObjectByType<InventoryController>();
    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            Item item = collision.GetComponent<Item>();

            if (item == null || item.isCollected)
                return;

            item.isCollected = true;

            bool itemAdded = inventoryController.AddItem(collision.gameObject);

            if (itemAdded)
            {
                item.PickUp();
                Destroy(collision.gameObject);
            }
            else
            {
                item.isCollected = false;
            }
        }
    }
}
