using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;

[Serializable]
public class ShopItemData
{
    public string itemName;
    public int price;
}

public class ShopSystem : SoundsModule
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private ShopBuy shopBuy;
    [SerializeField] private InventorySystem inventorySystem;
    [SerializeField] private GameObject ui;
    [SerializeField] private TMP_Text mainText;
    [SerializeField] private TMP_Text dialogueText;

    [SerializeField] private GameObject selectPanel;
    [SerializeField] private GameObject buyPanel;
    [SerializeField] private GameObject buyDialoguePanel;

    [SerializeField] private List<TMP_Text> uiSlots = new List<TMP_Text>();

    private List<ShopItemData> currentShopItems;

    private Coroutine typeRoutine;
    private readonly WaitForSeconds textDelay = new WaitForSeconds(0.03f);

    public void Active(string messageText, List<ShopItemData> itemsToSell)
    {
        currentShopItems = itemsToSell;

        if (playerMovement != null) playerMovement.currentState = PlayerState.Frozen;
        if (ui != null) ui.SetActive(true);

        StartTyping(messageText, mainText);
    }

    public void Buy()
    {
        if (selectPanel != null) selectPanel.SetActive(false);
        if (buyPanel != null) buyPanel.SetActive(true);

        foreach (var slot in uiSlots)
        {
            if (slot != null) slot.text = "";
        }

        for (int i = 0; i < currentShopItems.Count; i++)
        {
            if (i < uiSlots.Count && uiSlots[i] != null)
            {
                uiSlots[i].text = currentShopItems[i].itemName;
            }
        }
    }

    public void ExitBuy()
    {
        if (buyPanel != null) buyPanel.SetActive(false);
        if (selectPanel != null) selectPanel.SetActive(true);
        if (mainText != null) mainText.gameObject.SetActive(true);
        if (buyDialoguePanel != null) buyDialoguePanel.SetActive(false);
    }

    public void BuyItem()
    {
        inventorySystem.AddItem(shopBuy.itemName);
    }

    public void Talk()
    {
        
    }

    public void Exit()
    {
        if (playerMovement != null) playerMovement.currentState = PlayerState.Free;
        if (ui != null) ui.SetActive(false);

        currentShopItems = null;
        StopCurrentTyping();
    }

    public void ClickSlot(int slotIndex)
    {
        if (currentShopItems == null || slotIndex < 0 || slotIndex >= currentShopItems.Count) return;

        if (mainText != null) mainText.gameObject.SetActive(false);
        if (buyDialoguePanel != null) buyDialoguePanel.SetActive(true);

        ShopItemData item = currentShopItems[slotIndex];
        string messageText = $"{item.itemName} costs {item.price}\nAre you sure you want to buy this?";
        
        shopBuy.itemName = item.itemName;
        shopBuy.price = item.price;

        StartTyping(messageText, dialogueText);
    }

    private void StartTyping(string targetText, TMP_Text label)
    {
        if (label == null) return;

        StopCurrentTyping();
        typeRoutine = StartCoroutine(TypeTextRoutine(targetText, label));
    }

    private void StopCurrentTyping()
    {
        if (typeRoutine != null)
        {
            StopCoroutine(typeRoutine);
            typeRoutine = null;
        }
    }

    private IEnumerator TypeTextRoutine(string targetText, TMP_Text label)
    {
        label.text = targetText;
        label.maxVisibleCharacters = 0;
        label.ForceMeshUpdate();

        int totalVisibleCharacters = label.textInfo.characterCount;

        for (int i = 0; i <= totalVisibleCharacters; i++)
        {
            label.maxVisibleCharacters = i;

            if (i > 0 && label.textInfo.characterInfo[i - 1].character != ' ')
            {
                if (sounds != null && sounds.Length > 0)
                {
                    PlaySound(sounds[0]);
                }
            }

            yield return textDelay;
        }

        label.maxVisibleCharacters = totalVisibleCharacters;
        typeRoutine = null;
    }
}