using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    private InteractableObject currentObject; 

    void Update()
    {
        if (currentObject != null && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            currentObject.Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        InteractableObject interactable = other.GetComponent<InteractableObject>();
        if (interactable != null)
        {
            currentObject = interactable;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        InteractableObject interactable = other.GetComponent<InteractableObject>();
        if (interactable != null && interactable == currentObject)
        {
            currentObject = null;
        }
    }
}