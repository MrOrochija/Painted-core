using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_InputField))]
public class BrushSizeInput : MonoBehaviour
{
    public Drawing drawableCanvas;

    public int minSize = 1;
    public int maxSize = 100;

    private TMP_InputField inputField;

    void Awake()
    {
        inputField = GetComponent<TMP_InputField>();
        
        inputField.contentType = TMP_InputField.ContentType.IntegerNumber;

        inputField.onValueChanged.AddListener(OnInputChanged);
    }

    void Start()
    {
        if (drawableCanvas != null)
        {
            inputField.text = drawableCanvas.brushSize.ToString();
        }
    }

    private void OnInputChanged(string textValue)
    {
        if (drawableCanvas == null) return;

        if (int.TryParse(textValue, out int newSize))
        {
            drawableCanvas.brushSize = Mathf.Clamp(newSize, minSize, maxSize);
        }
    }
}