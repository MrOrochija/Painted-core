using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerClickHandler
{
    public GameObject invSystemObject;
    private InventorySystem inventorySystem;
    public string itemName;
    public string bio;

    void Start()
    {
        if (invSystemObject != null)
        {
            inventorySystem = invSystemObject.GetComponent<InventorySystem>();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        inventorySystem.SlotWasClicked(gameObject, itemName, bio);
    }
}