using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ClickableItem : MonoBehaviour, IPointerClickHandler
{
    public GameObject selectFigure;
    private SelectFigure selectFigureScript;

    private GameObject playerAttackSprite;

    private Sprite Line;
    private Sprite Circle;
    private Sprite Triangle;
    private Sprite Square;

    void Start()
    {
        Transform spriteTransform = gameObject.transform.Find("PlayerAttack");

        if (spriteTransform != null)
        {
            playerAttackSprite = spriteTransform.gameObject;
        }

        if (selectFigure != null)
        {
            selectFigureScript = selectFigure.GetComponent<SelectFigure>();
        }

        if (selectFigureScript != null)
        {
            Line = selectFigureScript.Line;
            Circle = selectFigureScript.Circle;
            Triangle = selectFigureScript.Triangle;
            Square = selectFigureScript.Square;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Vector2 screenPos = eventData.position;

        if (Camera.main != null)
        {
            Vector3 mousePos = new Vector3(screenPos.x, screenPos.y, Mathf.Abs(Camera.main.transform.position.z));
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            worldPos.z = 0;
        
            if (playerAttackSprite != null)
            {
                GameObject newSpawnedObject = Instantiate(playerAttackSprite);

                newSpawnedObject.transform.SetParent(null);

                newSpawnedObject.transform.position = worldPos;

                newSpawnedObject.transform.rotation = Quaternion.identity;

                newSpawnedObject.SetActive(true);
            
                SpriteRenderer sr = newSpawnedObject.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    switch (selectFigureScript.currentSelection)
                    {
                        case 1: sr.sprite = Line; break;
                        case 2: sr.sprite = Circle; break;
                        case 3: sr.sprite = Triangle; break;
                        case 4: sr.sprite = Square; break;
                    }
                }

                StartCoroutine(attackDestroy(newSpawnedObject));
            }
        }
    }

    private IEnumerator attackDestroy(GameObject newSpawnedObject)
    {
        yield return new WaitForSeconds(2f);

        if (newSpawnedObject != null)
        {
            Destroy(newSpawnedObject);
        }
    }
}