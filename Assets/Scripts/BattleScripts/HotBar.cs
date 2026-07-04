using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class HotBar : MonoBehaviour
{
    public Sprite slotSelect;
    public Sprite slotNotSelect;

    private int currentSelection = 1;
    [HideInInspector] public bool isBattleActive;
    [HideInInspector] public EnemyInteract enemyScript;
    private BattleSystem BattleSystemScript;

    public GameObject battleZone;
    public GameObject selectFigure;

    private Image imgA;
    private Image imgB;
    private Image imgC;

    [HideInInspector] public bool isFighting = false;

    void Start()
    {
        if (battleZone != null)
        {
            BattleSystemScript = battleZone.GetComponent<BattleSystem>();    
        }

        imgA = FindSlotImage("slotA");
        imgB = FindSlotImage("slotB");
        imgC = FindSlotImage("slotC");

        UpdateSlotVisuals();
    }

    void Update()
    {
        if (!isFighting && isBattleActive && Keyboard.current != null)
        {
            if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
            {
                if (currentSelection < 3) 
                {
                    currentSelection++;
                    UpdateSlotVisuals();
                }
            }

            if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
            {
                if (currentSelection > 1) 
                {
                    currentSelection--;
                    UpdateSlotVisuals();
                }
            }

            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                if (currentSelection == 1)
                {
                    isFighting = true;
                    battleZone.SetActive(true);
                    selectFigure.SetActive(true);

                    StartCoroutine(BattleSystemScript.startBattle());
                }

                if (currentSelection == 2)
                {
                    battleZone.SetActive(false);
                }

                if (currentSelection == 3)
                {
                    battleZone.SetActive(false);

                    int randomInt = Random.Range(0, 2);

                    if (randomInt == 0)
                    {
                        isBattleActive = false;
                        StartCoroutine(enemyScript.leave());
                        Debug.Log("Won!");
                    } else
                    {
                        Debug.Log("Lose!");
                    }
                }
            }
        }
    }

    public void UpdateSlotVisuals()
    {
        SetSprite(imgA, currentSelection == 1);
        SetSprite(imgB, currentSelection == 2);
        SetSprite(imgC, currentSelection == 3);
    }

    private void SetSprite(Image slotImage, bool isSelected)
    {
        if (slotImage != null)
        {
            slotImage.sprite = isSelected ? slotSelect : slotNotSelect;
        }
    }

    private Image FindSlotImage(string slotName)
    {
        Transform slotTransform = transform.Find(slotName);
        if (slotTransform == null)
        {
            Debug.LogError($"[HotBar] Не найден объект с именем {slotName} на {gameObject.name}!");
            return null;
        }

        return slotTransform.GetComponent<Image>();
    }
}