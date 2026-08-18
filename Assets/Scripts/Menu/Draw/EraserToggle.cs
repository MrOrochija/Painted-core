using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class EraserToggle : MonoBehaviour
{
    public Drawing drawableCanvas;
    public Toggle fillToggle;

    private Toggle toggle;
    private Color savedBrushColor = Color.black;

    void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnEraserToggled);
    }

    private void OnEraserToggled(bool isEraserActive)
    {
        if (drawableCanvas == null) return;

        if (isEraserActive)
        {
            if (fillToggle != null && fillToggle.isOn)
            {
                fillToggle.isOn = false;
            }

            savedBrushColor = drawableCanvas.brushColor;
            drawableCanvas.brushColor = drawableCanvas.clearColor;
        }
        else
        {
            drawableCanvas.brushColor = savedBrushColor;
        }
    }
}