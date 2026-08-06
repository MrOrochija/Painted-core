using System.Collections;
using UnityEngine;

public class FigureSpawner : MonoBehaviour
{
    public GameObject bEnemy;
    private SelectAction selectAction;
    private SelectFigure selectFigure;
    private BattleSystem battleSystem;
    private GameObject battleZone;
    private BattleFigure[] figures; 

    private GameObject plrFigure;
    private GameObject enemyFigure;
    private GameObject zone;
    private Collider2D zoneCollider;

    private bool active = false;
    
    private float spawnCooldown = 5.0f;
    private float spawnTimer = 0f;

    [HideInInspector] public int spawnedCount = 0;
    private int enemySpawnedCount = 0;

    private Animator bEnemyAnim;

    void Start()
    {
        battleSystem = gameObject.GetComponent<BattleSystem>();
        selectAction = gameObject.GetComponent<SelectAction>();
        selectFigure = gameObject.GetComponent<SelectFigure>();

        figures = battleSystem.figures; 
        zone = battleSystem.zone;

        if (zone != null) zoneCollider = zone.GetComponent<Collider2D>();

        Transform battleZoneTransform = transform.Find("BattleZone");
        if (battleZoneTransform != null) battleZone = battleZoneTransform.gameObject;

        Transform plrFigureTransform = transform.Find("PlrFigure");
        if (plrFigureTransform != null) plrFigure = plrFigureTransform.gameObject;

        Transform enemyFigureTransform = transform.Find("EnemyFigure");
        if (enemyFigureTransform != null) enemyFigure = enemyFigureTransform.gameObject;

        if (bEnemy != null) bEnemyAnim = bEnemy.GetComponent<Animator>();
    }

    void Update()
    {
        if (!active) return;

        if (enemySpawnedCount >= 5)
        {
            EnemyFigure[] aliveFigures = Object.FindObjectsByType<EnemyFigure>();

            if (aliveFigures.Length == 0)
            {
                Deactivate();
                selectFigure.Deactivate();
                StartCoroutine(BattleZoneActive(false));
            }
            return; 
        }

        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            SpawnRandomFigureInZone();
            spawnTimer = spawnCooldown;
        }
    }

    private IEnumerator BattleZoneActive(bool value)
    {
        yield return new WaitForSeconds(2.0f);

        battleZone.SetActive(value);
        selectAction.Activate();
    }

    private void SpawnRandomFigureInZone()
    {
        if (figures == null || figures.Length == 0 || zoneCollider == null) return;

        int randomIndex = Random.Range(0, figures.Length);
        BattleFigure randomFigure = figures[randomIndex];

        Bounds bounds = zoneCollider.bounds;

        float minX = bounds.min.x + 0.5f;
        float maxX = bounds.max.x - 0.5f;
        float minY = bounds.min.y + 0.5f;
        float maxY = bounds.max.y - 0.5f;

        if (minX > maxX) minX = maxX = bounds.center.x;
        if (minY > maxY) minY = maxY = bounds.center.y;

        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        Vector2 randomCoordinates = new Vector2(randomX, randomY);

        SpawnFigure(randomFigure, randomCoordinates, "Enemy");
    }

    public void SpawnFigure(BattleFigure figure, Vector2 coordinates, string owner)
    {
        GameObject prefab = owner == "Player" ? plrFigure : enemyFigure;
        if (prefab == null || figure == null) return;

        GameObject newFigure = Instantiate(prefab, coordinates, Quaternion.identity);
        SpriteRenderer spriteRenderer = newFigure.GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = figure.sprite;
        }

        if (figure.name == "Line" || figure.name == "Triangle")
        {
            StartCoroutine(TriggerAnim("Attack"));
        }
        else if (figure.name == "Circle" || figure.name == "Square")
        {
            StartCoroutine(TriggerAnim("Block"));
        }

        newFigure.SetActive(true);

        if (owner == "Enemy")
        {
            enemySpawnedCount++;
        }

        spawnedCount++;
    }

    private IEnumerator TriggerAnim(string paramName)
    {
        bEnemyAnim.SetBool(paramName, true);
        yield return new WaitForSeconds(0.1f);
        bEnemyAnim.SetBool(paramName, false);
    }

    public void Activate()
    {
        active = true;
        spawnTimer = 0f;
        spawnedCount = 0;
        enemySpawnedCount = 0;
    }

    public void Deactivate()
    {
        active = false;
    }
}