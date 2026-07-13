using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    private IInteractable interactableInRange = null;
    public GameObject interactionIcon;
    private CircleCollider2D interactionCollider;
    private float defaultRadius;
    
    void Awake()
    {
        interactionCollider = GetComponent<CircleCollider2D>();

        if(interactionCollider != null)
            defaultRadius = interactionCollider.radius;
    }

    void Start()
    {
        interactionIcon.SetActive(false);
    }

    public void SetInteractionRadius(float radius)
    {
        if(interactionCollider != null)
            interactionCollider.radius = radius;
    }

    public void ResetInteractionRadius()
    {
        if (interactionCollider != null)
            interactionCollider.radius = defaultRadius;
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (interactableInRange == null)
                return;

            interactableInRange.Interact();
            if (!interactableInRange.CanInteract())
            {
                interactionIcon.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
        {
            interactableInRange = interactable;
            interactionIcon.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable == interactableInRange)
        {
            interactableInRange = null;
            interactionIcon.SetActive(false);
        }
    }
}
