using UnityEngine;

public class Chest : InteractableObject
{
    public ChestSystem chestSystem;
    
    public Sprite[] itemsInChest; 
    
    public override void Interact()
    {
        if (chestSystem != null)
        {
            chestSystem.Toggle(itemsInChest);
        }
    }

    public void resetElement(int index)
    {
        itemsInChest[index] = null;
    }
}