using UnityEngine.InputSystem;
using UnityEngine;
using Unity.VisualScripting;

public class BattleSystem : MonoBehaviour
{
    private LayerMask zoneLayerMask;

    private Sprite Line;
    private Sprite Circle;
    private Sprite Triangle;
    private Sprite Square;
    [HideInInspector] public Sprite[] figures;

    private EnemyTrigger enemyTrigger;
    private SelectAction selectAction;
    private SelectFigure selectFigure;
    private FigureSpawner figureSpawner;

    private GameObject battleZone;
    [HideInInspector] public GameObject zone;

    private Camera mainCamera;

    void Awake()
    {
        figureSpawner = gameObject.GetComponent<FigureSpawner>();
        selectAction = gameObject.GetComponent<SelectAction>();
        selectFigure = gameObject.GetComponent<SelectFigure>();

        Line = selectFigure.Line;
        Circle = selectFigure.Circle;
        Triangle = selectFigure.Triangle;
        Square = selectFigure.Square;

        figures = new Sprite[] { Line, Circle, Triangle, Square };

        zoneLayerMask = LayerMask.GetMask("BattleZone");

        Transform battleZoneTransform = transform.Find("BattleZone");
        if (battleZoneTransform != null)
        {
            battleZone = battleZoneTransform.gameObject;
            Transform zoneTransform = battleZone.transform.Find("Zone");

            if (zoneTransform != null)
            {
                zone = zoneTransform.gameObject;
            }
        }
    }

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            DetectZoneClick();
        }
    }

    private void DetectZoneClick()
    {
        if (figures == null || figures.Length == 0 || zone == null) return;

        Vector2 mouseScreenPos = Pointer.current.position.ReadValue();
        
        Vector3 targetScreenPos = new Vector3(mouseScreenPos.x, mouseScreenPos.y, -mainCamera.transform.position.z);
        Vector3 mouseWorldPosition3D = mainCamera.ScreenToWorldPoint(targetScreenPos);
        Vector2 mouseWorldPosition = new Vector2(mouseWorldPosition3D.x, mouseWorldPosition3D.y);

        Collider2D hitCollider = Physics2D.OverlapPoint(mouseWorldPosition, zoneLayerMask);

        if (hitCollider != null && hitCollider.gameObject == zone)
        {
            int index = Mathf.Clamp(selectFigure.currentFigureIndex, 0, figures.Length - 1);
            figureSpawner.SpawnFigure(figures[index], mouseWorldPosition, "Player");
        }
    }

    public void StartBattle(EnemyTrigger script)
    {
        if (script != null) enemyTrigger = script;
        selectAction.Activate();
    }

    public void SelectAction(int action)
    {
        switch (action)
        {
            case 1:
                selectAction.Deactivate();
                battleZone.SetActive(true);
                selectFigure.Activate();
                figureSpawner.Activate();
                break;
            case 2:
                break;
            case 3:
                int randomInt = Random.Range(0, 2);
                if (randomInt == 0 && enemyTrigger != null)
                {
                    StartCoroutine(enemyTrigger.RunAway());
                }
                break;
        }
    }
}