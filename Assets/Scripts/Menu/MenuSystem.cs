using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuSystem : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public GameObject buttons;
    public TMP_Text text;
    public GameObject uI;
    public GameObject background;

    private bool start = false;
    private GameObject settings;

    void Start()
    {
        playerMovement.currentState = PlayerState.Frozen;

        Transform settingsTransform = uI.transform.Find("Settings");

        if (settingsTransform != null) settings = settingsTransform.gameObject;

        buttons.SetActive(true);
        settings.SetActive(false);
        background.SetActive(true);
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            SetMenu(true);
        }
    }

    public void Play()
    {
        if (!start)
        {
            start = true;
            text.text = "Continue";
        }

        SetMenu(false);
    }

    public void Settings()
    {
        settings.SetActive(true);
    }

    private void SetMenu(bool value)
    {
        buttons.SetActive(value);
        background.SetActive(value);
        settings.SetActive(false);

        if (value) playerMovement.currentState = PlayerState.Frozen;
            else playerMovement.currentState = PlayerState.Free;
    }
}
