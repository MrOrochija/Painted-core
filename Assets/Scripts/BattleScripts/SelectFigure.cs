using UnityEngine;
using UnityEngine.InputSystem;

public class SelectFigure : MonoBehaviour
{
    public Sprite Line;
    public Sprite Circle;
    public Sprite Triangle;
    public Sprite Square;

    private GameObject squareObject;
    private SpriteRenderer spriteRenderer;

    [HideInInspector] public int currentSelection = 1;

    void Start()
    {
        Transform squareTransform = transform.Find("Frame/Square");

        if (squareTransform != null)
        {
            squareObject = squareTransform.gameObject;
            spriteRenderer = squareObject.GetComponent<SpriteRenderer>();
        }

        UpdateSprite();
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            currentSelection++;
            if (currentSelection > 4)
            {
                currentSelection = 1;
            }
            UpdateSprite();
        }

        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            currentSelection--;
            if (currentSelection < 1)
            {
                currentSelection = 4;
            }
            UpdateSprite();
        }
    }

    private void UpdateSprite()
    {
        if (spriteRenderer == null) return;

        switch (currentSelection)
        {
            case 1:
                spriteRenderer.sprite = Line;
                break;
            case 2:
                spriteRenderer.sprite = Circle;
                break;
            case 3:
                spriteRenderer.sprite = Triangle;
                break;
            case 4:
                spriteRenderer.sprite = Square;
                break;
        }
    }
}