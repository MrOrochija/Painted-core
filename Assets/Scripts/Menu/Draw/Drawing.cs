using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RawImage))]
public class Drawing : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public bool active = true; 
    public bool fill = false;

    public int textureWidth = 512;
    public int textureHeight = 512;
    public Color clearColor = new Color(0f, 0f, 0f, 0f); 

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
        if (!active || fill) return;

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
            int px = Mathf.Clamp((int)((localPoint.x - rect.x) / rect.width * textureWidth), 0, textureWidth - 1);
            int py = Mathf.Clamp((int)((localPoint.y - rect.y) / rect.height * textureHeight), 0, textureHeight - 1);

            if (fill)
            {
                FloodFill(px, py);
                return;
            }

            Vector2 currentPixelPos = new Vector2(px, py);

            if (hasLastPos)
            {
                DrawLine(lastPixelPos, currentPixelPos);
            }
            else
            {
                DrawCircle(px, py);
            }

            lastPixelPos = currentPixelPos;
            hasLastPos = true;

            texture.Apply();
        }
    }

    private void FloodFill(int startX, int startY)
    {
        Color[] pixels = texture.GetPixels();
        Color targetColor = pixels[startY * textureWidth + startX];

        if (ColorsMatch(targetColor, brushColor)) return;

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(new Vector2Int(startX, startY));

        bool[] visited = new bool[textureWidth * textureHeight];
        visited[startY * textureWidth + startX] = true;

        while (queue.Count > 0)
        {
            Vector2Int pt = queue.Dequeue();
            int index = pt.y * textureWidth + pt.x;

            pixels[index] = brushColor;

            CheckNeighbor(pt.x + 1, pt.y, targetColor, pixels, visited, queue);
            CheckNeighbor(pt.x - 1, pt.y, targetColor, pixels, visited, queue);
            CheckNeighbor(pt.x, pt.y + 1, targetColor, pixels, visited, queue);
            CheckNeighbor(pt.x, pt.y - 1, targetColor, pixels, visited, queue);
        }

        texture.SetPixels(pixels);
        texture.Apply();
    }

    private void CheckNeighbor(int x, int y, Color targetColor, Color[] pixels, bool[] visited, Queue<Vector2Int> queue)
    {
        if (x < 0 || x >= textureWidth || y < 0 || y >= textureHeight) return;

        int index = y * textureWidth + x;
        if (!visited[index] && ColorsMatch(pixels[index], targetColor))
        {
            visited[index] = true;
            queue.Enqueue(new Vector2Int(x, y));
        }
    }

    private bool ColorsMatch(Color a, Color b)
    {
        return Mathf.Approximately(a.r, b.r) &&
               Mathf.Approximately(a.g, b.g) &&
               Mathf.Approximately(a.b, b.b) &&
               Mathf.Approximately(a.a, b.a);
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