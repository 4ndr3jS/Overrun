using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDragHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    Transform originalParent;
    CanvasGroup canvasGroup;

    public float minDropDistance = 2f;
    public float maxDropDistance = 3f;

    private InventoryController inventoryController;

    private Collider2D dropBoundary;

    private Slot pressedSlot;
    private bool didDrag;
    private bool isDragging;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        inventoryController = InventoryController.Instance;

        GameObject boundaryObject = GameObject.FindGameObjectWithTag("DropBoundary");
        if(boundaryObject != null)
        {
            dropBoundary = boundaryObject.GetComponent<Collider2D>();
        }
        else
        {
            Debug.Log("No tagged DropBoundary found.");
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        didDrag = true;
        isDragging = true;

        originalParent = transform.parent;
        transform.SetParent(transform.root);
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging)
            return; 

        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging)
            return;

        isDragging = false;

        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        Slot dropSlot = eventData.pointerEnter?.GetComponent<Slot>();

        if(dropSlot == null)
        {
            GameObject dropItem = eventData.pointerEnter;
            if(dropItem != null)
            {
                dropSlot = dropItem.GetComponentInParent<Slot>();
            }
        }

        Slot originalSlot = originalParent.GetComponent<Slot>();

        if(dropSlot == originalSlot || (dropSlot != null && dropSlot.currentItem == gameObject))
        {
            transform.SetParent(originalParent);
            UIUtils.FitAndPreserveAspectRatio(GetComponent<RectTransform>());
            return;
        }

        // to chekc if we are dropping on a slot
        if(dropSlot != null)
        {
            if (dropSlot.currentItem != null)
            {
                Item draggedItem = GetComponent<Item>();
                Item targetItem = dropSlot.currentItem.GetComponent<Item>();

                if (draggedItem.ID == targetItem.ID)
                {
                    targetItem.AddToStack(draggedItem.quantity);
                    originalSlot.currentItem = null;
                    Destroy(gameObject);
                }
                else
                {
                    dropSlot.currentItem.transform.SetParent(originalSlot.transform);
                    originalSlot.currentItem = dropSlot.currentItem;
                    UIUtils.FitAndPreserveAspectRatio(dropSlot.currentItem.GetComponent<RectTransform>());

                    transform.SetParent(dropSlot.transform);
                    dropSlot.currentItem = gameObject;
                    UIUtils.FitAndPreserveAspectRatio(GetComponent<RectTransform>());
                }
            }

            else
            {  
                originalSlot.currentItem = null;

                transform.SetParent(dropSlot.transform);
                dropSlot.currentItem = gameObject;
                UIUtils.FitAndPreserveAspectRatio(GetComponent<RectTransform>());
            }
        }
        else
        {
            // if we are even dropping inside of the inv
            if (!IsWithInInventory(eventData.position)){
                // Drop the item
                DropItem(originalSlot);

            }
            else
            {
                transform.SetParent(originalParent);
                UIUtils.FitAndPreserveAspectRatio(GetComponent<RectTransform>());
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pressedSlot = GetComponentInParent<Slot>();
        didDrag = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Slot clickedSlot = pressedSlot;
        pressedSlot = null;

        if (didDrag)
        {
            didDrag = false;
            return;
        }

        if (eventData.button == PointerEventData.InputButton.Left)
            clickedSlot?.UseAsHotbarSlot();

        else if (eventData.button == PointerEventData.InputButton.Right)
            SplitStack();
    }

    bool IsWithInInventory(Vector2 mousePosition)
    {
        RectTransform inventoryRect = originalParent.parent.GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(inventoryRect, mousePosition);
    }

    void DropItem(Slot originalSlot)
    {
        Item item = GetComponent<Item>();
        int quantity = item.quantity;

        if(quantity > 1)
        {
            item.RemoveFromStack();

            transform.SetParent(originalParent);
            UIUtils.FitAndPreserveAspectRatio(GetComponent<RectTransform>());

            quantity = 1;
        }
        else 
        {
            originalSlot.currentItem = null;
        }


        Transform playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        if(playerTransform == null)
        {
            Debug.LogError("Missing 'player' tag!");
            return;
        }

        Vector2 dropPosition = GetValidDropPos(playerTransform.position);
        
        GameObject dropItem = Instantiate(gameObject, dropPosition, Quaternion.identity);
        Item droppedItem = dropItem.GetComponent<Item>();
        droppedItem.quantity = 1;
        dropItem.GetComponent<BounceEffect>().StartBounce();

        if (quantity <= 1 && originalSlot.currentItem == null)
        {
            Destroy(gameObject);
        }
    }
    
    private Vector2 GetValidDropPos(Vector2 origin)
    {
        const int maxAttempts = 8;
        for(int i = 0; i < maxAttempts; i++)
        {
            Vector2 offset = Random.insideUnitCircle.normalized * Random.Range(minDropDistance, maxDropDistance);
            Vector2 candidate = origin + offset;

            if(dropBoundary == null || dropBoundary.OverlapPoint(candidate))
            {
                return candidate;
            }
        }

        if(dropBoundary != null)
        {
            Vector2 closest = dropBoundary.ClosestPoint(origin + (Vector2)(Random.insideUnitCircle * minDropDistance));
            Vector2 inward = (origin - closest).normalized * 0.2f;
            return closest + inward;
        }

        return origin;
    }

    private void SplitStack()
    {
        Item item = GetComponent<Item>();
        if (item == null || item.quantity <= 1)
            return;

        int splitAmount = item.quantity / 2;
        if (splitAmount <= 0)
            return;

        item.RemoveFromStack(splitAmount);

        GameObject newItem = item.CloneItem(splitAmount);

        if (inventoryController == null || newItem == null)
            return;

        foreach(Transform slotTransform in inventoryController.inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if(slot != null && slot.currentItem == null)
            {
                slot.currentItem = newItem;
                newItem.transform.SetParent(slot.transform);
                UIUtils.FitAndPreserveAspectRatio(newItem.GetComponent<RectTransform>());
                return;
            }
        }

        item.AddToStack(splitAmount);
        Destroy(newItem);
    }
}