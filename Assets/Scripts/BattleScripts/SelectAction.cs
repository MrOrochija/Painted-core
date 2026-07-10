using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class SelectAction : MonoBehaviour
{
    public Sprite slotSelect;
    public Sprite slotNotSelect;

    private BattleSystem battleSystem;
    private GameObject slots;
    private Image slotA;
    private Image slotB;
    private Image slotC;

    private bool active = false;
    [HideInInspector] public int currentSelection = 1;

    void Start()
    {
        battleSystem = gameObject.GetComponent<BattleSystem>();

        Transform slotsTransform = transform.Find("Slots");

        if (slotsTransform != null)
        {
            slots = slotsTransform.gameObject;

            Transform transformA = slotsTransform.Find("SlotA");
            Transform transformB = slotsTransform.Find("SlotB");
            Transform transformC = slotsTransform.Find("SlotC");

            if (transformA != null) slotA = transformA.GetComponent<Image>();
            if (transformB != null) slotB = transformB.GetComponent<Image>();
            if (transformC != null) slotC = transformC.GetComponent<Image>();
        }

        UpdateSlots();
    }

    void Update()
    {
        if (active && Keyboard.current != null)
        {
            if (Keyboard.current.dKey.wasPressedThisFrame)
            {
                if (currentSelection < 3) 
                {
                    currentSelection++;
                    UpdateSlots();
                }
            }

            if (Keyboard.current.aKey.wasPressedThisFrame)
            {
                
                if (currentSelection > 1) 
                {
                    currentSelection--;
                    UpdateSlots();
                }
            }

            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                battleSystem.SelectAction(currentSelection);
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
        slots.SetActive(true);
        active = true;
    }

    public void Deactivate()
    {
        slots.SetActive(false);
        active = false;
    }
}
