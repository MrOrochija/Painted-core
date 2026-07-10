using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SetCheckpoint : MonoBehaviour
{
    private Canvas UI;
    private bool isOpen = false;

    public Button yesButton;
    public Button noButton;

    void Start()
    {
        Transform canvasTransform = transform.Find("UI");
        if (canvasTransform != null)
        {
            UI = canvasTransform.GetComponent<Canvas>();
            UI.enabled = isOpen;
        }

        if (yesButton != null)
        {
            yesButton.onClick.AddListener(YesButtonClick);
        }
        if (noButton != null)
        {
            noButton.onClick.AddListener(NoButtonClick);
        }
    }

    void YesButtonClick()
    {
        ToggleUI();
    }

    void NoButtonClick()
    {
        ToggleUI();
    }

    void Update()
    {
        if (UI == null) return;

        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            ToggleUI();
        }
    }

    private void ToggleUI()
    {
        isOpen = !isOpen;
        UI.enabled = isOpen;
    }
}