using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectFigure : MonoBehaviour
{
    public Sprite Line;
    public Sprite Circle;
    public Sprite Triangle;
    public Sprite Square;

    public GameObject figureModel;
    private SpriteRenderer figureSr;
    private Sprite[] figures;
    [HideInInspector] public int currentFigureIndex = 0;

    public Sprite leftArrow;
    public Sprite leftBoldArrow;
    public Sprite rightArrow;
    public Sprite rightBoldArrow;

    public GameObject leftArrowModel;
    public GameObject rightArrowModel;

    private SpriteRenderer leftArrowSr;
    private SpriteRenderer rightArrowSr;

    private bool active = false;

    void Start()
    {
        if (leftArrowModel != null) leftArrowSr = leftArrowModel.GetComponent<SpriteRenderer>();
        if (rightArrowModel != null) rightArrowSr = rightArrowModel.GetComponent<SpriteRenderer>();

        if (figureModel != null) figureSr = figureModel.GetComponent<SpriteRenderer>();
        
        figures = new Sprite[] { Line, Circle, Triangle, Square };

        UpdateFigureSprite();
    }

    void Update()
    {
        if (active && Keyboard.current != null)
        {
            if (Keyboard.current.dKey.wasPressedThisFrame)
            {
                if (rightArrowSr != null) StartCoroutine(FlashArrow(rightArrowSr, rightBoldArrow, rightArrow));
                
                ChangeFigure(1);
            }

            if (Keyboard.current.aKey.wasPressedThisFrame)
            {
                if (leftArrowSr != null) StartCoroutine(FlashArrow(leftArrowSr, leftBoldArrow, leftArrow));
                
                ChangeFigure(-1);
            }
        }
    }

    private void ChangeFigure(int direction)
    {
        if (figures == null || figures.Length == 0) return;

        currentFigureIndex += direction;

        if (currentFigureIndex >= figures.Length)
        {
            currentFigureIndex = 0;
        }
        else if (currentFigureIndex < 0)
        {
            currentFigureIndex = figures.Length - 1;
        }

        UpdateFigureSprite();
    }

    private void UpdateFigureSprite()
    {
        if (figureSr != null && figures[currentFigureIndex] != null)
        {
            figureSr.sprite = figures[currentFigureIndex];
        }
    }

    private IEnumerator FlashArrow(SpriteRenderer sr, Sprite boldSprite, Sprite normalSprite)
    {
        sr.sprite = boldSprite;
        yield return new WaitForSeconds(0.1f);
        sr.sprite = normalSprite;
    }

    public void Activate()
    {
        active = true;
    }

    public void Deactivate()
    {
        active = false;
    }
}