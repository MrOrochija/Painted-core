using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;

[Serializable]
public class ShopItem
{
    public string itemName;
    public int price;
    public TMP_Text slotText;
}

public class ShopSystem : SoundsModule
{
    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private ShopBuy shopBuy;
    [SerializeField] private InventorySystem inventorySystem;
    [SerializeField] private GameObject ui;
    [SerializeField] private TMP_Text mainText;
    [SerializeField] private TMP_Text dialogueText;

    [Header("UI Panels")]
    [SerializeField] private GameObject selectPanel;
    [SerializeField] private GameObject buyPanel;
    [SerializeField] private GameObject buyDialoguePanel;

    [Header("Shop Content")]
    [SerializeField] private List<ShopItem> items = new List<ShopItem>();

    private Coroutine typeRoutine;
    private readonly WaitForSeconds textDelay = new WaitForSeconds(0.03f);

    public void Active(string messageText)
    {
        if (playerMovement != null) playerMovement.currentState = PlayerState.Frozen;
        if (ui != null) ui.SetActive(true);

        StartTyping(messageText, mainText);
    }

    public void Buy()
    {
        if (selectPanel != null) selectPanel.SetActive(false);
        if (buyPanel != null) buyPanel.SetActive(true);

        foreach (var item in items)
        {
            if (item.slotText != null && !string.IsNullOrEmpty(item.itemName))
            {
                item.slotText.text = item.itemName;
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

        StopCurrentTyping();
    }

    public void ClickSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= items.Count) return;

        if (mainText != null) mainText.gameObject.SetActive(false);
        if (buyDialoguePanel != null) buyDialoguePanel.SetActive(true);

        ShopItem item = items[slotIndex];
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