using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Dialogue : MonoBehaviour
{
    public GameObject invSystem;
    private InventorySystem inventorySystem;
    private Canvas UI;
    private Canvas dialogueCanva;

    private PlayerMovement playerMovement;

    [HideInInspector] public TextMeshProUGUI itemName;
    [HideInInspector] public TextMeshProUGUI itemBio;
    [HideInInspector] public Image itemPicture;

    public Button use;
    public Button nothing;

    private GameObject targetSlot;

    void Start()
    {
        inventorySystem = invSystem.GetComponent<InventorySystem>();
        dialogueCanva = gameObject.GetComponent<Canvas>();
        
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerMovement = playerObj.GetComponent<PlayerMovement>();
        }

        if (use != null) 
            use.onClick.AddListener(OnUsePressed);
            
        if (nothing != null) 
            nothing.onClick.AddListener(OnNothingPressed);

        Transform UITransform = invSystem.transform.Find("UI");
        if (UITransform != null)
        {
            UI = UITransform.GetComponent<Canvas>();
        }
    }
    
    public void Activate(GameObject slot, string newItemName, string bio, Sprite sprite)
    {
        targetSlot = slot;
        itemName.text = newItemName;
        itemBio.text = bio;
        
        if (itemPicture != null)
        {
            itemPicture.sprite = sprite;
        }

        dialogueCanva.enabled = true;
    }

    public void OnUsePressed()
    {
        inventorySystem.UseItemByName(itemName.text);
        Destroy(targetSlot);
    
        CloseDialogue();
    }

    public void OnNothingPressed()
    {
        CloseDialogue();
    }

    private void CloseDialogue()
    {
        dialogueCanva.enabled = false;
        UI.enabled = true;
    }
}