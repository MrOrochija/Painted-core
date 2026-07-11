using UnityEngine;
using UnityEngine.InputSystem;

public class InventorySystem : MonoBehaviour
{

    private bool active = true;

    void Update()
    {
        if (active) return;
        
        if (Keyboard.current != null && Keyboard.current.qKey.wasPressedThisFrame)
        {
            
        }
    }

    public void Activate()
    {
        active = true;
    }

    public void Deactivate()
    {
        active = false;
    }
}
