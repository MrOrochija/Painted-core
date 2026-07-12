using UnityEngine;

public class Item : MonoBehaviour
{
    public GameObject invSystem;
    private InventorySystem inventorySystem;

    void Start()
    {
        inventorySystem = invSystem.GetComponent<InventorySystem>();
    }

    private void OnTriggerEnter2D()
    {
        inventorySystem.AddItem(gameObject.name);
        Destroy(gameObject);
    }
}
