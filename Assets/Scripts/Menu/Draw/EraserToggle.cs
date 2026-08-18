using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class EraserToggle : MonoBehaviour
{
    public Drawing drawableCanvas;

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
            savedBrushColor = drawableCanvas.brushColor;

            drawableCanvas.brushColor = drawableCanvas.clearColor;
        }
        else
        {
            drawableCanvas.brushColor = savedBrushColor;
        }
    }
}