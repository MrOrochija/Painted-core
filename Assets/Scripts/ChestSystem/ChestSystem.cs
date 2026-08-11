using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class ChestSystem : MonoBehaviour
{
    public GameObject ui;
    public GameObject content;

    public PlayerMovement playerMovement;

    private bool isOpen = false;

    public void Toggle(Sprite[] items)
    {
        isOpen = !isOpen;
        if (ui != null) ui.SetActive(isOpen);

        if (playerMovement != null)
        {
            if (isOpen) playerMovement.currentState = PlayerState.Frozen;
                else playerMovement.currentState = PlayerState.Free;
        }

        UpdateChestUI(items);
    }

    public void UpdateChestUI(Sprite[] items)
    {
        for (int i = 0; i < content.transform.childCount; i++)
        {
            Transform slot = content.transform.GetChild(i);
            Transform imageObject = slot.Find("Image");

            if (i < items.Length && items[i] != null)
            {
                slot.gameObject.SetActive(true);

                if (imageObject != null)
                {
                    Image imgComponent = imageObject.GetComponent<Image>();
                    
                    if (imgComponent != null)
                    {
                        imgComponent.sprite = items[i];
                        imageObject.gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                slot.gameObject.SetActive(false);
            }
        }
    }
}