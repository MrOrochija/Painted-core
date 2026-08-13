using UnityEngine;
using System.Collections.Generic;

public class Shop : InteractableObject
{
    public ShopSystem shopSystem;

    [TextArea(2, 5)]
    public string[] dialogue;

    public List<ShopItemData> shopItems = new List<ShopItemData>();

    public override void Interact()
    {
        if (dialogue != null && dialogue.Length > 0)
        {
            int randomIndex = Random.Range(0, dialogue.Length);
            string randomText = dialogue[randomIndex];

            shopSystem.Active(randomText, shopItems); 
        }
    }
}