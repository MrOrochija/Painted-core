using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class FillToggle : MonoBehaviour
{
    public Drawing drawableCanvas;
    public Toggle eraserToggle;

    private Toggle toggle;

    void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnFillToggled);
    }

    private void OnFillToggled(bool isFillActive)
    {
        if (drawableCanvas == null) return;

        if (isFillActive)
        {
            if (eraserToggle != null && eraserToggle.isOn)
            {
                eraserToggle.isOn = false;
            }
        }

        drawableCanvas.fill = isFillActive;
    }
}