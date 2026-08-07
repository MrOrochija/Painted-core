using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerInteract : MonoBehaviour
{
    public PlayerHealth playerHealth;
    private InteractableObject currentObject; 

    void Update()
    {
        if (currentObject != null && playerHealth != null)
        {
            if (IsKeyWasPressedThisFrame(playerHealth.interact))
            {
                currentObject.Interact();
            }
        }
    }

    private bool IsKeyWasPressedThisFrame(char c)
    {
        if (Keyboard.current == null) return false;

        c = char.ToLower(c);

        KeyControl keyControl = c switch
        {
            'a' => Keyboard.current.aKey,
            'b' => Keyboard.current.bKey,
            'c' => Keyboard.current.cKey,
            'd' => Keyboard.current.dKey,
            'e' => Keyboard.current.eKey,
            'f' => Keyboard.current.fKey,
            'g' => Keyboard.current.gKey,
            'h' => Keyboard.current.hKey,
            'i' => Keyboard.current.iKey,
            'j' => Keyboard.current.jKey,
            'k' => Keyboard.current.kKey,
            'l' => Keyboard.current.lKey,
            'm' => Keyboard.current.mKey,
            'n' => Keyboard.current.nKey,
            'o' => Keyboard.current.oKey,
            'p' => Keyboard.current.pKey,
            'q' => Keyboard.current.qKey,
            'r' => Keyboard.current.rKey,
            's' => Keyboard.current.sKey,
            't' => Keyboard.current.tKey,
            'u' => Keyboard.current.uKey,
            'v' => Keyboard.current.vKey,
            'w' => Keyboard.current.wKey,
            'x' => Keyboard.current.xKey,
            'y' => Keyboard.current.yKey,
            'z' => Keyboard.current.zKey,
            _ => null
        };

        return keyControl != null && keyControl.wasPressedThisFrame;
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