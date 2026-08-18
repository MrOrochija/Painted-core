using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuSystem : MonoBehaviour
{
    public Drawing drawing;
    public GameObject wheel;
    public GameObject draw;
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
        draw.SetActive(true);
        drawing.gameObject.SetActive(true);
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

    public void Exit()
    {
        settings.SetActive(false);
    }

    private void SetMenu(bool value)
    {
        buttons.SetActive(value);
        background.SetActive(value);
        settings.SetActive(false);
        draw.SetActive(value);
        drawing.gameObject.SetActive(value);

        if (value) playerMovement.currentState = PlayerState.Frozen;
            else playerMovement.currentState = PlayerState.Free;
    }

    public void Draw()
    {
        if (drawing != null)
        {
            drawing.active = !drawing.active;
            buttons.SetActive(!drawing.active);
            wheel.SetActive(drawing.active);
        }
    }
}
