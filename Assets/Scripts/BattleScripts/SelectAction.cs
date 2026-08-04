using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class SelectAction : SoundsModule
{
    public Sprite slotSelect;
    public Sprite slotNotSelect;

    public GameObject slots;
    public Image slotA;
    public Image slotB;
    public Image slotC;

    private BattleSystem battleSystem;
    private bool active = false;
    
    [HideInInspector] public int currentSelection = 1;

    void Start()
    {
        battleSystem = GetComponent<BattleSystem>();

        if (slots == null)
        {
            Transform slotsTransform = transform.Find("Slots");
            if (slotsTransform != null)
            {
                slots = slotsTransform.gameObject;
                slotA = slotsTransform.Find("SlotA")?.GetComponent<Image>();
                slotB = slotsTransform.Find("SlotB")?.GetComponent<Image>();
                slotC = slotsTransform.Find("SlotC")?.GetComponent<Image>();
            }
        }

        UpdateSlots();
    }

    void Update()
    {
        if (active && Keyboard.current != null)
        {
            if (Keyboard.current.dKey.wasPressedThisFrame && currentSelection < 3)
            {
                currentSelection++;
                PlaySound(sounds[0]);
                UpdateSlots();
            }
            else if (Keyboard.current.aKey.wasPressedThisFrame && currentSelection > 1)
            {
                currentSelection--;
                PlaySound(sounds[0]);
                UpdateSlots();
            }
            else if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                PlaySound(sounds[1]);
                
                battleSystem.SelectAction((BattleActionType)currentSelection);
            }
        }
    }

    public void UpdateSlots()
    {
        SetSprite(slotA, currentSelection == 1);
        SetSprite(slotB, currentSelection == 2);
        SetSprite(slotC, currentSelection == 3);
    }

    private void SetSprite(Image slotImage, bool isSelected)
    {
        if (slotImage != null)
        {
            slotImage.sprite = isSelected ? slotSelect : slotNotSelect;
        }
    }

    public void Activate()
    {
        if (slots != null) slots.SetActive(true);
        active = true;
    }

    public void Deactivate()
    {
        if (slots != null) slots.SetActive(false);
        active = false;
    }
}