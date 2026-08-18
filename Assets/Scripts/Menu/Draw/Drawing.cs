using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RawImage))]
public class Drawing : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [Header("State")]
    [Tooltip("Если false — рисование заблокировано")]
    public bool active = true; 

    [Header("Canvas Settings")]
    public int textureWidth = 512;
    public int textureHeight = 512;
    public Color clearColor = new Color(0f, 0f, 0f, 0f); 

    [Header("Brush Settings")]
    public Color brushColor = Color.black;
    public int brushSize = 8;

    private Texture2D texture;
    private RawImage rawImage;
    private RectTransform rectTransform;
    private Vector2 lastPixelPos;
    private bool hasLastPos = false;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
        rectTransform = GetComponent<RectTransform>();

        texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
        
        ClearCanvas();

        rawImage.texture = texture;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!active) return;

        hasLastPos = false;
        DrawAtPointer(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!active) return;

        DrawAtPointer(eventData);
    }

    private void DrawAtPointer(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, 
            eventData.position, 
            eventData.pressEventCamera, 
            out Vector2 localPoint))
        {
            Rect rect = rectTransform.rect;
            float px = (localPoint.x - rect.x) / rect.width * textureWidth;
            float py = (localPoint.y - rect.y) / rect.height * textureHeight;

            Vector2 currentPixelPos = new Vector2(px, py);

            if (hasLastPos)
            {
                DrawLine(lastPixelPos, currentPixelPos);
            }
            else
            {
                DrawCircle((int)px, (int)py);
            }

            lastPixelPos = currentPixelPos;
            hasLastPos = true;

            texture.Apply();
        }
    }

    private void DrawLine(Vector2 start, Vector2 end)
    {
        float distance = Vector2.Distance(start, end);
        for (float i = 0; i <= distance; i += 1.0f)
        {
            Vector2 point = Vector2.Lerp(start, end, i / distance);
            DrawCircle((int)point.x, (int)point.y);
        }
    }

    private void DrawCircle(int cx, int cy)
    {
        for (int x = -brushSize; x <= brushSize; x++)
        {
            for (int y = -brushSize; y <= brushSize; y++)
            {
                if (x * x + y * y <= brushSize * brushSize)
                {
                    int px = cx + x;
                    int py = cy + y;

                    if (px >= 0 && px < textureWidth && py >= 0 && py < textureHeight)
                    {
                        texture.SetPixel(px, py, brushColor);
                    }
                }
            }
        }
    }

    public void ClearCanvas()
    {
        Color[] pixels = new Color[textureWidth * textureHeight];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = clearColor;
        texture.SetPixels(pixels);
        texture.Apply();
    }

    public void SetActive(bool isActive)
    {
        active = isActive;
    }
}