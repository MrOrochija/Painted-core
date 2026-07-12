using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public struct ItemData
{
    public string itemName;
    public string bio;
    public Sprite itemSprite;
    public ItemEffect effect;
}

public struct InventoryItem
{
    public string name;
    public string bio;

    public InventoryItem(string name, string bio)
    {
        this.name = name;
        this.bio = bio;
    }
}

public class InventorySystem : MonoBehaviour
{
    public ItemData[] allItems;

    private List<InventoryItem> currentInventory = new List<InventoryItem>();

    public GameObject player;
    private PlayerMovement playerMovement;

    public GameObject dialogueObject;
    private Dialogue dialogue;

    public GameObject button; 
    private Image image;
    private Canvas UI;
    public GameObject background;
    private bool active = true;

    void Start()
    {
        dialogue = dialogueObject.GetComponent<Dialogue>();
        playerMovement = player.GetComponent<PlayerMovement>();

        Transform UITransform = transform.Find("UI");

        if (UITransform != null)
        {
            UI = UITransform.GetComponent<Canvas>();
            Transform backgroundTransform = UI.transform.Find("Background");

            if (backgroundTransform != null)
            {
                background = backgroundTransform.gameObject;
            }
        }

        Transform buttonTransform = transform.Find("Button");

        if (buttonTransform != null)
        {
            button = buttonTransform.gameObject;

            Transform slotTransform = button.transform.Find("Slot");
            if (slotTransform != null)
            {
                Transform imageTransform = slotTransform.Find("Image");
                if (imageTransform != null)
                {
                    image = imageTransform.GetComponent<Image>();
                }
            }
        }
    }

    void Update()
    {
        if (!active) return;
    
        if (Keyboard.current != null && Keyboard.current.qKey.wasPressedThisFrame)
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        UI.enabled = !UI.enabled;
    
        if (UI.enabled)
        {
            playerMovement.currentState = PlayerState.Frozen;
        } else
        {
            if (playerMovement.currentState != PlayerState.Combat)
            {
                playerMovement.currentState = PlayerState.Free;
            }
        }
    }

    public void SlotWasClicked(GameObject slot, string itemName, string bio)
    {
        ItemData foundItem = Array.Find(allItems, item => item.itemName == itemName);
        Sprite targetSprite = null;

        if (!string.IsNullOrEmpty(foundItem.itemName))
        {
            targetSprite = foundItem.itemSprite;
        }

        dialogue.Activate(slot, itemName, bio, targetSprite);
        UI.enabled = false;
    }

    public void UseItemByName(string targetName)
    {
        ItemData foundItem = Array.Find(allItems, item => item.itemName == targetName);

        if (!string.IsNullOrEmpty(foundItem.itemName))
        {
            if (foundItem.effect != null)
            {
                foundItem.effect.Execute(player);
            }
        }
    }

    public void AddItem(string itemName)
    {
        foreach (ItemData item in allItems)
        {
            if (item.itemName == itemName)
            {
                currentInventory.Add(new InventoryItem(item.itemName, item.bio));

                if (button != null && background != null)
                {
                    GameObject newButton = Instantiate(button, background.transform);
                    newButton.SetActive(true);

                    Slot slotScript = newButton.GetComponent<Slot>();
                    if (slotScript != null)
                    {
                        slotScript.itemName = item.itemName;
                        slotScript.bio = item.bio;
                        slotScript.invSystemObject = gameObject; 
                    }

                    Transform slotTransform = newButton.transform.Find("Slot");
                    if (slotTransform != null)
                    {
                        Transform imageTransform = slotTransform.Find("Image");
                        if (imageTransform != null)
                        {
                            Image newImage = imageTransform.GetComponent<Image>();
                            if (newImage != null)
                            {
                                newImage.sprite = item.itemSprite; 
                            }
                        }
                    }
                }

                break;
            }
        }
    }

    public void RemoveItem()
    {
        
    }

    public void Activate()
    {
        active = true;
    }

    public void Deactivate()
    {
        active = false;
        UI.enabled = false;
    }
}