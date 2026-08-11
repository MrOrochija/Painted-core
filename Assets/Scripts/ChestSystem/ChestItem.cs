using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class ChestItem : MonoBehaviour
{
    public InventorySystem inventorySystem;
    public Chest chest;

    public void ButtonClick()
    {
        string numberOnly = Regex.Replace(gameObject.name, "[^0-9]", "");

        if (int.TryParse(numberOnly, out int slotNumber))
        {
            int index = slotNumber - 1;

            chest.resetElement(index);
        }

        Transform imageTransform = transform.Find("Image");

        if (imageTransform != null)
        {
            Image img = imageTransform.GetComponent<Image>();

            if (img != null && img.sprite != null)
            {
                string spriteName = img.sprite.name;

                if (inventorySystem != null)
                {
                    inventorySystem.AddItem(spriteName);
                }
            }
        }
        
        gameObject.SetActive(false);
    }
}