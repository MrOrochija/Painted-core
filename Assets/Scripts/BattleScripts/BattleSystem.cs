using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections;

public class BattleSystem : MonoBehaviour
{
    public GameObject player;
    private PlayerHealth playerHealth;
    private LayerMask zoneLayerMask;

    private Sprite Line;
    private Sprite Circle;
    private Sprite Triangle;
    private Sprite Square;
    [HideInInspector] public Sprite[] figures;

    private EnemyTrigger enemyTrigger;
    private EnemyHealth enemyHealth;
    private SelectAction selectAction;
    private SelectFigure selectFigure;
    private FigureSpawner figureSpawner;

    public RectTransform playerHealthBarRect;
    public RectTransform enemyHealthBarRect;

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

        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }

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

        InitializeHealthBars();
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

    public void StartBattle(EnemyTrigger script, EnemyHealth script2)
    {
        if (script != null) enemyTrigger = script;
        if (script2 != null) enemyHealth = script2;

        selectAction.Activate();

        InitializeHealthBars();
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

    public void PlayerGetDamage(int damage)
    {
        if (playerHealth != null)
        {
            playerHealth.GetDamage(damage);
            StartCoroutine(AnimateHealthBar(playerHealthBarRect, playerHealth.currentHealth, playerHealth.maxHealth));
        }
    }

    public void EnemyGetDamage(int damage)
    {
        if (enemyHealth != null)
        {
            enemyHealth.GetDamage(damage);
            StartCoroutine(AnimateHealthBar(enemyHealthBarRect, enemyHealth.currentHealth, enemyHealth.maxHealth));
        }
    }

    public void PlayerHeal(int healAmount)
    {
        if (playerHealth != null)
        {
            playerHealth.Heal(healAmount); 
            StartCoroutine(AnimateHealthBar(playerHealthBarRect, playerHealth.currentHealth, playerHealth.maxHealth));
        }
    }

    public void EnemyHeal(int healAmount)
    {
        if (enemyHealth != null)
        {
            enemyHealth.Heal(healAmount);
            StartCoroutine(AnimateHealthBar(enemyHealthBarRect, enemyHealth.currentHealth, enemyHealth.maxHealth));
        }
    }

    private void InitializeHealthBars()
    {
    if (playerHealth != null && playerHealthBarRect != null)
        {
            float targetXScale = Mathf.Clamp01((float)playerHealth.currentHealth / playerHealth.maxHealth);
            playerHealthBarRect.localScale = new Vector3(targetXScale, playerHealthBarRect.localScale.y, playerHealthBarRect.localScale.z);
        }

        if (enemyHealth != null && enemyHealthBarRect != null)
        {
            float targetXScale = Mathf.Clamp01((float)enemyHealth.currentHealth / enemyHealth.maxHealth);
            enemyHealthBarRect.localScale = new Vector3(targetXScale, enemyHealthBarRect.localScale.y, enemyHealthBarRect.localScale.z);
        }
    }

    private IEnumerator AnimateHealthBar(RectTransform barRect, float currentHealth, float maxHealth)
    {
        if (barRect == null) yield break;

        Vector3 initialScale = barRect.localScale;
        float targetXScale = Mathf.Clamp01(currentHealth / maxHealth);
        Vector3 finalScale = new Vector3(targetXScale, initialScale.y, initialScale.z);

        float timer = 0f;

        while (timer < 0.5f)
        {
            timer += Time.deltaTime;
            float progress = timer / 0.5f;

            barRect.localScale = Vector3.Lerp(initialScale, finalScale, progress);
            
            yield return null;
        }

        barRect.localScale = finalScale;
    }
}